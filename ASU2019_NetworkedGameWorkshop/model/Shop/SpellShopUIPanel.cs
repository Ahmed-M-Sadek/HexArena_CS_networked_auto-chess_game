using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.spell;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.model.Shop {
    class SpellShopUIPanel:FlowLayoutPanel {
        private readonly Button btn_hide;
        public SpellShopUIPanel(GameForm gameForm) {
            this.Size = new Size(270, 300);
            this.Location = new Point((int)(gameForm.Width * 0.78), (int)(gameForm.Height * 0.05));
            this.BackColor = Color.White;
            this.Visible = true;
            this.Padding = new Padding(10, 10, 10, 10);
            this.FlowDirection = FlowDirection.TopDown;

            btn_hide = new Button();
            btn_hide.Size = new Size(20, 20);
            btn_hide.Text = "X";
            btn_hide.MouseClick += hideBtn_click;

            gameForm.Controls.Add(this);
        }

        private void hideBtn_click(object sender, MouseEventArgs e) {
            this.Visible = false;
            Shop.selectedCharacterView.Visible = true;
        }

        public void ShowSpells(Character selectedChar) {
            this.Controls.Clear();
            this.Controls.Add(btn_hide);
            foreach (Spells spell in Spells.Values) {
                if (selectedChar.spells.Contains(spell)) {
                    spell.lbl_spell.ForeColor = Color.BlueViolet;
                } else {
                    spell.lbl_spell.ForeColor = Color.OrangeRed;
                }
                this.Controls.Add(spell.lbl_spell);
            }
            this.Visible = true;
        }
    }
}
