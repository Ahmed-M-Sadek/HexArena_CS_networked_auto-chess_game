using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.grid;
using ASU2019_NetworkedGameWorkshop.model.spell;
using System.Collections.Generic;
using System.Windows.Forms;
using System;
using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model
{
    public class ChooseSpell : Panel
    {
        private const float WIDTH = Tile.WIDTH - 30 * 2,
            WIDTH_HALF = WIDTH / 2f,
            height = 8;
        private const float HEX_OFFSET_Y = Tile.HEIGHT * 0.2f;
        private const int BACK_PADDING_H = 6;
        private const float BACK_OFFSET_X = WIDTH_HALF + BACK_PADDING_H / 2f,
            BACK_OFFSET_Y = 2f;
        private const float BACK_WIDTH = WIDTH + BACK_PADDING_H,
            BACK_HEIGHT = height + BACK_OFFSET_Y +12;
       
        private readonly Character character;
        private List<Spells> spells;
        private readonly float offsetY;
        private readonly float backOffsetY;

        public ChooseSpell(Character character, List<Spells> spells)
        {
            this.BackColor = Color.White;
            this.spells = spells;
            this.character = character;
            offsetY = -Tile.HALF_HEIGHT -1 * BACK_HEIGHT + HEX_OFFSET_Y;
            backOffsetY = offsetY - (BACK_OFFSET_Y / 2f);
            this.Size = new Size((int)BACK_WIDTH, (int)BACK_HEIGHT);
            this.Location = new Point((int)(character.CurrentTile.centerX - BACK_OFFSET_X), (int)(character.CurrentTile.centerY + backOffsetY));
            for (int i = 0; i < spells.Count; i++)
            {
                PictureBox[] pics = new PictureBox[spells.Count];
                pics[i] = new PictureBox();
                pics[i].ImageLocation = spells[i].image;
                pics[i].Size = new Size(18, 18);
                pics[i].SizeMode = PictureBoxSizeMode.StretchImage;
                pics[i].Location = new Point(i * 18 + 2 * (i + 1), i * 18 + 2 * (i + 1));
                int k = i;
                pics[i].MouseClick += new MouseEventHandler((sender, e) => picOneFaceUpA_Click(sender, e, spells[k]));

                this.Controls.Add(pics[i]);
            }
        }
        public void picOneFaceUpA_Click(object sender,EventArgs e,Spells spell)
        {
            character.resetMana();
            spell.castSpell(character);
            character.gameManager.removeFromForm(this);
        }

            
    }
}