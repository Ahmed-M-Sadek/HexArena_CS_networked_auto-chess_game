using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.character.types;
using ASU2019_NetworkedGameWorkshop.model.spell;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.model.ui.shop
{
    class Shop
    {
        public static Spells selectedSpell;

        private static SpellShopPopUP skillShop;
        private static Button btn_hideSkillShop;
        private static Character selectedCharacter;
        public static SpellShopUIPanel spellShopView { get; private set; }

        private readonly Button btn_showSpells;
        private readonly Button btn_levelUp;
        private readonly Button btn_sellChar;
        private readonly FlowLayoutPanel mainButtonPanel;
        private readonly GameManager manager;

        public static ShopUIPanel SelectedCharacterView { get; private set; }

        public Shop(GameForm gameForm, GameManager manager)
        {
            this.manager = manager;
            SelectedCharacterView = new ShopUIPanel(gameForm, manager);
            skillShop = new SpellShopPopUP(gameForm, manager, this);
            spellShopView = new SpellShopUIPanel(gameForm);

            btn_hideSkillShop = new Button
            {
                Size = new Size(30, 30),
                Text = "X",
                Location = new Point(skillShop.Width - 35, 5)
            };

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

            SelectedCharacterView.Controls.Add(btn_sellChar);

            btn_showSpells.Size = new Size(btn_sellChar.Width / 2 - 20, btn_sellChar.Height);
            btn_levelUp.Size = new Size(btn_sellChar.Width / 2 - 20, btn_sellChar.Height);

            mainButtonPanel.Controls.Add(btn_showSpells);
            mainButtonPanel.Controls.Add(btn_levelUp);

            mainButtonPanel.Location = new Point(btn_sellChar.Location.X, btn_sellChar.Location.Y - btn_sellChar.Height - 20);
            mainButtonPanel.Size = new Size(btn_sellChar.Width, btn_sellChar.Height + 10);

            SelectedCharacterView.Controls.Add(mainButtonPanel);

            btn_showSpells.MouseClick += showSpells_click;
            btn_levelUp.MouseClick += levelUp_click;
            btn_levelUp.MouseHover += levelUp_hover;
            btn_levelUp.MouseLeave += levelUp_leave;
            btn_sellChar.MouseClick += sellChar_click;
            btn_hideSkillShop.MouseClick += hideShop_btn;

            skillShop.Controls.Add(btn_hideSkillShop);

            SelectedCharacterView.Visible = false;
            spellShopView.Visible = false;
        }

        private void levelUp_leave(object sender, EventArgs e)
        {
            SelectedCharacterView.HideChanges();
        }

        private void levelUp_hover(object sender, EventArgs e)
        {
            SelectedCharacterView.ShowStatsChanges();
        }

        private void sellChar_click(object sender, MouseEventArgs e)
        {
            manager.TeamBlue.Remove(selectedCharacter);
            selectedCharacter.CurrentTile.CurrentCharacter = null;

            manager.Player.Gold += 10;//todo change this plz
            manager.deselectSelectedTile();
        }

        private void levelUp_click(object sender, MouseEventArgs e)
        {
            if (manager.Player.Gold < selectedCharacter.CurrentLevel * 5 || manager.CurrentGameStage != StageManager.GameStage.Buy)
            {
                return;
            }
            manager.Player.Gold -= selectedCharacter.CurrentLevel * 5;
            selectedCharacter.levelUp();
            viewCharStats();
            if (!(selectedCharacter.CurrentLevel < CharacterType.MAX_CHAR_LVL - 1))
            {
                btn_levelUp.Enabled = false;
            }
            SelectedCharacterView.UpdateChanges();
        }

        private void showSpells_click(object sender, MouseEventArgs e)
        {
            SelectedCharacterView.Visible = false;
            spellShopView.ShowSpells(selectedCharacter);
        }

        private void hideShop_btn(object sender, MouseEventArgs e)
        {
            skillShop.Visible = false;
        }

        public void updateShop()
        {
            if (manager.SelectedTile != null)
            {
                selectedCharacter = manager.SelectedTile.CurrentCharacter;
                if (selectedCharacter != null)
                {
                    SelectedCharacterView.Visible = true;
                    viewCharStats();
                    SelectedCharacterView.Invalidate();
                    if (selectedCharacter.CurrentLevel < CharacterType.MAX_CHAR_LVL - 1)
                    {
                        btn_levelUp.Enabled = true;
                    }
                    else
                    {
                        btn_levelUp.Enabled = false;
                    }
                }
                else
                {
                    SelectedCharacterView.Visible = false; ;
                    skillShop.Visible = false;
                    spellShopView.Visible = false;
                }
            }
            else
            {
                SelectedCharacterView.Visible = false; ;
                skillShop.Visible = false;
                spellShopView.Visible = false;

            }
        }
        private void viewCharStats()
        {
            if (selectedCharacter != null)
            {
                SelectedCharacterView.ShowCharStats();
            }
        }
        public static void viewSkillShop()
        {
            skillShop.setParameters(selectedCharacter, selectedSpell);
            skillShop.Visible = true;
        }
        public static void HideShop()
        {
            spellShopView.Visible = false;
        }
    }
}
