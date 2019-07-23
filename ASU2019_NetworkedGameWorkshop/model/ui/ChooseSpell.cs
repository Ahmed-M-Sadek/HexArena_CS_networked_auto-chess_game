using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.grid;
using ASU2019_NetworkedGameWorkshop.model.spell;
using System.Collections.Generic;
using System;
using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model
{
    public class ChooseSpell : GraphicsObject
    {
        private const float WIDTH = Tile.WIDTH - 30 * 2,
            WIDTH_HALF = WIDTH / 2f,
            Height = 8;
        private const float HEX_OFFSET_Y = Tile.HEIGHT * 0.2f;
        private const int BACK_PADDING_H = 6;
        private const float BACK_OFFSET_X = WIDTH_HALF + BACK_PADDING_H / 2f,
            BACK_OFFSET_Y = 2f;
        private const float BACK_WIDTH = WIDTH + BACK_PADDING_H,
            BACK_HEIGHT = Height + BACK_OFFSET_Y +12;

        private static readonly Brush BACK_BRUSH = new SolidBrush(Color.White);
        private readonly Character character;
        private List<Spells> spells;
        private readonly float offsetY;
        private readonly float backOffsetY;

        public ChooseSpell(Character character, List<Spells> spells)
        {
            this.spells = spells;
            this.character = character;
            offsetY = -Tile.HALF_HEIGHT -1 * BACK_HEIGHT + HEX_OFFSET_Y;
            backOffsetY = offsetY - (BACK_OFFSET_Y / 2f);
        }

        public override void draw(Graphics graphics)
        {
            graphics.FillRectangle(BACK_BRUSH,
                character.CurrentTile.centerX - BACK_OFFSET_X,
                character.CurrentTile.centerY + backOffsetY,
                BACK_WIDTH,
                BACK_HEIGHT);
            for(int i=0; i < spells.Count; i++)
            {
                graphics.DrawImage(spells[i].image, character.CurrentTile.centerX - WIDTH_HALF,
                character.CurrentTile.centerY + offsetY,
                18,
                18);
                
            }
        }
        
    }
}