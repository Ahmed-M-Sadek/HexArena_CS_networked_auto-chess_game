using ASU2019_NetworkedGameWorkshop.model.character.types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.model.ui
{
    public class CharShop : GraphicsObject
    {
        private Rectangle rectangle;
        private Label label;
        private int drawInShopY;
        private int drawInShopX;
        private StringFormat stringFormat;
        private List<CharacterType[]> charTypesList;
        private List<CharacterType[]> shopList;
        private ShopButton[] charBtns;

        private readonly int PADDINGX = 20;
        private readonly int PADDINGY = 10;
        private readonly controller.GameManager gameManager;

        public CharShop(GameForm gameForm, controller.GameManager gameManager)
        {
            this.gameManager = gameManager;
            rectangle = new Rectangle((int)(gameForm.Width * 0.78), (int)(gameForm.Height * 0.5), 270, 300);
            drawInShopX = rectangle.X + PADDINGX;
            drawInShopY = rectangle.Y + PADDINGY;
            stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            label = new Label();
            label.Text = "Buy Characters";

            charTypesList = new List<CharacterType[]>();
            foreach (var item in CharacterType.Values)
            {
                charTypesList.AddRange(item.ToList());
            }

            shopList = generateChars();
            charBtns = new ShopButton[3];
            for (int i = 0; i < shopList.Count; i++)
            {
                charBtns[i] = new ShopButton(shopList[i], gameManager);
                gameForm.Controls.Add(charBtns[i]);
            }
        }

        public override void draw(Graphics graphics)
        {
            Rectangle rect_price = new Rectangle(rectangle.X, (int)(drawInShopY + PADDINGY * 3 + CharacterType.HEIGHT), 270, 200);
            Rectangle rect_info = new Rectangle(rectangle.X, (int)(rect_price.Y + PADDINGY * 2), 270, 200);

            graphics.DrawRectangle(Pens.Black, rectangle);
            graphics.DrawString("Buy Characters", new Font("Arial", 14f, FontStyle.Bold), Brushes.Black, rectangle, stringFormat);

            for (int i = 0; i < shopList.Count; i++)
            {
                charBtns[i].Location = new Point((int)(drawInShopX + (CharacterType.WIDTH + PADDINGX) * i), drawInShopY + PADDINGX);
                if (charBtns[i].MouseHovered)
                {
                    CharacterType c = charBtns[i].CharacterType[0];
                    String charInfo = String.Format(
                        "character class: {0}\n" +
                        "Health Points: {1}\n" +
                        "range: {2}\n" +
                        "Atk Damage: {3}\n" +
                        "Atk Speed: {4}\n" +
                        "armour: {5}\n" +
                        "magicResist: {6}",
                        c.Name, c.Health, c.Range,
                        c.AtkDamage, c.AtkSpeed,
                        c.Armour, c.MagicResist);
                    graphics.DrawString(charInfo, new Font("Arial", 14f, FontStyle.Bold), Brushes.Black, rect_info, stringFormat);

                }
            }

            graphics.DrawString("Character cost: " + gameManager.CharacterPrice, new Font("Arial", 12f, FontStyle.Bold), Brushes.Black, rect_price, stringFormat);

        }
        public override void drawDebug(Graphics graphics)
        {
            //graphics.DrawRectangle(Pens.AliceBlue, rectangle);
        }

        private List<CharacterType[]> generateChars()
        {
            List<CharacterType[]> shopList = new List<CharacterType[]>();
            Random random = new Random();
            for (int i = 0; i < 3; i++)
            {
                shopList.Add(charTypesList[random.Next(charTypesList.Count())]);
                if (charBtns != null)
                    charBtns[i].CharacterType = shopList[i];
            }
            return shopList;
        }

        internal void generate()
        {
            shopList = generateChars();
        }
    }
}
