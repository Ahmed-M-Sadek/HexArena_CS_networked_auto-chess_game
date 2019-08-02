using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.grid;
using ASU2019_NetworkedGameWorkshop.model.spell;
using ASU2019_NetworkedGameWorkshop.controller;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.model
{
    public class ChooseSpell : Panel
    {
        private const float WIDTH = Tile.WIDTH - 30 * 2,
            WIDTH_HALF = WIDTH / 2f,
            height = 8;
        private const float HEX_OFFSET_Y = Tile.HEIGHT * 0.2f;
        private const int BACK_PADDING_H = 10;
        private const float BACK_OFFSET_X = WIDTH_HALF + BACK_PADDING_H / 2f,
            BACK_OFFSET_Y = 2f;
        private const float BACK_WIDTH = WIDTH + BACK_PADDING_H,
            BACK_HEIGHT = height + BACK_OFFSET_Y + 12;
        private const int IMAGE_SIZE = 18;
        private const int IMAGE_PADDING_Y = 2,
                          IMAGE_PADDING_X = 2;
        private Character character;
        private readonly float offsetY;
        private readonly float backOffsetY;
        public ChooseSpell(Character character, List<Spells[]> spells)
        {
            this.character = character;
            BackColor = Color.White;
            offsetY = -Tile.HALF_HEIGHT - 1 * BACK_HEIGHT + HEX_OFFSET_Y;
            backOffsetY = offsetY - (BACK_OFFSET_Y / 2f);
            Size = new Size((int)BACK_WIDTH, (int)BACK_HEIGHT);
            Location = new Point((int)(character.CurrentTile.centerX - BACK_OFFSET_X), (int)(character.CurrentTile.centerY + backOffsetY));
            refreshPanel(character,spells);


        }
        public void refreshPanel(Character character,List<Spells[]> spells) 
        {
            if(spells == null)
            {
                return;
            }
            Controls.Clear();
            refreshLocation(character);
            for (int i = 0; i < spells.Count; i++)
            {
                Spells[] currentSpell = spells[i];
                PictureBox pics = new PictureBox
                {
                    Image = currentSpell[character.SpellLevel[currentSpell]].Image,
                    Size = new Size(IMAGE_SIZE, IMAGE_SIZE),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Location = new Point(i * IMAGE_SIZE + IMAGE_PADDING_X * (i + 1), IMAGE_PADDING_Y)
                };
                pics.MouseClick += new MouseEventHandler(mouseEvent(spells, i));

                Controls.Add(pics);
            }
        }
        public void refreshLocation(Character character)
        {
            Location = new Point((int)(character.CurrentTile.centerX - BACK_OFFSET_X), (int)(character.CurrentTile.centerY + backOffsetY));
        }

        private MouseEventHandler mouseEvent(List<Spells[]> actives, int k)
        {
            return (sender, e) =>
            {
                Spells[] currentSpell = actives[k];
                if(character.gameManager.CurrentGameStage == StageManager.GameStage.Buy && e.Button == MouseButtons.Right)
                {
                    character.InactiveSpells.Add(currentSpell);
                    character.ActiveSpells.Remove(currentSpell);
                    refreshPanel(character,character.ActiveSpells);
                    character.InactiveSpell.refreshPanel(character.InactiveSpells);
                }
                if(e.Button == MouseButtons.Left)
                {
                    if(character.gameManager.CurrentGameStage == StageManager.GameStage.Fight && character.Stats[StatusType.Charge] == character.Stats[StatusType.ChargeMax])
                    {
                        character.gameManager.removeRangeFromForm(this);
                        character.SpellReady = false;
                        character.resetMana();
                        currentSpell[character.SpellLevel[currentSpell]].castSpell(character);
                    }
                    if( character.Stats[StatusType.Charge]/character.Stats[StatusType.ChargeMax] < 0.9)
                    {
                        character.DefaultSkill = currentSpell;
                        Spells[] temp = character.ActiveSpells[0];
                        character.ActiveSpells[0] = character.ActiveSpells[k];
                        character.ActiveSpells[k] = temp;
                        if (character.gameManager.CurrentGameStage == StageManager.GameStage.Fight && e.Button == MouseButtons.Left)
                            character.hideSpellUI();
                        refreshPanel(character,character.ActiveSpells);
                    }
                }
            }
            ;
        }
    }
}