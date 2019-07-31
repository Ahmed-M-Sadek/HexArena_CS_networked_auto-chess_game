using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.spell;
using System.Drawing;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.model.ui.shop
{
    class SpellShopPopUP : Panel
    {
        private readonly Button btn_upgradeSkill;
        private readonly Button btn_buySkill;
        private readonly GameManager manager;
        private readonly Shop shop;
        private readonly Label lbl_spellStats;

        private bool isNewSpell;
        private Character character;
        private Spells spell;

        public SpellShopPopUP(GameForm gameForm, GameManager gameManager, Shop shop)
        {
            this.shop = shop;
            lbl_spellStats = new Label
            {
                Dock = DockStyle.Top
            };
            manager = gameManager;

            Size = new Size(270, 300);
            Location = new Point((int)(gameForm.Width * 0.55), (int)(gameForm.Height * 0.09));
            Visible = false;
            Padding = new Padding(10, 10, 10, 10);
            BorderStyle = BorderStyle.FixedSingle;

            BackColor = Color.FromArgb(100, 255, 255, 255);
            DoubleBuffered = true;

            btn_upgradeSkill = new Button
            {
                Height = 35,
                Text = "Upgrade spell",
                Dock = DockStyle.Bottom
            };
            btn_upgradeSkill.MouseClick += upgradeSpell_click;
            btn_upgradeSkill.Visible = false;

            btn_buySkill = new Button
            {
                Height = 35,
                Text = "Purchase spell",
                Dock = DockStyle.Bottom
            };
            btn_buySkill.Visible = false;
            btn_buySkill.MouseClick += buySkill_click;

            Controls.Add(btn_buySkill);
            Controls.Add(btn_upgradeSkill);
            gameForm.Controls.Add(this);
        }

        private void buySkill_click(object sender, MouseEventArgs e)
        {
            if (isNewSpell)
            {
                character.learnSpell(spell);
                Visible = false;
                shop.spellShopView.ShowSpells(manager.SelectedTile.CurrentCharacter);
            }
        }

        private void upgradeSpell_click(object sender, MouseEventArgs e)
        {
            if (!isNewSpell)
            {
                Visible = false;
            }
        }

        public void showSpellStats(Spells spell)
        {
            lbl_spellStats.Text = spell.ToString();
            lbl_spellStats.AutoSize = true;
            lbl_spellStats.BackColor = Color.Transparent;

            Controls.Add(lbl_spellStats);
        }

        public void setParameters(Character selectedCharacter, Spells selectedSpell)
        {
            character = selectedCharacter;
            spell = selectedSpell;
            isNewSpell = !character.spells.Contains(spell);
            setButton();
        }

        public void setButton()
        {
            showSpellStats(spell);
            btn_upgradeSkill.Visible = !isNewSpell;
            btn_buySkill.Visible = isNewSpell;
        }
    }
}
