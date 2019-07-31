using ASU2019_NetworkedGameWorkshop.controller;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.view
{
    public partial class ConnectForm : Form
    {
        public ConnectForm()
        {
            InitializeComponent();
        }

        private void ConnectForm_Load(object sender, EventArgs e)
        {
            txt_connectPort.Text = txt_hostPort.Text = NetworkManager.DEFAULT_PORT.ToString();
            txt_hostIP.Lines = NetworkManager.LocalIP.ToArray();
            txt_connectIP.Text = NetworkManager.LocalIPBase[NetworkManager.LocalIPBase.Count - 1];
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
            lbl_connectStatus.Visible = true;
            lbl_connectStatus.Refresh();
            foreach (var (ip, ping) in NetworkManager.getServersInNetwork(int.Parse(txt_connectPort.Text)))
            {
                lbx_connectList.Items.Add(string.Format("{0}\t({1}ms)", ip, ping));
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
    }
}
