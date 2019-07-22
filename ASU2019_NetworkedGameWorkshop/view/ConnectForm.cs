using System;
using System.Linq;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.view {
    public partial class ConnectForm : Form {
        public ConnectForm() {
            InitializeComponent();
        }

        private void ConnectForm_Load(object sender, EventArgs e) {
        }

        private void Txt_port_KeyPress(object sender, KeyPressEventArgs e) {
            handleIfNotNumber(e);
        }

        private void Txt_connectPort_KeyPress(object sender, KeyPressEventArgs e) {
            handleIfNotNumber(e);
        }

        private static void handleIfNotNumber(KeyPressEventArgs e) {
            e.Handled = (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) || e.Handled;
        }

        private void Txt_connectIP_KeyPress(object sender, KeyPressEventArgs e) {
            if(e.KeyChar == '.') {
                if((sender as TextBox).Text.Count(c => c == '.') > 2) {
                    e.Handled = true;
                }
            } else if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) {
                e.Handled = true;
            }
        }
    }
}
