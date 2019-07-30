using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.character.types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ASU2019_NetworkedGameWorkshop.model.ui.shop
{
    public class CharShop : GraphicsObject
    {
        private const int OFFERED_CHARACTERS_COUNT = 3;
        private static readonly int PADDING_X = 20,
                                    PADDING_Y = 10;
        private static readonly Font FONT_INFO = new Font("Arial", 14f, FontStyle.Bold),
                                     FONT_PRICE = new Font("Arial", 12f, FontStyle.Bold);

        private readonly controller.GameManager gameManager;
        private readonly int drawInShopY;
        private readonly int drawInShopX;
        private readonly StringFormat stringFormat;
        private readonly Random random;
        private readonly List<CharacterType[]> charTypesList;
        private readonly ShopButton[] shopButtons;
        private readonly List<CharacterType[]> offeredCharacters;

        private Rectangle rectangle;

        private int CharacterPrice
        {
            get
            {//not working as required
                return Math.Min(40, gameManager.TeamBlue.Count * 10 - Math.Max(0, gameManager.TeamBlue.Count * 2));
            }
        }

        public CharShop(GameForm gameForm, controller.GameManager gameManager)
        {
            this.gameManager = gameManager;
            rectangle = new Rectangle((int)(gameForm.Width * 0.78), (int)(gameForm.Height * 0.5), 270, 300);
            drawInShopX = rectangle.X + PADDING_X;
            drawInShopY = rectangle.Y + PADDING_Y;
            stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center
            };
            random = new Random();

            charTypesList = new List<CharacterType[]>();
            foreach (var item in CharacterType.Values)
            {
                charTypesList.AddRange(item.ToList());
            }

            shopButtons = new ShopButton[3];
            for (int i = 0; i < OFFERED_CHARACTERS_COUNT; i++)
            {
                shopButtons[i] = new ShopButton();
                shopButtons[i].OnPurchaseEvent += purchase;
                shopButtons[i].Location = new Point((int)(drawInShopX + (CharacterType.WIDTH + PADDING_X) * i), drawInShopY + PADDING_X);
                gameForm.Controls.Add(shopButtons[i]);
            }

            offeredCharacters = new List<CharacterType[]>();
            refreshShop();
        }

        private void purchase(CharacterType[] characterType)
        {
            if (gameManager.Player.Gold < CharacterPrice
                || gameManager.CurrentGameStage != controller.StageManager.GameStage.Buy)
                return;

            gameManager.Player.Gold -= CharacterPrice;
            gameManager.AddCharacter(characterType);
        }

        public override void draw(Graphics graphics)
        {
            Rectangle rect_price = new Rectangle(rectangle.X, (int)(drawInShopY + PADDING_Y * 3 + CharacterType.HEIGHT), 270, 200);
            Rectangle rect_info = new Rectangle(rectangle.X, rect_price.Y + PADDING_Y * 2, 270, 200);

            graphics.DrawRectangle(Pens.Black, rectangle);
            graphics.DrawString("Buy Characters", FONT_INFO, Brushes.Black, rectangle, stringFormat);

            ShopButton shopButton = shopButtons.Where(btn => btn.MouseHovered).FirstOrDefault();
            if (shopButton != null)
            {
                CharacterType characterType = shopButton.CharacterType[0];
                graphics.DrawString(
                    $@"Character Class: 
Health Points: 
Range: 
Atk Damage: 
Atk Speed: 
Armour: 
Magic Resist:",
                    FONT_INFO,
                    Brushes.Black,
                    rect_info,
                    new StringFormat
                    {
                        Alignment = StringAlignment.Near
                    });

                graphics.DrawString(
                                    $@"{characterType.Name}
{characterType[StatusType.HealthPoints]}
{characterType[StatusType.Range]}
{characterType[StatusType.AttackDamage]}
{1000f / characterType[StatusType.AttackSpeed]:0.00}
{characterType[StatusType.Armor]}
{characterType[StatusType.MagicResist]}",
                                    FONT_INFO,
                                    Brushes.Black,
                                    rect_info,
                                    new StringFormat
                                    {
                                        Alignment = StringAlignment.Far
                                    });

            }

            Brush costBrush = gameManager.Player.Gold >= CharacterPrice ? Brushes.Goldenrod : Brushes.Red;
            graphics.DrawString("Character cost: " + CharacterPrice, FONT_PRICE, costBrush, rect_price, stringFormat);
        }
        public override void drawDebug(Graphics graphics)
        {
        }

        internal void refreshShop()
        {
            offeredCharacters.Clear();
            for (int i = 0; i < OFFERED_CHARACTERS_COUNT; i++)
            {
                offeredCharacters.Add(charTypesList[random.Next(charTypesList.Count())]);
                shopButtons[i].CharacterType = offeredCharacters[i];
            }
        }
    }
}
