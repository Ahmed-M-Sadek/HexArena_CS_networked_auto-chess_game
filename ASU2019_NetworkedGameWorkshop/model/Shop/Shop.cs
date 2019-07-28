using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.spell;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.model.Shop {
    class Shop {

        private FlowLayoutPanel selectedCharacterView;
        private Character selectedCharacter;
        private GameManager manager;
        private CharacterShop characterShop;

        private static FlowLayoutPanel skillShop;
        private static Button btn_hideSkillShop;
        private static Button btn_buySkill;

        public static Spell selectedSpell;

        public Shop(GameManager manager, GameForm gameForm) {

            this.manager = manager;
            characterShop = new CharacterShop(gameForm);

            selectedCharacterView = new FlowLayoutPanel();
            selectedCharacterView.Size = new Size(245, 215);
            selectedCharacterView.Location = new Point(12, 534);
            selectedCharacterView.BackColor = Color.White;
            selectedCharacterView.Visible = true;
            selectedCharacterView.FlowDirection = FlowDirection.TopDown;
            gameForm.Controls.Add(selectedCharacterView);

            skillShop = new FlowLayoutPanel();
            skillShop.Visible = false;
            skillShop.Location = new Point(20, (gameForm.Height / 2) - (skillShop.Height / 2));
            skillShop.BackColor = Color.Gold;
            skillShop.FlowDirection = FlowDirection.TopDown;

            btn_hideSkillShop = new Button();
            btn_hideSkillShop.Size = new Size(20, 20);
            btn_hideSkillShop.Text = "X";
            btn_hideSkillShop.Dock = DockStyle.Right;
            btn_hideSkillShop.MouseClick += hideShop_btn;

            btn_buySkill = new Button();
            btn_buySkill.Text = "Purchase";
            btn_buySkill.MouseClick += buySkill_click;

            gameForm.Controls.Add(skillShop);
        }


        private void hideShop_btn(object sender, MouseEventArgs e) {
            skillShop.Visible = false;
        }

        private void buySkill_click(object sender, MouseEventArgs e) {
            if (!selectedCharacter.spells.Contains(selectedSpell)) {

                selectedCharacter.learnSpell(selectedSpell);

                skillShop.Visible = false;
                selectedCharacterView.Controls.Remove(selectedSpell.lbl_spell);
                viewNewSpells();

                printLearnedSpells();
            }

        }

        public void updateShop() {
            selectedCharacter = manager.selectedTile.CurrentCharacter;
            viewNewSpells();
            selectedCharacterView.Invalidate();
        }
        private void viewNewSpells() {
            selectedCharacterView.Controls.Clear();
            if (selectedCharacter != null) {
                foreach (Spell spell in Spell.Values) {
                    if (!(selectedCharacter.spells.Contains(spell))) {
                        selectedCharacterView.Controls.Add(spell.lbl_spell);
                    }
                }
            }
        }
        public static void viewSkillShop() {
            skillShop.Controls.Clear();

            skillShop.Controls.Add(btn_hideSkillShop);
            skillShop.Controls.Add(btn_buySkill);

            skillShop.Visible = true;
        }
        private void printLearnedSpells() {
            foreach (Spell spell in selectedCharacter.spells) {
                Console.WriteLine(spell.name);
            }
        }
    }
}
