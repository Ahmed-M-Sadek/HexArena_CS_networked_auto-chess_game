using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.model.character.types;
using ASU2019_NetworkedGameWorkshop.model.grid;
using System;
using System.Collections.Generic;
using System.Drawing;
using ASU2019_NetworkedGameWorkshop.model.spell;
using System.Threading;


namespace ASU2019_NetworkedGameWorkshop.model.character {
    public class Character : GraphicsObject {
        public enum Teams { Red, Blue };

        private readonly Grid grid ;
        public readonly Teams team;
        public Character CurrentTarget { get; private set; }
        private Tile toMoveTo;

        public Tile CurrentTile { get; set; }//public set ?
        public CharacterType CharacterType { get; private set; }

        public bool IsDead { get; private set; }

        private List<Spell> spells;

        private int healthPoints;
        private int healthPointsMax;

        private int chargePoints;
        private int chargePointsMax;

        private GameManager gameManager;
        private long nextAtttackTime;
        private readonly StatBar hpBar;
        private readonly StatBar charageBar;


        public Character(Grid grid, Tile currentTile, Teams team,
            CharacterType characterType, GameManager gameManager) {
            this.grid = grid;
            currentTile.CurrentCharacter = this;
            this.team = team;
            this.spells = new List<Spell>();

            switch(team) {
                case Teams.Red:
                    grid.TeamRed.Add(this);
                    break;
                case Teams.Blue:
                    grid.TeamBlue.Add(this);
                    break;
            }

            CharacterType = characterType;

            IsDead = false;
            healthPoints = characterType.BaseHP;
            healthPointsMax = characterType.BaseHP;
            chargePoints = 50;
            chargePointsMax = characterType.MaxChargePoints;

            hpBar = new StatBar(this,
                team == Teams.Blue ? Brushes.GreenYellow : Brushes.OrangeRed, 0);
            charageBar = new StatBar(this, Brushes.Blue, 1);

            this.gameManager = gameManager;
        }

        public void healHealthPoints(int healValue) {
            if(healValue < 0) {
                throw new ArgumentException();
            }
            healthPoints = Math.Min(healthPoints + healValue, healthPointsMax);
        }

        public void learnSpell(Spell spell)
        {
            spells.Add(spell);
        }

        public void takeDamage(int dmgValue) {
            if(dmgValue < 0) {
                throw new ArgumentException();
            }
            healthPoints -= dmgValue;
            if(healthPoints < 0) {
                healthPoints = 0;
                IsDead = true;
                if(CurrentTile != null) {
                    //CurrentTile.CurrentCharacter = null;
                    //CurrentTile = null;
                    //causes an excepetion in path finding 
                }
            } else {
                chargePoints = Math.Min(chargePoints + 10, chargePointsMax);
            }
        }

        public override void draw(Graphics graphics) {
            graphics.FillRectangle(team == Teams.Blue ? Brushes.BlueViolet : Brushes.Red,
                CurrentTile.centerX - CharacterType.WidthHalf,
                CurrentTile.centerY - CharacterType.HeightHalf,
                CharacterType.Width, CharacterType.Height);

            hpBar.setTrackedAndDraw(graphics, healthPoints, healthPointsMax);
            charageBar.setTrackedAndDraw(graphics, chargePoints, chargePointsMax);
        }

        public Grid getGrid()
        {
            return this.grid;
        }
        public bool tick() {
            if(toMoveTo != null) {
                CurrentTile.CurrentCharacter = null;
                CurrentTile.Walkable = true;
                toMoveTo.CurrentCharacter = this;
                toMoveTo = null;

                return true;
            }
            return false;
        }
        private void chooseSpell()
        {
            chargePoints = 0;
            Console.WriteLine("Choose spell");
            int currentSpell = Convert.ToInt32(Console.ReadLine());
            spells[currentSpell].castSpell(this);
            
        }
        public bool update() {
            
            if (toMoveTo == null) {
                List<Tile> path = null;
                if(CurrentTarget == null
                    || CurrentTarget.IsDead) {
                    try {
                        (path, CurrentTarget) = PathFinding.findPathToClosestEnemy(CurrentTile, team, grid);//temp
                    } catch(PathFinding.PathNotFoundException) {
                        return false;
                    }
                }
                if (PathFinding.getDistance(CurrentTile, CurrentTarget.CurrentTile) <= CharacterType.Range)
                {
                    if (chargePoints == chargePointsMax && spells.Count != 0 )
                    {

                        chooseSpell();
                        return true;
                    }
                }
                if (PathFinding.getDistance(CurrentTile, CurrentTarget.CurrentTile) <= CharacterType.Range) {
                    if(gameManager.ElapsedTime > nextAtttackTime) {
                        nextAtttackTime = gameManager.ElapsedTime + 500;
                        CurrentTarget.takeDamage(10);
                        return true;
                    }
                }
                 else {
                    if(path == null) {
                        try {
                            path = PathFinding.findPath(CurrentTile, CurrentTarget.CurrentTile, grid, (Tile[,]) grid.Tiles.Clone());
                        } catch(PathFinding.PathNotFoundException) {
                            return false;
                        }
                    }
                    toMoveTo = path[0];
                }
            }
            return false;
        }
    }
}
