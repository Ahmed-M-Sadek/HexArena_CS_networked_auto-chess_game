using ASU2019_NetworkedGameWorkshop.controller.networking;
using ASU2019_NetworkedGameWorkshop.controller.networking.game;
using ASU2019_NetworkedGameWorkshop.model;
using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.character.types;
using ASU2019_NetworkedGameWorkshop.model.grid;
using ASU2019_NetworkedGameWorkshop.model.spell;
using ASU2019_NetworkedGameWorkshop.model.ui;
using ASU2019_NetworkedGameWorkshop.model.ui.shop;
using ASU2019_NetworkedGameWorkshop.model.ui.shop.charactershop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static ASU2019_NetworkedGameWorkshop.controller.StageManager;
using static ASU2019_NetworkedGameWorkshop.model.ui.StageTimer;

namespace ASU2019_NetworkedGameWorkshop.controller
{
    public class GameManager
    {
        private const int GAMELOOP_INTERVAL = 50, TICK_INTERVAL = 1000;
        private const int GRID_HEIGHT = 6, GRID_WIDTH = 7;

        private readonly Grid grid;
        private readonly GameForm gameForm;
        private readonly Timer timer;
        private readonly StageTimer stageTimer;
        private readonly Stopwatch stopwatch;
        private readonly PlayersLeaderBoard playersLeaderBoard;
        private readonly GameNetworkManager gameNetworkManager;
        private readonly List<Player> otherPlayers;
        public static int randomSeed;

        private StageManager stageManager;
        private Shop spellShop;
        private long nextTickTime;
        private bool updateCanvas;

        /// <summary>
        /// Elapsed Time in ms.
        /// <para>Increments even if program execution was paused (uses system time).</para>
        /// </summary>
        public long ElapsedTime { get { return stopwatch.ElapsedMilliseconds; } }
        public List<Character> TeamBlue { get; private set; }
        public List<Character> TeamRed { get; private set; }
        public Tile SelectedTile { get; set; }
        public Player Player { get; }
        public CharShop CharShop { get; private set; }

        public GameStage CurrentGameStage
        {
            get
            {
                return stageManager.CurrentGameStage;
            }
        }

        public bool IsHost { get; }

        public GameManager(GameForm gameForm, string playerName, bool isHost, string ip, int port)
        {
            this.gameForm = gameForm;

            if (isHost)
            {
                IsHost = isHost;
                gameNetworkManager = new GameServer(port);
                randomSeed = (int)DateTime.Now.Ticks;
                gameNetworkManager.enqueueMsg(NetworkMsgPrefix.SetSeed,
                              GameNetworkUtilities.serializeRandomSeed(randomSeed));
            }
            else
            {
                gameNetworkManager = new GameClient(ip, port);
            }

            grid = new Grid(GRID_WIDTH, GRID_HEIGHT,
                (int)((gameForm.Width - (Tile.WIDTH * GRID_WIDTH)) / 3),
                (int)((gameForm.Height - (Tile.HEIGHT * GRID_HEIGHT)) / 3) + 30,
                this);

            TeamBlue = new List<Character>();
            TeamRed = new List<Character>();

            Player = new Player(playerName, true);

            otherPlayers = new List<Player>();
            playersLeaderBoard = new PlayersLeaderBoard(Player);

            CharShop = new CharShop(gameForm, this);
            spellShop = new Shop(gameForm, this, gameNetworkManager);

            stageTimer = new StageTimer(this);
            stageManager = new StageManager(stageTimer,
                                            TeamBlue,
                                            TeamRed,
                                            grid,
                                            Player,
                                            playersLeaderBoard,
                                            CharShop,
                                            this,
                                            gameNetworkManager);
            stageTimer.switchStageEvent += stageManager.switchStage;

            stopwatch = new Stopwatch();
            timer = new Timer
            {
                Interval = GAMELOOP_INTERVAL
            };
            timer.Tick += new EventHandler(gameLoop);
        }

        internal void resetTickTimeForRoundStart()
        {
            nextTickTime = ((ElapsedTime / TICK_INTERVAL) + 2) * TICK_INTERVAL;
        }

        public void addRangeToForm(params Control[] controls)
        {
            gameForm.Controls.AddRange(controls);
        }
        public void removeRangeFromForm(params Control[] controls)
        {
            foreach (Control control in controls)
            {
                gameForm.Controls.Remove(control);
            }
        }

