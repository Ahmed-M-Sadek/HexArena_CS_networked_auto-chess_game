using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.model.character.types;
using ASU2019_NetworkedGameWorkshop.model.grid;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model.character {
    public class Character : GraphicsObject {
        public enum Teams { Red, Blue };

        public Tile CurrentTile { get; set; }//public set ?
        public CharacterType CharacterType {
            get {
                return characterType[CurrentLevel];
            }
        }
        public bool IsDead { get; private set; }
        public int CurrentLevel { get; private set; }

        public readonly Teams team;

        private readonly Grid grid;
        private readonly StatBar hpBar, charageBar;
        private readonly GameManager gameManager;
        private readonly CharacterType[] characterType;

        private int healthPoints, healthPointsMax;
        private int chargePoints, chargePointsMax;
        private Character currentTarget;
        private Tile toMoveTo;
        private long nextAtttackTime;

        public Character(Grid grid, Tile currentTile, Teams team,
            CharacterType[] characterType, GameManager gameManager) {
            this.grid = grid;
            currentTile.CurrentCharacter = this;
            this.team = team;
            this.characterType = characterType;
            this.gameManager = gameManager;

            switch(team) {
                case Teams.Red:
                    grid.TeamRed.Add(this);
                    break;
                case Teams.Blue:
                    grid.TeamBlue.Add(this);
                    break;
            }

            IsDead = false;

            hpBar = new StatBar(this,
                team == Teams.Blue ? Brushes.GreenYellow : Brushes.OrangeRed, 0);
            charageBar = new StatBar(this, Brushes.Blue, 1);

            initializeStats();
        }

        private void initializeStats() {
            healthPoints = CharacterType.BaseHP;
            healthPointsMax = CharacterType.BaseHP;
            chargePoints = CharacterType.ChargeStarting;
            chargePointsMax = CharacterType.ChargeMax;
        }

        public void healHealthPoints(int healValue) {
            if(healValue < 0) {
                throw new ArgumentException();
            }
            healthPoints = Math.Min(healthPoints + healValue, healthPointsMax);
        }

        public void takeDamage(int dmgValue, DamageType damageType) {
            if(dmgValue < 0) {
                throw new ArgumentException();
            }
            healthPoints -= (int) (dmgValue * 100 / 
                (100 + (damageType == DamageType.MagicDamage ?  CharacterType.Armor: CharacterType.MagicResist)));
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

        private void levelUp() {
            if(CurrentLevel < CharacterType.MAX_CHAR_LVL) {
                CurrentLevel++;
            }
        }

        public override void draw(Graphics graphics) {
            graphics.FillRectangle(team == Teams.Blue ? Brushes.BlueViolet : Brushes.Red,
                CurrentTile.centerX - CharacterType.WIDTH_HALF,
                CurrentTile.centerY - CharacterType.HEIGHT_HALF,
                CharacterType.WIDTH, CharacterType.HEIGHT);

            hpBar.setTrackedAndDraw(graphics, healthPoints, healthPointsMax);
            charageBar.setTrackedAndDraw(graphics, chargePoints, chargePointsMax);
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

        public bool update() {
            if(toMoveTo == null) {
                List<Tile> path = null;
                if(currentTarget == null
                    || currentTarget.IsDead) {
                    try {
                        (path, currentTarget) = PathFinding.findPathToClosestEnemy(CurrentTile, team, grid);//temp
                    } catch(PathFinding.PathNotFoundException) {
                        return false;
                    }
                }
                if(PathFinding.getDistance(CurrentTile, currentTarget.CurrentTile) <= CharacterType.Range) {
                    if(gameManager.ElapsedTime > nextAtttackTime) {
                        nextAtttackTime = gameManager.ElapsedTime + CharacterType.AttackSpeed;
                        currentTarget.takeDamage(CharacterType.AttackDamage, DamageType.PhysicalDamage);//temp DamageType
                        return true;
                    }
                } else {
                    if(path == null) {
                        try {
                            path = PathFinding.findPath(CurrentTile, currentTarget.CurrentTile, grid, (Tile[,]) grid.Tiles.Clone());
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
