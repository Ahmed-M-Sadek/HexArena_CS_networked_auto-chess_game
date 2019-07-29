using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.controller.networking;
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

        private bool connectedToServer;
        private int lastPortScanned;

        public ConcurrentQueue<string> LobbyMembers { get; private set; }

        private Timer timer;

        public ConnectForm()
        {
            InitializeComponent();

            ips = new List<Tuple<string, long, string>>();
        }

        private void ConnectForm_Load(object sender, EventArgs e)
        {
            txt_connectPort.Text = txt_hostPort.Text = NetworkManager.DEFAULT_PORT.ToString();
            txt_hostIP.Lines = NetworkManager.LocalIP.ToArray();
            txt_connectIP.Text = NetworkManager.LocalIPBase[NetworkManager.LocalIPBase.Count - 1];

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
            Tuple<string, long, string>[] tuple = NetworkManager.getServersInNetwork(lastPortScanned);
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
            if (!connectedToServer)
            {
                connectedToServer = true;
                tabControl.SelectedTab = tabControl.TabPages[2];
                disableNewServerOptions();

                lbx_lobbyPlayerList.Items.Add("Local Player\t(HOST)");
                setLobbyGameName(txt_hostGameName.Text);

                new LobbyServer(txt_hostGameName.Text, int.Parse(txt_hostPort.Text), this).startServer();
                timer.Start();
            }
        }

        private void disableNewServerOptions()
        {
            btn_host.Enabled = false;
            btn_connect.Enabled = false;
            btn_manualConnect.Enabled = false;
        }

        private void TabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == 2 && !connectedToServer)
            {
                e.Cancel = true;
                MessageBox.Show("Please host or join a Lobby first.", "Can't Show Lobby", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Btn_connect_Click(object sender, EventArgs e)
        {
            if (lbx_connectList.SelectedIndex != -1)
            {
                connectedToServer = true;
                tabControl.SelectedTab = tabControl.TabPages[2];
                disableNewServerOptions();

                (string ip, _, string gameName) = ips[lbx_connectList.SelectedIndex];
                lbx_lobbyPlayerList.Items.Add("Local Player\t(Local)");
                setLobbyGameName(gameName);
                new LobbyClient(ip, lastPortScanned, this).connectToServer();
                timer.Start();
            }
        }

        private void setLobbyGameName(string gameName)
        {
            lbl_lobbyGameName.Text = "Game Name: " + txt_hostGameName.Text;
        }
    }
}
