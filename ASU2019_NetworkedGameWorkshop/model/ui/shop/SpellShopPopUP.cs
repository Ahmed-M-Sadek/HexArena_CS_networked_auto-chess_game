using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.controller.networking;
using ASU2019_NetworkedGameWorkshop.controller.networking.game;
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
        private readonly GameNetworkManager gameNetworkManager;
        private readonly Label lbl_spellStats;
        private const int SPELL_PRICE = 10;

        private bool isNewSpell;
        private int spellNewLevel;
        private Character character;
        private Spells[] spell;


        public SpellShopPopUP(GameForm gameForm, GameManager gameManager, Shop shop, GameNetworkManager gameNetworkManager)
        {
            this.shop = shop;
            this.gameNetworkManager = gameNetworkManager;
            lbl_spellStats = new Label
            {
                Dock = DockStyle.Top
            };
            manager = gameManager;

            Size = new Size(270, 300);
            Location = new Point((int)(gameForm.Width * 0.55), (int)(gameForm.Height * 0.09));
            Visible = false;
            Padding = new Padding(10, 10, 10, 10);
            BorderStyle = BorderStyle.FixedSingle;

            BackColor = Color.FromArgb(100, 255, 255, 255);
            DoubleBuffered = true;

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
            if (manager.Player.Gold < SPELL_PRICE)
            {
                return;
            }
            SoundManager.PlaySound("learnSpell.wav");
            manager.Player.Gold -= SPELL_PRICE;
            character.learnSpell(spell);
            Visible = false;
            gameNetworkManager.enqueueMsg(NetworkMsgPrefix.LearnSpell, GameNetworkUtilities.serializeSpellAction(spell, character.CurrentTile));
            shop.SpellShopView.ShowSpells(manager.SelectedTile.CurrentCharacter);
            character.ChooseSpell.refreshPanel(character, character.ActiveSpells);
            character.InactiveSpell.refreshPanel(character.InactiveSpells);
        }

        private void upgradeSpell_click(object sender, MouseEventArgs e)
        {
            character.upgradeSpell(spell);
            Visible = false;
            gameNetworkManager.enqueueMsg(NetworkMsgPrefix.LevelUpSpell, GameNetworkUtilities.serializeSpellAction(spell, character.CurrentTile));
            shop.SpellShopView.ShowSpells(manager.SelectedTile.CurrentCharacter);
            if (!isNewSpell)
            {
                SoundManager.PlaySound("upgradeSpell.wav");
                Visible = false;
            }
        }

        public void showSpellStats(Spells[] spell)
        {
            lbl_spellStats.Text = spell[spellNewLevel].ToString();
            lbl_spellStats.AutoSize = true;
            lbl_spellStats.BackColor = Color.Transparent;

            Controls.Add(lbl_spellStats);
        }

        public void setParameters(Character selectedCharacter, Spells[] selectedSpell, int spellNewLevel)
        {
            character = selectedCharacter;
            spell = selectedSpell;
            isNewSpell = spellNewLevel == 0;
            this.spellNewLevel = spellNewLevel;
            setButton();
        }

        public void setButton()
        {
            showSpellStats(spell);
            btn_upgradeSkill.Visible = !isNewSpell;
            btn_buySkill.Visible = isNewSpell;
        }
    }
}
