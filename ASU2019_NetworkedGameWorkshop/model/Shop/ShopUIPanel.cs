using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.model.character;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.model.Shop {
    class ShopUIPanel:Panel {
        private readonly FlowLayoutPanel statLabels;
        private readonly GameManager manager;
        private readonly Label lbl_statsNames;

        private Label lbl_statsValues;

        public ShopUIPanel(GameForm gameForm,GameManager gameManager) {
            manager = gameManager;

            this.Size = new Size(270, 300);
            this.Location = new Point((int)(gameForm.Width * 0.78), (int)(gameForm.Height * 0.05));
            this.BackColor = Color.White;
            this.Visible = true;
            this.Padding = new Padding(10, 10, 10, 10);

            statLabels = new FlowLayoutPanel {
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Top
            };


            lbl_statsNames = new Label {
                Dock = DockStyle.Left,
                Text = "Character Class:\nHealth Points:\nRange:\nAtk Damage:\nAtk Speed:\nArmour:\nMagic Resist:",
                AutoSize = true
            };

            lbl_statsValues = new Label {
                Dock = DockStyle.Right
            };

            statLabels.Controls.Add(lbl_statsNames);
            statLabels.Controls.Add(lbl_statsValues);
            gameForm.Controls.Add(this);
        }
        private void GetCharStatsPanel() {
            if (manager.SelectedTile.CurrentCharacter != null) {
                Character selected = manager.SelectedTile.CurrentCharacter;

                lbl_statsValues.Text = $"{selected.CharacterType.Name}\n{selected.Stats[StatusType.HealthPointsMax]}\n{selected.Stats[StatusType.Range]}\n{selected.Stats[StatusType.AttackDamage]}\n{selected.Stats[StatusType.AttackSpeed]}\n{selected.Stats[StatusType.Armor]}\n{selected.Stats[StatusType.MagicResist]}";
                lbl_statsValues.AutoSize = true;
            }
        }
        public void ShowCharStats() {
            GetCharStatsPanel();
            this.Controls.Add(statLabels);
        }
    }
}
