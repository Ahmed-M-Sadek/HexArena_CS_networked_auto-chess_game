using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.model.character;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.model.Shop {
    class Shop {
        FlowLayoutPanel selectedCharacterView;
        private Character selectedCharacter;
        private static FlowLayoutPanel skillShop;
        private static Button btn_hideSkillShop;
        private static Button btn_buySkill;
        public static Skill selectedSkill;
        public Shop(GameForm gameForm) {
            selectedCharacterView = new FlowLayoutPanel();
            selectedCharacterView.Size = new Size(245, 215);
            selectedCharacterView.Location = new Point(12, 534);
            selectedCharacterView.BackColor = Color.White;
            selectedCharacterView.Visible = true;
            selectedCharacterView.FlowDirection = FlowDirection.TopDown;
            gameForm.Controls.Add(selectedCharacterView);

            skillShop = new FlowLayoutPanel();
            skillShop.Visible = false;
            skillShop.Location = new Point((gameForm.Width / 2) - (skillShop.Width / 2), (gameForm.Height / 2) - (skillShop.Height / 2));
            skillShop.BackColor = Color.Yellow;
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
            selectedSkill.buySkill();
            skillShop.Visible = false;
            selectedCharacterView.Invalidate();
        }

        public void updateShop(Character character) {

            selectedCharacter = character;
            showSkills();

        }

        public void showStats() {
            selectedCharacterView.Controls.Clear();
            foreach (SingleStat stat in selectedCharacter.charStats.list) {
                Label lbl_stat = new Label();
                lbl_stat.ForeColor = Color.Orange;
                lbl_stat.Text = stat.Show();
                selectedCharacterView.Controls.Add(lbl_stat);
                selectedCharacterView.Invalidate();
            }
        }
        public void showSkills() {
            selectedCharacterView.Controls.Clear();
            if (selectedCharacter != null) {
                foreach (Skill skill in selectedCharacter.allSkills) {

                    if (!skill.isUnlocked) {
                        selectedCharacterView.Controls.Add(skill.getLabel());
                    }
                }
            }
        }

        public static void viewSkillShop(Skill skill) {
            skillShop.Controls.Clear();

            skillShop.Controls.Add(btn_hideSkillShop);
            skillShop.Controls.Add(btn_buySkill);

            skillShop.Visible = true;
        }
        private void selectedCharacterView_Paint(object sender, PaintEventArgs e) {
            throw new NotImplementedException();
        }

    }
}
