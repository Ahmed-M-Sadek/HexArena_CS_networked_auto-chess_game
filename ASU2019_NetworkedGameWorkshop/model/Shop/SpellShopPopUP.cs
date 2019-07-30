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
    class SpellShopPopUP:Panel {

        private readonly Button btn_upgradeSkill;
        private readonly Button btn_buySkill;

        private Character character;
        private Spells spell;
        public SpellShopPopUP(GameForm gameForm) {
            this.Size = new Size(270, 300);
            this.Location = new Point((int)(gameForm.Width * 0.55), (int)(gameForm.Height * 0.09));
            this.BackColor = Color.White;
            this.Visible = false;
            this.Padding = new Padding(10, 10, 10, 10);
            this.BorderStyle = BorderStyle.FixedSingle;

            this.BackColor = Color.Gold;

            btn_upgradeSkill = new Button();
            btn_upgradeSkill.Height = 35;
            btn_upgradeSkill.Text = "upgrade spell";
            btn_upgradeSkill.Dock = DockStyle.Bottom;
            btn_upgradeSkill.MouseClick += upgradeSpell_click;

            btn_buySkill = new Button();
            btn_buySkill.Height = 35;
            btn_buySkill.Text = "Purchase";
            btn_buySkill.Dock = DockStyle.Bottom;
            btn_buySkill.MouseClick += buySkill_click;


            gameForm.Controls.Add(this);
        }

        private void buySkill_click(object sender, MouseEventArgs e) {
            if (!character.spells.Contains(spell)) {
                character.learnSpell(spell);
            }
        }

        private void upgradeSpell_click(object sender, MouseEventArgs e) {
            throw new NotImplementedException();
        }

        public void showSpellStats(Spells spell,bool isNew) {
            Label lbl_spellStats = new Label();

            lbl_spellStats.Text = spell.ToString();

            this.Controls.Add(lbl_spellStats);
        }
        public void setParameters(Character selectedCharacter,Spells selectedSpell) {
            character = selectedCharacter;
            spell = selectedSpell;
        }
    }
}
