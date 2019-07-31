using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.character.types;
using ASU2019_NetworkedGameWorkshop.model.spell;
using System.Drawing;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.model.ui.shop
{
    class Shop
    {
        public const int BORDER_THICKNESS = 1,
                         BORDER_HALF_THICKNESS = BORDER_THICKNESS / 2;

        public static readonly Pen BORDER_PEN = new Pen(Color.Black, BORDER_THICKNESS);

        private static SpellShopPopUP skillShop;
        private static Button btn_hideSkillShop;
        private static Character selectedCharacter;

        private readonly Button btn_showSpells;
        private readonly Button btn_levelUp;
        private readonly Button btn_sellChar;
        private readonly FlowLayoutPanel mainButtonPanel;
        private readonly GameManager gameManager;

        public SpellShopUIPanel spellShopView { get; private set; }
        public ShopUIPanel SelectedCharacterView { get; private set; }

        public Spells SelectedSpell { get; set; }

        public Shop(GameForm gameForm, GameManager gameManager)
        {
            this.gameManager = gameManager;
            SelectedCharacterView = new ShopUIPanel(gameForm, gameManager)
            {
                Visible = false
            };
            skillShop = new SpellShopPopUP(gameForm, gameManager, this);
            spellShopView = new SpellShopUIPanel(gameForm, this)
            {
                Visible = false
            };

            btn_sellChar = new Button
            {
                Text = "Sell",
                Padding = new Padding(0, 0, 0, 5),
                Dock = DockStyle.Bottom,
                Height = 50
            };
            btn_sellChar.MouseClick += sellChar_click;
            SelectedCharacterView.Controls.Add(btn_sellChar);

            mainButtonPanel = new FlowLayoutPanel
            {
                Padding = new Padding(10, 5, 10, 0),
                Location = new Point(btn_sellChar.Location.X, btn_sellChar.Location.Y - btn_sellChar.Height - 20),
                Size = new Size(btn_sellChar.Width, btn_sellChar.Height + 10)
            };
            SelectedCharacterView.Controls.Add(mainButtonPanel);


            btn_showSpells = new Button
            {
                Text = "Spells",
                Size = new Size(btn_sellChar.Width / 2 - 20, btn_sellChar.Height)
            };
            btn_showSpells.MouseClick += showSpells_click;
            mainButtonPanel.Controls.Add(btn_showSpells);

            btn_levelUp = new Button
            {
                Text = "Level UP",
                Size = new Size(btn_sellChar.Width / 2 - 20, btn_sellChar.Height)
            };
            btn_levelUp.MouseClick += levelUp_click;
            btn_levelUp.MouseHover += (sender, e) => SelectedCharacterView.ShowStatsChanges();
            btn_levelUp.MouseLeave += (sender, e) => SelectedCharacterView.HideChanges();
            mainButtonPanel.Controls.Add(btn_levelUp);


            btn_hideSkillShop = new Button
            {
                Text = "Back",
                Location = new Point(skillShop.Width - 90, 5)
            };
            btn_hideSkillShop.MouseClick += (sender, e) => skillShop.Visible = false;
            skillShop.Controls.Add(btn_hideSkillShop);

        }

        private void sellChar_click(object sender, MouseEventArgs e)
        {
            gameManager.TeamBlue.Remove(selectedCharacter);
            selectedCharacter.CurrentTile.CurrentCharacter = null;

            gameManager.Player.Gold += 10;//todo change this plz
            gameManager.deselectSelectedTile();
        }

        private void levelUp_click(object sender, MouseEventArgs e)
        {
            if (gameManager.Player.Gold < selectedCharacter.CurrentLevel * 5 || gameManager.CurrentGameStage != StageManager.GameStage.Buy)
            {
                return;
            }
            gameManager.Player.Gold -= selectedCharacter.CurrentLevel * 5;
            selectedCharacter.levelUp();
            viewCharStats();
            if (!(selectedCharacter.CurrentLevel < CharacterType.MAX_CHAR_LVL - 1))
            {
                btn_levelUp.Enabled = false;
                btn_levelUp.Text = "Max level";
            }
            else
            {
                btn_levelUp.Text = "Level UP";
            }
            SelectedCharacterView.UpdateChanges();
        }

        private void showSpells_click(object sender, MouseEventArgs e)
        {
            SelectedCharacterView.Visible = false;
            spellShopView.ShowSpells(selectedCharacter);
        }

        public void updateShop()
        {
            if (gameManager.SelectedTile != null)
            {
                selectedCharacter = gameManager.SelectedTile.CurrentCharacter;
                if (selectedCharacter != null)
                {
                    SelectedCharacterView.Visible = true;
                    viewCharStats();
                    SelectedCharacterView.Invalidate();
                    btn_levelUp.Enabled = selectedCharacter.CurrentLevel < CharacterType.MAX_CHAR_LVL - 1;
                }
                else
                {
                    hideShops();
                }
            }
            else
            {
                hideShops();
            }
        }

        private void hideShops()
        {
            SelectedCharacterView.Visible = false;
            skillShop.Visible = false;
            spellShopView.Visible = false;
        }

        private void viewCharStats()
        {
            if (selectedCharacter != null)
            {
                SelectedCharacterView.ShowCharStats();
            }
        }

        public void viewSkillShop()
        {
            skillShop.setParameters(selectedCharacter, SelectedSpell);
            skillShop.Visible = true;
        }
    }
}
