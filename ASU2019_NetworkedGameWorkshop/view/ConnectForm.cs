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
        private readonly List<GameNetworkUtilities.ServerStats> ips;

        private bool isConnectedToServer;
        private int lastPortScanned;

        public ConcurrentQueue<string> LobbyMembers { get; private set; }
        public bool StartGame { get; set; }

        private Timer timer;
        private LobbyClient lobbyClient;
        private LobbyServer lobbyServer;
        private string connectedIP;
        private int connectedPort;
        private string playerName;

        public ConnectForm()
        {
            InitializeComponent();

            ips = new List<GameNetworkUtilities.ServerStats>();
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
            GameNetworkUtilities.ServerStats[] serversStats = GameNetworkUtilities.getServersInNetwork(lastPortScanned);
            ips.AddRange(serversStats);
            foreach (GameNetworkUtilities.ServerStats serverStats in serversStats)
            {
                lbx_connectList.Items.Add($"{serverStats.GameName}\t{serverStats.HostPlayerName}({serverStats.Ip})\t{serverStats.Ping}ms");
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

            playerName = txt_hostPlayerName.Text;
            lbx_lobbyPlayerList.Items.Add($"{playerName}\t\t(HOST)");
            setLobbyGameName(txt_hostGameName.Text);

            lobbyServer = new LobbyServer(txt_hostGameName.Text, connectedPort, this, playerName);
            lobbyServer.startServer();

        }

        private void disableNewServerOptions()
        {
            btn_host.Enabled = false;
            btn_connect.Enabled = false;
            btn_manualConnect.Enabled = false;
            btn_refresh.Enabled = false;
            txt_connectPlayerName.Enabled = false;
            txt_hostPlayerName.Enabled = false;
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

                GameNetworkUtilities.ServerStats serverStats = ips[lbx_connectList.SelectedIndex];
                connectedIP = serverStats.Ip;
                connectedPort = lastPortScanned;
                playerName = txt_connectPlayerName.Text;
                lbx_lobbyPlayerList.Items.Add($"{playerName}\t\t(Local)");
                setLobbyGameName(serverStats.GameName);
                lobbyClient = new LobbyClient(connectedIP, connectedPort, this, playerName);
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
                gameForm = new GameForm(playerName, connectedPort);
            }
            else
            {
                lobbyClient.terminateConnection();
                gameForm = new GameForm(playerName, connectedIP, connectedPort);
            }
            gameForm.FormClosed += (s, args) => Close();
            gameForm.Show();
            Hide();
            timer.Stop();
        }

        private void Btn_manualConnect_Click(object sender, EventArgs e)
        {

            connectedIP = txt_connectIP.Text;
            connectedPort = int.Parse(txt_connectPort.Text);
            GameNetworkUtilities.ServerStats serverStats = GameNetworkUtilities.pingIP(connectedIP, connectedPort);
            if (serverStats.Equals(GameNetworkUtilities.INVALID_SERVER))
            {
                MessageBox.Show($"Could NOT Ping {connectedIP}:{connectedPort}",
                "Could NOT Ping",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            }
            else
            {
                connected();
                playerName = txt_connectPlayerName.Text;
                lbx_lobbyPlayerList.Items.Add($"{playerName}\t\t(Local)");
                setLobbyGameName(serverStats.GameName);
                lobbyClient = new LobbyClient(connectedIP, connectedPort, this, playerName);
                lobbyClient.connectToServer();
            }

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

        private void Txt_hostPlayerName_TextChanged(object sender, EventArgs e)
        {
            txt_connectPlayerName.Text = txt_hostPlayerName.Text;
        }

        private void Txt_connectPlayerName_TextChanged(object sender, EventArgs e)
        {
            txt_hostPlayerName.Text = txt_connectPlayerName.Text;
        }
    }
}
