using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.model.Shop {
    class ShopUIPanel:Panel {

        public ShopUIPanel(GameForm gameForm) {
            this.Size = new Size(270, 300);
            this.Location = new Point((int)(gameForm.Width * 0.78), (int)(gameForm.Height * 0.05));
            this.BackColor = Color.White;
            this.Visible = true;
            this.Padding = new Padding(10, 10, 10, 10);

            gameForm.Controls.Add(this);
        }


    }
}
