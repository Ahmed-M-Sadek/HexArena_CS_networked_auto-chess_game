using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.model.character.types;
using ASU2019_NetworkedGameWorkshop.model.grid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
        private readonly Dictionary<StatusType, int> statsAdder;
        private readonly Dictionary<StatusType, float> statsMultiplier;

        private Dictionary<StatusType, int> stats;
        private List<StatusEffect> statusEffects;
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

            stats = CharacterType.statsCopy();
            statsMultiplier = new Dictionary<StatusType, float>();
            statsAdder = new Dictionary<StatusType, int>();
            foreach(StatusType statusType in Enum.GetValues(typeof(StatusType))) {
                statsAdder.Add(statusType, 0);
                statsMultiplier.Add(statusType, 1f);
            }

            statusEffects = new List<StatusEffect>();

            IsDead = false;

            hpBar = new StatBar(this,
                team == Teams.Blue ? Brushes.GreenYellow : Brushes.OrangeRed, 0);
            charageBar = new StatBar(this, Brushes.Blue, 1);
        }


        public void healHealthPoints(int healValue) {
            if(healValue < 0) {
                throw new ArgumentException("healValue should be positive: " + healValue);
            }
            stats[StatusType.HealthPoints] = Math.Min(stats[StatusType.HealthPoints] + healValue,
                                                        stats[StatusType.HealthPointsMax]);
        }

        public void takeDamage(int dmgValue, DamageType damageType) {
            if(dmgValue < 0) {
                throw new ArgumentException("dmgValue should be positive: " + dmgValue);
            }
            stats[StatusType.HealthPoints] -= (int) (dmgValue * 100 /
                (100 + (damageType == DamageType.MagicDamage ? stats[StatusType.Armor] : stats[StatusType.MagicResist])));
            if(stats[StatusType.HealthPoints] < 0) {
                stats[StatusType.HealthPoints] = 0;
                IsDead = true;
                if(CurrentTile != null) {
                    //CurrentTile.Walkable = true;
                    //CurrentTile.CurrentCharacter = null;
                    //CurrentTile = null;
                    //causes an excepetion in path finding 
                }
            } else {
                stats[StatusType.Charge] = Math.Min(stats[StatusType.Charge] + 10, stats[StatusType.ChargeMax]);//temp value
            }
        }

        public void reset() {
            stats = CharacterType.statsCopy();
            statusEffects.Clear();
            IsDead = false;
            toMoveTo = null;
            currentTarget = null;
        }

        public void addStatusEffect(StatusEffect statusEffect) {
            applyStatusEffect(statusEffect);
            statusEffects.Add(statusEffect);
        }

        public override void draw(Graphics graphics) {
            graphics.FillRectangle(team == Teams.Blue ? Brushes.BlueViolet : Brushes.Red,
                CurrentTile.centerX - CharacterType.WIDTH_HALF,
                CurrentTile.centerY - CharacterType.HEIGHT_HALF,
                CharacterType.WIDTH, CharacterType.HEIGHT);

            hpBar.setTrackedAndDraw(graphics, stats[StatusType.HealthPoints], stats[StatusType.HealthPointsMax]);
            charageBar.setTrackedAndDraw(graphics, stats[StatusType.Charge], stats[StatusType.ChargeMax]);
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
            statusEffects = statusEffects.Where(effect => {
                if(effect.removeEffectTimeStamp < gameManager.ElapsedTime) {
                    effect.inverseValue();
                    applyStatusEffect(effect);
                    return false;
                }
                return true;
            }).ToList();

            if(toMoveTo == null) {
                List<Tile> path = null;
                if(currentTarget == null
                    || currentTarget.IsDead) {
                    try {
                        (path, currentTarget) = PathFinding.findPathToClosestEnemy(CurrentTile, team, grid, gameManager);//temp
                    } catch(PathFinding.PathNotFoundException) {
                        return false;
                    }
                }
                if(PathFinding.getDistance(CurrentTile, currentTarget.CurrentTile) <= stats[StatusType.Range]) {
                    if(gameManager.ElapsedTime > nextAtttackTime) {
                        nextAtttackTime = gameManager.ElapsedTime + stats[StatusType.AttackSpeed];
                        currentTarget.takeDamage(stats[StatusType.AttackDamage], DamageType.PhysicalDamage);//temp DamageType?
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

        private void levelUp() {
            if(CurrentLevel < CharacterType.MAX_CHAR_LVL) {
                CurrentLevel++;
                stats = CharacterType.statsCopy();
            }
        }
        private void applyStatusEffect(StatusEffect statusEffect) {
            if(statusEffect.Type == StatusEffect.StatusEffectType.Adder) {
                statsMultiplier[statusEffect.StatusType] += statusEffect.Value;
            } else {
                statsMultiplier[statusEffect.StatusType] *= statusEffect.Value;
            }
        }
    }
}
