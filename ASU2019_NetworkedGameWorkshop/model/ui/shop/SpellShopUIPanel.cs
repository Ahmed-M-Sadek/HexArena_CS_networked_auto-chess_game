using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.spell;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.model.ui.shop
{
    class SpellShopUIPanel : FlowLayoutPanel
    {
        private readonly Button btn_hide;
        private readonly Shop shop;

        public SpellShopUIPanel(GameForm gameForm, Shop shop)
        {
            this.shop = shop;

            Size = new Size(270, 300);
            Location = new Point((int)(gameForm.Width * 0.78), (int)(gameForm.Height * 0.05));
            BackColor = Color.Transparent;
            DoubleBuffered = true;
            Visible = true;
            Padding = new Padding(10, 10, 10, 10);
            FlowDirection = FlowDirection.TopDown;
            AutoScroll = true;

            btn_hide = new Button
            {
                Text = "Back"
            };
            btn_hide.MouseClick += hideBtn_click;

            gameForm.Controls.Add(this);
        }

        private void hideBtn_click(object sender, MouseEventArgs e)
        {
            Visible = false;
            shop.SelectedCharacterView.Visible = true;
        }

        public void ShowSpells(Character selectedChar)
        {
            Controls.Clear();
            Controls.Add(btn_hide);
            foreach (Spells[] spell in Spells.Values)
            {
                Label label = new Label
                {
                    AutoSize = true,
                    Font = new Font("Arial", 10, FontStyle.Bold),
                };
                if (selectedChar.LearnedSpells.Contains(spell))
                {
                    if (selectedChar.SpellLevel[spell] != spell.Length - 1)
                    {
                        label.Text = $"{spell[selectedChar.SpellLevel[spell]].Name}({selectedChar.SpellLevel[spell] + 1})";
                        label.ForeColor = Color.MediumSeaGreen;
                        label.MouseClick += (sender, e) => shop.viewSkillShop(spell, selectedChar.SpellLevel[spell] + 1);
                    }
                    else
                    {
                        label.Text = $"{spell[selectedChar.SpellLevel[spell]].Name}({selectedChar.SpellLevel[spell] + 1})-Max-";
                        label.ForeColor = Color.MediumVioletRed;
                    }
                }
                else
                {
                    label.Text = $"{spell[0].Name}(1)";
                    label.ForeColor = Color.OrangeRed;
                    label.MouseClick += (sender, e) => shop.viewSkillShop(spell, 0);
                }
                Controls.Add(label);
            }
            Visible = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawRectangle(Shop.BORDER_PEN, new Rectangle(Shop.BORDER_HALF_THICKNESS,
                                                                    Shop.BORDER_HALF_THICKNESS,
                                                                    ClientSize.Width - Shop.BORDER_THICKNESS,
                                                                    ClientSize.Height - Shop.BORDER_THICKNESS));
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Invalidate();
        }
    }
}