        public void startGame()
        {
            gameNetworkManager.start();

            gameNetworkManager.enqueueMsg(NetworkMsgPrefix.NewPlayer, GameNetworkUtilities.serializePlayerHP(Player));

            gameStart();
            timer.Start();
        }

        public void mouseClick(MouseEventArgs e)
        {
            if (stageManager.CurrentGameStage == GameStage.Buy)
            {
                if (e.Button == MouseButtons.Right)
                {
                    deselectSelectedTile();

                }
                else if (e.Button == MouseButtons.Left)
                {
                    Tile tile = grid.getSelectedHexagon(e.X, e.Y);
                    if (tile != null)
                    {
                        selectTile(tile);
                    }
                }
            }
            else if (stageManager.CurrentGameStage == GameStage.Fight)
            {
                Tile tile = grid.getSelectedHexagon(e.X, e.Y);
                if (tile != null && tile.CurrentCharacter != null)
                {
                    if (tile.CurrentCharacter.ActiveSpells.Count > 0 && tile.CurrentCharacter.SpellReady == false)
                    {
                        tile.CurrentCharacter.showChooseSpell();
                    }
                }
            }
        }


        private void selectTile(Tile tile)
        {
            if (SelectedTile == tile)
            {
                deselectSelectedTile();
            }
            else if (SelectedTile == null)
            {
                SelectedTile = tile;
                SelectedTile.Selected = true;
                updateCanvas = true;
                spellShop.updateShop();
            }
            else
            {
                if ((tile.CurrentCharacter == null || tile.CurrentCharacter.team != Character.Teams.Red)
                    && (SelectedTile.CurrentCharacter == null || SelectedTile.CurrentCharacter.team != Character.Teams.Red))
                {
                    if (IsHost && SelectedTile.Y >= grid.GridHeight / 2 && tile.Y >= grid.GridHeight / 2 ||
                        !IsHost && SelectedTile.Y < grid.GridHeight / 2 && tile.Y < grid.GridHeight / 2)
                    {
                        SoundManager.PlaySound("swapCharacter.wav");
                        swapCharacters(tile, SelectedTile);
                        gameNetworkManager.enqueueMsg(NetworkMsgPrefix.CharacterSwap,
                                                      GameNetworkUtilities.serializeCharacterSwap(tile, SelectedTile));
                        deselectSelectedTile();
                    }

                }
            }
        }

        internal void endGame(string msg)
        {
            timer.Stop();
            MessageBox.Show(msg, msg, MessageBoxButtons.OK, MessageBoxIcon.Information);
            gameForm.Close();
        }

        private void swapCharacters(Tile tile, Tile selectedTile)
        {
            Character temp = selectedTile.CurrentCharacter;
            selectedTile.CurrentCharacter = tile.CurrentCharacter;
            tile.CurrentCharacter = temp;
        }

        public void deselectSelectedTile()
        {
            if (SelectedTile != null)
            {
                SelectedTile.Selected = false;
                SelectedTile = null;
                updateCanvas = true;
                spellShop.updateShop();
            }
        }

        public void updatePaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            grid.draw(e.Graphics);

            TeamBlue.ForEach(character => character.draw(e.Graphics));
            TeamRed.ForEach(character => character.draw(e.Graphics));

            stageTimer.draw(e.Graphics);

            Player.draw(e.Graphics);
            e.Graphics.DrawString("Round: " + stageManager.CurrentRound, new Font("Roboto", 12, FontStyle.Bold), Brushes.Black, 800, 15);
            playersLeaderBoard.draw(e.Graphics);
            CharShop.draw(e.Graphics);

            if (false)
            {
                grid.drawDebug(e.Graphics);
                TeamBlue.ForEach(character => character.drawDebug(e.Graphics));
                TeamRed.ForEach(character => character.drawDebug(e.Graphics));
            }
        }

        private void gameStart()
        {
            stopwatch.Start();
            stageTimer.resetTimer(StageTime.BUY);
        }

        private void gameLoop(object sender, EventArgs e)
        {
            checkNetworkMsgs();

            updateCanvas = stageTimer.update() || updateCanvas;

            if (stageManager.CurrentGameStage == GameStage.Buy)
            {
                updateCanvas = stageUpdateBuy() || updateCanvas;
            }
            else if (stageManager.CurrentGameStage == GameStage.Fight)
            {
                updateCanvas = stageUpdateFight() || updateCanvas;
            }

            if (updateCanvas)
            {
                gameForm.Refresh();
            }
        }

