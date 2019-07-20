using System;
using System.Drawing;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.model.Shop {
    public class Skill {
        String name;
        int price;
        private Label lbl_skill;
        public bool isUnlocked { get; set; }
        public Skill() : this("Default Skill", 500) {

        }

        public Skill(String skillName, int skillPrice) {
            name = skillName;
            price = skillPrice;
            isUnlocked = false;

            lbl_skill = new Label();
            lbl_skill.ForeColor = Color.Purple;
            lbl_skill.Text = Show();
            lbl_skill.MouseClick += skillLabel_clicked;
        }

        private void skillLabel_clicked(object sender, MouseEventArgs e) {
            Shop.viewSkillShop(this);
            Shop.selectedSkill = this;
        }

        internal string Show() {
            return name + " -> " + price;
        }

        public Label getLabel() {
            return lbl_skill;
        }

        public bool buySkill() {
            if (isUnlocked) {
                return false;
            } else {
                isUnlocked = true;
                return true;
            }
        }
    }
}
