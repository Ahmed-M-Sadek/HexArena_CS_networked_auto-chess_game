using ASU2019_NetworkedGameWorkshop.controller.networking;
using ASU2019_NetworkedGameWorkshop.controller.networking.lobby;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.view
{
    public partial class ConnectForm : Form
    {
        private readonly List<Tuple<string, long, string>> ips;

        private bool isConnectedToServer;
        private int lastPortScanned;

        public ConcurrentQueue<string> LobbyMembers { get; private set; }
        public bool StartGame { get; set; }

        private Timer timer;
        private LobbyClient lobbyClient;
        private LobbyServer lobbyServer;
        private string connectedIP;
        private int connectedPort;

        public ConnectForm()
        {
            InitializeComponent();

            ips = new List<Tuple<string, long, string>>();
        }

        private void ConnectForm_Load(object sender, EventArgs e)
        {
            txt_connectPort.Text = txt_hostPort.Text = GameNetworkUtilities.DEFAULT_PORT.ToString();
            txt_hostIP.Lines = GameNetworkUtilities.LocalIP.ToArray();
            txt_connectIP.Text = GameNetworkUtilities.LocalIPBase[GameNetworkUtilities.LocalIPBase.Count - 1];

            LobbyMembers = new ConcurrentQueue<string>();

            timer = new Timer
            {
                Interval = 100
            };
            timer.Tick += new EventHandler(checkLobbyMembers);
        }

        private void checkLobbyMembers(object sender, EventArgs e)
        {
            if (LobbyMembers.Count > 0)
            {
                for (int i = 0; i < LobbyMembers.Count; i++)
                {
                    LobbyMembers.TryDequeue(out string result);
                    lbx_lobbyPlayerList.Items.Add(result);
                }
            }
            //temp
            if (StartGame)
            {
                startGame();
            }
        }

        private void Txt_port_KeyPress(object sender, KeyPressEventArgs e)
        {
            handleIfNotNumber(e);
        }

        private void Txt_connectPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            handleIfNotNumber(e);
        }

        private static void handleIfNotNumber(KeyPressEventArgs e)
        {
            e.Handled = (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) || e.Handled;
        }

        private void Txt_connectIP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '.')
            {
                if ((sender as TextBox).Text.Count(c => c == '.') > 2)
                {
                    e.Handled = true;
                }
            }
            else if (!(char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void Btn_refresh_Click(object sender, EventArgs e)
        {
            refreshServerList();
        }

        private void refreshServerList()
        {
            enableConnectTabControls(false);

            lbx_connectList.Items.Clear();
            ips.Clear();

            lbl_connectStatus.Visible = true;
            lbl_connectStatus.Refresh();

            lastPortScanned = int.Parse(txt_connectPort.Text);
            Tuple<string, long, string>[] tuple = GameNetworkUtilities.getServersInNetwork(lastPortScanned);
            ips.AddRange(tuple);
            foreach (var (ip, ping, gameName) in tuple)
            {
                lbx_connectList.Items.Add($"{gameName}\t{ip}\t{ping}ms");
            }

            lbl_connectStatus.Visible = false;
            enableConnectTabControls(true);
        }

        private void enableConnectTabControls(bool enable)
        {
            btn_connect.Enabled = enable;
            btn_manualConnect.Enabled = enable;
            btn_refresh.Enabled = enable;
            lbx_connectList.Enabled = enable;
            txt_connectIP.Enabled = enable;
            txt_connectPort.Enabled = enable;
        }

        private void Btn_host_Click(object sender, EventArgs e)
        {
            connected();

            btn_lobbyStartGame.Enabled = true;

            connectedIP = null;
            connectedPort = int.Parse(txt_hostPort.Text);

            lbx_lobbyPlayerList.Items.Add("Local Player\t(HOST)");
            setLobbyGameName(txt_hostGameName.Text);

            lobbyServer = new LobbyServer(txt_hostGameName.Text, connectedPort, this);
            lobbyServer.startServer();

        }

        private void disableNewServerOptions()
        {
            btn_host.Enabled = false;
            btn_connect.Enabled = false;
            btn_manualConnect.Enabled = false;
        }

        private void TabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == 2 && !isConnectedToServer)
            {
                e.Cancel = true;
                MessageBox.Show("Please host or join a Lobby first.", "Can't Show Lobby", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Btn_connect_Click(object sender, EventArgs e)
        {
            if (lbx_connectList.SelectedIndex != -1)
            {
                connected();

                (string ip, _, string gameName) = ips[lbx_connectList.SelectedIndex];
                connectedIP = ip;
                connectedPort = lastPortScanned;
                lbx_lobbyPlayerList.Items.Add("Local Player\t(Local)");
                setLobbyGameName(gameName);
                lobbyClient = new LobbyClient(connectedIP, connectedPort, this);
                lobbyClient.connectToServer();
            }
        }

        private void setLobbyGameName(string gameName)
        {
            lbl_lobbyGameName.Text = "Game Name: " + gameName;
        }

        private void Btn_lobbyStartGame_Click(object sender, EventArgs e)
        {
            StartGame = true;
        }

        private void startGame()
        {
            GameForm gameForm;
            if (lobbyClient == null)
            {
                lobbyServer.terminateConnection();
                gameForm = new GameForm(connectedPort);
            }
            else
            {
                lobbyClient.terminateConnection();
                gameForm = new GameForm(connectedIP, connectedPort);
            }
            gameForm.FormClosed += (s, args) => Close();
            gameForm.Show();
            Hide();
            timer.Stop();
        }

        private void Btn_manualConnect_Click(object sender, EventArgs e)
        {
            connected();
            
            connectedIP = txt_connectIP.Text;
            connectedPort = int.Parse(txt_connectPort.Text);
            lbx_lobbyPlayerList.Items.Add("Local Player\t(Local)");
            lobbyClient = new LobbyClient(connectedIP, connectedPort, this);
            lobbyClient.connectToServer();
        }

        private void connected()//rename
        {
            switchToLobby();
            disableNewServerOptions();
            timer.Start();
        }

        private void switchToLobby()
        {
            isConnectedToServer = true;
            tabControl.SelectedTab = tabControl.TabPages[2];
        }
    }
}
