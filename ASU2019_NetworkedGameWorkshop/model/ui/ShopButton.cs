using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.model.character.types;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.model.ui
{
    class ShopButton : Button
    {
        private GameManager gameManager;
        private CharacterType[] characterType;
        public bool MouseHovered
        {
            get; set;
        }

        public CharacterType[] CharacterType
        {
            get
            {
                return characterType;
            }
            set
            {
                characterType = value;
                Text = CharacterType[0].ToString();
            }
        }


        public ShopButton(CharacterType[] characterType, GameManager gameManager)
        {
            this.gameManager = gameManager;
            CharacterType = characterType;
            Size = new Size((int)character.types.CharacterType.WIDTH, (int)character.types.CharacterType.HEIGHT);
            Click += ShopButton_Click;
            MouseEnter += ShopButton_MouseHover;
            MouseLeave += ShopButton_MouseLeave;
        }

        private void ShopButton_MouseHover(object sender, EventArgs e)
        {
            this.MouseHovered = true;
        }
        private void ShopButton_MouseLeave(object sender, EventArgs e)
        {
            this.MouseHovered = false;
        }

        private void ShopButton_Click(object sender, System.EventArgs e)
        {
            gameManager.AddCharacter(CharacterType);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            pevent.Graphics.FillRectangle(Brushes.LightGray, 0, 0, Width, Height);
            TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
            TextRenderer.DrawText(pevent.Graphics, Text, Font, new Point(Width + 3, Height / 2), ForeColor, flags);
        }

    }
}
