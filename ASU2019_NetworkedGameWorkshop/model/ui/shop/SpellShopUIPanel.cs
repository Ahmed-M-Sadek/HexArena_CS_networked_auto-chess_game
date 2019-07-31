using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.spell;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.model.ui.shop
{
    class SpellShopUIPanel : FlowLayoutPanel
    {
        private readonly Button btn_hide;
        private readonly Dictionary<Spells, Label> spellLabels;
        private readonly Shop shop;

        public SpellShopUIPanel(GameForm gameForm, Shop shop)
        {
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

            spellLabels = new Dictionary<Spells, Label>();
            foreach (Spells spell in Spells.Values)
            {
                Label label = new Label
                {
                    Text = spell.Name,
                    AutoSize = true,
                    Font = new Font("Arial", 10, FontStyle.Bold)
                };
                label.MouseClick += (sender, e) =>
                {
                    shop.SelectedSpell = spell;
                    shop.viewSkillShop();
                };
                spellLabels.Add(spell, label);
            }

            this.shop = shop;
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
            foreach (Spells spell in Spells.Values)
            {
                spellLabels[spell].ForeColor = selectedChar.spells.Contains(spell) ? Color.MediumSeaGreen : Color.OrangeRed;
                Controls.Add(spellLabels[spell]);
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
