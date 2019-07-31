using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.spell;
using System.Drawing;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.model.ui.shop
{
    class SpellShopUIPanel : FlowLayoutPanel
    {
        private readonly Button btn_hide;
        public SpellShopUIPanel(GameForm gameForm)
        {
            Size = new Size(270, 300);
            Location = new Point((int)(gameForm.Width * 0.78), (int)(gameForm.Height * 0.05));
            BackColor = Color.White;
            Visible = true;
            Padding = new Padding(10, 10, 10, 10);
            FlowDirection = FlowDirection.TopDown;
            AutoScroll = true;

            btn_hide = new Button();
            btn_hide.Size = new Size(20, 20);
            btn_hide.Text = "X";
            btn_hide.MouseClick += hideBtn_click;

            gameForm.Controls.Add(this);
        }

        private void hideBtn_click(object sender, MouseEventArgs e)
        {
            Visible = false;
            Shop.SelectedCharacterView.Visible = true;
        }

        public void ShowSpells(Character selectedChar)
        {
            Controls.Clear();
            Controls.Add(btn_hide);
            foreach (Spells spell in Spells.Values)
            {
                if (selectedChar.spells.Contains(spell))
                {
                    spell.lbl_spell.ForeColor = Color.MediumSeaGreen;
                }
                else
                {
                    spell.lbl_spell.ForeColor = Color.OrangeRed;
                }
                Controls.Add(spell.lbl_spell);
            }
            Visible = true;
        }
    }
}
