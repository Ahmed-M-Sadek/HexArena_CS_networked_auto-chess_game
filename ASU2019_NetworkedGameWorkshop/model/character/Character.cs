using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.model.character.types;
using ASU2019_NetworkedGameWorkshop.model.grid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static ASU2019_NetworkedGameWorkshop.model.character.StatusEffect;

namespace ASU2019_NetworkedGameWorkshop.model.character
{
    public class Character : GraphicsObject
    {
        public enum Teams { Red, Blue };

        public readonly Teams team;

        private readonly Grid grid;
        private readonly StatBar hpBar, charageBar;
        private readonly GameManager gameManager;
        private readonly Brush brush;
        private readonly CharacterType[] characterType;
        private readonly Dictionary<StatusType, int> statsAdder;
        private readonly Dictionary<StatusType, float> statsMultiplier;

        private Dictionary<StatusType, int> stats;
        private List<StatusEffect> statusEffects;
        private Character currentTarget;
        private long nextAtttackTime;

        public Tile CurrentTile { get; set; }//public set ?
        public CharacterType CharacterType
        {
            get
            {
                return characterType[CurrentLevel];
            }
        }
        public bool IsDead { get; private set; }
        public int CurrentLevel { get; private set; }
        public Tile ToMoveTo { get; private set; }

        public Character(Grid grid,
                         Tile currentTile,
                         Teams team,
                         CharacterType[] characterType,
                         GameManager gameManager)
        {
            this.grid = grid;
            currentTile.CurrentCharacter = this;
            this.team = team;
            this.characterType = characterType;
            this.gameManager = gameManager;

            brush = (team == Teams.Blue) ? Brushes.BlueViolet : Brushes.Red;
            statusEffects = new List<StatusEffect>();
            stats = CharacterType.statsCopy();
            IsDead = false;

            statsMultiplier = new Dictionary<StatusType, float>();
            statsAdder = new Dictionary<StatusType, int>();
            foreach (StatusType statusType in Enum.GetValues(typeof(StatusType)))
            {
                statsAdder.Add(statusType, 0);
                statsMultiplier.Add(statusType, 1f);
            }

            hpBar = new StatBar(this,
                team == Teams.Blue ? Brushes.GreenYellow : Brushes.OrangeRed, 0);
            charageBar = new StatBar(this, Brushes.Blue, 1);
        }


        public void healHealthPoints(int healValue)
        {
            if (healValue < 0)
            {
                throw new ArgumentException("healValue should be positive: " + healValue);
            }
            stats[StatusType.HealthPoints] = Math.Min(stats[StatusType.HealthPoints] + healValue,
                                                        stats[StatusType.HealthPointsMax]);
        }

        public void takeDamage(int dmgValue, DamageType damageType)
        {
            if (dmgValue < 0)
            {
                throw new ArgumentException("dmgValue should be positive: " + dmgValue);
            }
            stats[StatusType.HealthPoints] -= (int)(dmgValue * 100 /
                (100 + (damageType == DamageType.MagicDamage ? stats[StatusType.Armor] : stats[StatusType.MagicResist])));
            if (stats[StatusType.HealthPoints] <= 0)
            {
                stats[StatusType.HealthPoints] = 0;
                IsDead = true;

                CurrentTile.CurrentCharacter = null;
                CurrentTile = null; //why?
                if (ToMoveTo != null)
                    ToMoveTo.Walkable = true;
            }
            else
            {
                stats[StatusType.Charge] = Math.Min(stats[StatusType.Charge] + 10, stats[StatusType.ChargeMax]);//temp value
            }
        }

        public void reset()
        {
            stats = CharacterType.statsCopy();
            statusEffects.Clear();
            IsDead = false;
            ToMoveTo = null;
            currentTarget = null;
        }

        public void addStatusEffect(StatusEffect statusEffect)
        {
            applyStatusEffect(statusEffect);
            statusEffects.Add(statusEffect);
        }

        public bool tick()
        {
            if (ToMoveTo != null)
            {
                CurrentTile.CurrentCharacter = null;
                CurrentTile.Walkable = true;
                ToMoveTo.CurrentCharacter = this;
                ToMoveTo = null;

                return true;
            }
            return false;
        }

        public bool update()
        {
            statusEffects = statusEffects.Where(effect =>
            {
                if (effect.removeEffectTimeStamp < gameManager.ElapsedTime)
                {
                    effect.inverseValue();
                    applyStatusEffect(effect);
                    return false;
                }
                return true;
            }).ToList();

            if (ToMoveTo == null)
            {
                List<Tile> path = null;
                if (currentTarget == null
                    || currentTarget.IsDead)
                {
                    try
                    {
                        (path, currentTarget) = PathFinding.findPathToClosestEnemy(CurrentTile, team, grid, gameManager);//temp
                    }
                    catch (PathFinding.PathNotFoundException)
                    {
                        return false;
                    }
                }
                if (PathFinding.getDistance(CurrentTile, currentTarget.CurrentTile) <= stats[StatusType.Range])
                {
                    if (gameManager.ElapsedTime > nextAtttackTime)
                    {
                        nextAtttackTime = gameManager.ElapsedTime + stats[StatusType.AttackSpeed];
                        currentTarget.takeDamage(stats[StatusType.AttackDamage], DamageType.PhysicalDamage);//temp DamageType?
                        return true;
                    }
                }
                else
                {
                    if (path == null)
                    {
                        try
                        {
                            (path, currentTarget) = PathFinding.findPathToClosestEnemy(CurrentTile, team, grid, gameManager);
                        }
                        catch (PathFinding.PathNotFoundException)
                        {
                            return false;
                        }
                    }
                    ToMoveTo = path[0];
                    ToMoveTo.Walkable = false;
                }
            }
            return false;
        }

        private void levelUp()
        {
            if (CurrentLevel < CharacterType.MAX_CHAR_LVL)
            {
                CurrentLevel++;
                stats = CharacterType.statsCopy();
            }
        }
        private void applyStatusEffect(StatusEffect statusEffect)
        {
            if (statusEffect.Type == StatusEffectType.Adder)
            {
                statsMultiplier[statusEffect.StatusType] += statusEffect.Value;
            }
            else
            {
                statsMultiplier[statusEffect.StatusType] *= statusEffect.Value;
            }
        }
        public override void draw(Graphics graphics)
        {
            if (!IsDead)
            {
                graphics.FillRectangle(brush,
                    CurrentTile.centerX - CharacterType.WIDTH_HALF,
                    CurrentTile.centerY - CharacterType.HEIGHT_HALF,
                    CharacterType.WIDTH, CharacterType.HEIGHT);

                hpBar.setTrackedAndDraw(graphics, stats[StatusType.HealthPoints], stats[StatusType.HealthPointsMax]);
                charageBar.setTrackedAndDraw(graphics, stats[StatusType.Charge], stats[StatusType.ChargeMax]);
            }
        }

        public override void drawDebug(Graphics graphics)
        {
        }
    }
}
