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

        private Label lbl_spellStats;
        private bool isNewSpell;

        private Character character;
        private Spells spell;
        public SpellShopPopUP(GameForm gameForm, GameManager gameManager, Shop shop)
        {
            this.shop = shop;
            lbl_spellStats = new Label();
            lbl_spellStats.Dock = DockStyle.Top;
            manager = gameManager;

            Size = new Size(270, 300);
            Location = new Point((int)(gameForm.Width * 0.55), (int)(gameForm.Height * 0.09));
            BackColor = Color.White;
            Visible = false;
            Padding = new Padding(10, 10, 10, 10);
            BorderStyle = BorderStyle.FixedSingle;

            BackColor = Color.Gold;

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

            Controls.Add(lbl_spellStats);
        }
        public void setParameters(Character selectedCharacter, Spells selectedSpell)
        {
            character = selectedCharacter;
            spell = selectedSpell;
            if (character.spells.Contains(spell))
            {
                isNewSpell = false;
            }
            else
            {
                isNewSpell = true;
            }
            setButton();
        }
        public void setButton()
        {
            showSpellStats(spell);
            if (isNewSpell)
            {
                btn_upgradeSkill.Visible = false;
                btn_buySkill.Visible = true;
            }
            else
            {
                btn_buySkill.Visible = false;
                btn_upgradeSkill.Visible = true;
            }

        }
    }
}
