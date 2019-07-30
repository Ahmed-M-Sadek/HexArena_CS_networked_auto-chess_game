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
    class SpellShopPopUP : Panel {

        private readonly Button btn_upgradeSkill;
        private readonly Button btn_buySkill;
        private readonly GameManager manager;

        private Label lbl_spellStats;
        private bool isNewSpell;

        private Character character;
        private Spells spell;
        public SpellShopPopUP(GameForm gameForm,GameManager gameManager) {
            lbl_spellStats = new Label();
            lbl_spellStats.Dock = DockStyle.Top;
            manager = gameManager;

            this.Size = new Size(270, 300);
            this.Location = new Point((int)(gameForm.Width * 0.55), (int)(gameForm.Height * 0.09));
            this.BackColor = Color.White;
            this.Visible = false;
            this.Padding = new Padding(10, 10, 10, 10);
            this.BorderStyle = BorderStyle.FixedSingle;

            this.BackColor = Color.Gold;

            btn_upgradeSkill = new Button {
                Height = 35,
                Text = "Upgrade spell",
                Dock = DockStyle.Bottom
            };
            btn_upgradeSkill.MouseClick += upgradeSpell_click;
            btn_upgradeSkill.Visible = false;

            btn_buySkill = new Button {
                Height = 35,
                Text = "Purchase spell",
                Dock = DockStyle.Bottom
            };
            btn_buySkill.Visible = false;
            btn_buySkill.MouseClick += buySkill_click;

            this.Controls.Add(btn_buySkill);
            this.Controls.Add(btn_upgradeSkill);
            gameForm.Controls.Add(this);
        }

        private void buySkill_click(object sender, MouseEventArgs e) {
            if (isNewSpell) {
                character.learnSpell(spell);
                Shop.HideShop();
                this.Visible = false;
                manager.deselectSelectedTile();
            }
        }

        private void upgradeSpell_click(object sender, MouseEventArgs e) {
            if (!isNewSpell) {
                this.Visible = false;
                Shop.HideShop();
                manager.deselectSelectedTile();
            }
        }

        public void showSpellStats(Spells spell) {
            lbl_spellStats.Text = spell.ToString();
            lbl_spellStats.AutoSize = true;

            this.Controls.Add(lbl_spellStats);
        }
        public void setParameters(Character selectedCharacter, Spells selectedSpell) {
            character = selectedCharacter;
            spell = selectedSpell;
            if (character.spells.Contains(spell)) {
                this.isNewSpell = false;
            } else {
                isNewSpell = true;
            }
            setButton();
        }
        public void setButton() {
            showSpellStats(spell);
            if (isNewSpell) {
                btn_upgradeSkill.Visible = false;
                btn_buySkill.Visible = true;
            } else {
                btn_buySkill.Visible = false;
                btn_upgradeSkill.Visible = true;
            }

        }
    }
}
