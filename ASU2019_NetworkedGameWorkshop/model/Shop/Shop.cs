using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.character.types;
using ASU2019_NetworkedGameWorkshop.model.spell;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.model.Shop {
    class Shop {

        private SpellShopUIPanel spellShopView;
        private FlowLayoutPanel mainButtonPanel;
        private static Character selectedCharacter;
        private GameManager manager;

        private static SpellShopPopUP skillShop;
        private static Button btn_hideSkillShop;

        private readonly Button btn_showSpells;
        private readonly Button btn_levelUp;
        private readonly Button btn_sellChar;

        public static ShopUIPanel selectedCharacterView { get; private set; }
        public static Spells selectedSpell;

        public Shop(GameForm gameForm, GameManager manager) {
            this.manager = manager;
            selectedCharacterView = new ShopUIPanel(gameForm);
            skillShop = new SpellShopPopUP(gameForm);
            spellShopView = new SpellShopUIPanel(gameForm);

            btn_hideSkillShop = new Button();
            btn_hideSkillShop.Size = new Size(30, 30);
            btn_hideSkillShop.Text = "X";
            btn_hideSkillShop.Location = new Point(skillShop.Width - 35, 5);


            
            btn_showSpells = new Button();
            btn_levelUp = new Button();
            btn_sellChar = new Button();
            mainButtonPanel = new FlowLayoutPanel();

            btn_showSpells.Text = "Spells";
            btn_levelUp.Text = "Level UP";
            btn_sellChar.Text = "Sell";

            mainButtonPanel.Padding = new Padding(10, 5, 10, 0);
            btn_sellChar.Padding = new Padding(0, 0, 0, 5);
            btn_sellChar.Height = 50;

            btn_sellChar.Dock = DockStyle.Bottom;

            selectedCharacterView.Controls.Add(btn_sellChar);

            btn_showSpells.Size = new Size(btn_sellChar.Width / 2 - 20, btn_sellChar.Height);
            btn_levelUp.Size = new Size(btn_sellChar.Width / 2 - 20, btn_sellChar.Height);

            mainButtonPanel.Controls.Add(btn_showSpells);
            mainButtonPanel.Controls.Add(btn_levelUp);

            mainButtonPanel.Location = new Point(btn_sellChar.Location.X, btn_sellChar.Location.Y - btn_sellChar.Height - 20);
            mainButtonPanel.Size = new Size(btn_sellChar.Width, btn_sellChar.Height + 10);

            selectedCharacterView.Controls.Add(mainButtonPanel);

            btn_showSpells.MouseClick += showSpells_click;
            btn_levelUp.MouseClick += levelUp_click;
            btn_sellChar.MouseClick += sellChar_click;
            btn_hideSkillShop.MouseClick += hideShop_btn;

            skillShop.Controls.Add(btn_hideSkillShop);

            selectedCharacterView.Visible = false;

        }

        private void sellChar_click(object sender, MouseEventArgs e) {
            manager.TeamBlue.Remove(selectedCharacter);
            selectedCharacter.CurrentTile.CurrentCharacter = null;

            manager.Player.Gold += 10;//todo change this
        }

        private void levelUp_click(object sender, MouseEventArgs e) {
            if (manager.Player.Gold < selectedCharacter.CurrentLevel * 5 || manager.CurrentGameStage != controller.StageManager.GameStage.Buy) {
                return;
            }
            manager.Player.Gold -= selectedCharacter.CurrentLevel * 5;
            selectedCharacter.levelUp();
        }

        private void showSpells_click(object sender, MouseEventArgs e) {
            selectedCharacterView.Visible = false;
            spellShopView.ShowSpells(selectedCharacter);
        }

        private void hideShop_btn(object sender, MouseEventArgs e) {
            skillShop.Visible = false;
        }

        public void updateShop() {
            selectedCharacter = manager.SelectedTile.CurrentCharacter;
            if (selectedCharacter != null) {
                selectedCharacterView.Visible = true;
                selectedCharacterView.Invalidate();
            } else {
                selectedCharacterView.Visible = false; ;
                skillShop.Visible = false;
                spellShopView.Visible = false;
            }
        }
        private void viewNewSpells() {
            selectedCharacterView.Visible = false;
        }
        private void viewCharStats() {
            if (selectedCharacter != null) {

            }
        }
        public static void viewSkillShop() {
            skillShop.setParameters(selectedCharacter, selectedSpell);
            skillShop.Visible = true;
        }

    }
}