        private void checkNetworkMsgs()
        {
            if (gameNetworkManager.DataReceived.Count > 0)
            {
                bool updateLeaderBoard = false;
                while (gameNetworkManager.DataReceived.Count > 0)
                {
                    updateLeaderBoard = applyNetworkMsg() || updateLeaderBoard;
                }
                if (updateLeaderBoard)
                {
                    playersLeaderBoard.update();
                }
            }
        }

        private bool applyNetworkMsg()
        {
            bool updateLeaderBoard = false;
            gameNetworkManager.DataReceived.TryDequeue(out string result);
            string[] msg = result.Split(GameNetworkManager.NETWORK_MSG_SEPARATOR);

            if (msg[0].Equals(NetworkMsgPrefix.CharacterSwap.getPrefix()))
            {
                (Tile tile, Tile selectedTile) = GameNetworkUtilities.parseCharacterSwap(msg, grid);
                swapCharacters(tile, selectedTile);
            }
            if (msg[0].Equals(NetworkMsgPrefix.SetSeed.getPrefix()))
            {
                randomSeed = Convert.ToInt32(msg[1]);
            }
            else if (msg[0].Equals(NetworkMsgPrefix.StageChange.getPrefix()))
            {
                (GameStage gameStage, bool HostWins) = GameNetworkUtilities.parseStage(msg[1], msg[2]);
                stageTimer.switchStageEvent();
                if (gameStage == GameStage.FightToBuy)
                    if (HostWins)
                    {
                        foreach (Character character in TeamBlue.Where(e => !e.IsDead))
                        {
                            character.takeDamage(10000, DamageType.PhysicalDamage);
                        }
                    }
                    else
                    {
                        foreach (Character character in TeamRed.Where(e => !e.IsDead))
                        {
                            character.takeDamage(10000, DamageType.PhysicalDamage);
                        }
                    }
            }
            else if (msg[0].Equals(NetworkMsgPrefix.NewCharacter.getPrefix()))
            {
                TeamRed.Add(CharStatToCharacter(GameNetworkUtilities.parseCharacter(msg)));
            }
            else if (msg[0].Equals(NetworkMsgPrefix.PlayerHealthUpdate.getPrefix()))
            {
                Player otherPlayer = otherPlayers.Find(player => player.Name.Equals(msg[1]));
                otherPlayer.Health = int.Parse(msg[2]);
                updateLeaderBoard = true;
                if (otherPlayer.Health == 0)
                {
                    endGame("You Win");
                }
            }
            else if (msg[0].Equals(NetworkMsgPrefix.NewPlayer.getPrefix()))
            {
                Player player = new Player(msg[1])
                {
                    Health = int.Parse(msg[2])
                };
                otherPlayers.Add(player);
                playersLeaderBoard.addPlayers(player);
                updateLeaderBoard = true;
            }
            else if (msg[0].Equals(NetworkMsgPrefix.SellCharacter.getPrefix()))
            {
                Tile tile = grid.Tiles[int.Parse(msg[1]), int.Parse(msg[2])];
                TeamRed.Remove(tile.CurrentCharacter);
                tile.CurrentCharacter = null;
                spellShop.updateShop();
            }
            else if (msg[0].Equals(NetworkMsgPrefix.LevelUpCharacter.getPrefix()))
            {
                grid.Tiles[int.Parse(msg[1]), int.Parse(msg[2])].CurrentCharacter.levelUp();
                spellShop.updateShop();
            }
            else if (msg[0].Equals(NetworkMsgPrefix.LevelUpSpell.getPrefix()))
            {
                grid.Tiles[int.Parse(msg[1]), int.Parse(msg[2])].CurrentCharacter.upgradeSpell(Spells.getSpell(int.Parse(msg[3])));
            }
            else if (msg[0].Equals(NetworkMsgPrefix.LearnSpell.getPrefix()))
            {
                grid.Tiles[int.Parse(msg[1]), int.Parse(msg[2])].CurrentCharacter.learnSpell(Spells.getSpell(int.Parse(msg[3])));
            }
            else if (msg[0].Equals(NetworkMsgPrefix.DefaultSkill.getPrefix()))
            {
                TeamRed[int.Parse(msg[1])].DefaultSkill = Spells.getSpell(int.Parse(msg[2]));
            }
            else if (msg[0].Equals(NetworkMsgPrefix.AddActiveSpells.getPrefix()))
            {
                Character character = grid.Tiles[int.Parse(msg[1]), int.Parse(msg[2])].CurrentCharacter;
                character.ActiveSpells.Add(Spells.getSpell(int.Parse(msg[3])));
                character.ChooseSpell.refreshPanel(character, character.ActiveSpells);
            }
            else if (msg[0].Equals(NetworkMsgPrefix.RemActiveSpells.getPrefix()))
            {
                Character character = grid.Tiles[int.Parse(msg[1]), int.Parse(msg[2])].CurrentCharacter;
                character.ActiveSpells.Remove(Spells.getSpell(int.Parse(msg[3])));
                character.ChooseSpell.refreshPanel(character, character.ActiveSpells);

            }
            else if (msg[0].Equals(NetworkMsgPrefix.ExchActiveSpells.getPrefix()))
            {
                Character character = TeamRed[int.Parse(msg[1])];
                character.ChooseSpell.spellSwap(int.Parse(msg[2]));
                character.ChooseSpell.refreshPanel(character, character.ActiveSpells);
            }
            else if (msg[0].Equals("EXIT"))
            {
                timer.Stop();
                MessageBox.Show("Disconnected", "Disconnected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                gameForm.Close();
            }
            return updateLeaderBoard;
        }

        private bool stageUpdateBuy()
        {
            foreach (Character character in TeamBlue.Where(e => !e.IsDead))
            {
                updateCanvas = character.updateBuy() || updateCanvas;
            }

            foreach (Character character in TeamRed.Where(e => !e.IsDead))
            {
                updateCanvas = character.updateBuy() || updateCanvas;
            }
            return updateCanvas;
        }

        private bool stageUpdateFight()
        {
            List<Character> Team1, Team2;
            (Team1, Team2) = IsHost ? (TeamBlue, TeamRed) : (TeamRed, TeamBlue);
            (Team1, Team2) = (stageManager.CurrentRound & 1) == 0 ? (Team1, Team2) : (Team2, Team1);


            if (Team1.Count(e => !e.IsDead) == 0 || Team2.Count(e => !e.IsDead) == 0)
            {
                stageTimer.endTimer();
                return true;
            }

            bool updateCanvas = false;

            foreach (Character character in Team1.Where(e => !e.IsDead))
            {
                updateCanvas = character.update() || updateCanvas;
            }

            foreach (Character character in Team2.Where(e => !e.IsDead))
            {
                updateCanvas = character.update() || updateCanvas;
            }


            if (nextTickTime < ElapsedTime)
            {
                nextTickTime = ((ElapsedTime / TICK_INTERVAL) + 1) * TICK_INTERVAL;
                foreach (Character character in Team1.Where(e => !e.IsDead))
                {
                    updateCanvas = character.tick() || updateCanvas;
                }
                foreach (Character character in Team2.Where(e => !e.IsDead))
                {
                    updateCanvas = character.tick() || updateCanvas;
                }
            }

            return updateCanvas;
        }

        public void AddCharacter(CharacterType[] characterType)
        {
            for (int j = grid.GridHeight - 1; j > (grid.GridHeight - 1) / 2; j--)
            {
                for (int i = grid.GridWidth - 1; i >= 0; i--)
                {
                    (int ii, int jj) = IsHost ? (i, j) : (grid.GridWidth - 1 - i, grid.GridHeight - 1 - j);
                    if (grid.Tiles[ii, jj].CurrentCharacter == null)
                    {
                        Character item = new Character(grid, grid.Tiles[ii, jj], Character.Teams.Blue, characterType, this, gameNetworkManager);
                        TeamBlue.Add(item);
                        gameNetworkManager.enqueueMsg(NetworkMsgPrefix.NewCharacter, GameNetworkUtilities.serializeCharacter(item));
                        return;
                    }
                }
            }
        }

        private Character CharStatToCharacter(GameNetworkUtilities.CharStat charStat)
        {
            return new Character(grid, grid.Tiles[charStat.X, charStat.Y], Character.Teams.Red, charStat.charType, this, gameNetworkManager);
        }
    }
}
