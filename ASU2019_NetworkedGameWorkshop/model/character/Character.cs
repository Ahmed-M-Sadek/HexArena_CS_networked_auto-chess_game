using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.model.character.types;
using ASU2019_NetworkedGameWorkshop.model.grid;
using ASU2019_NetworkedGameWorkshop.model.spell;
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
        public bool SpellReady { get; set; }
        public Character CurrentTarget { get; private set; }
        public Dictionary<StatusType, int> Stats { get; private set; }

        public readonly Teams team;
        public readonly GameManager gameManager;
        public readonly Grid grid;

        private readonly StatBar hpBar, charageBar;
        private readonly Brush brush;
        private readonly CharacterType[] characterType;
        private readonly Dictionary<StatusType, int> statsAdder;
        private readonly Dictionary<StatusType, float> statsMultiplier;
        private readonly List<Spells> spells;

        private List<StatusEffect> statusEffects;
        private Tile toMoveTo;
        private long nextAtttackTime;
        private ChooseSpell chooseSpell;


        public Character(Grid grid, Tile currentTile, Teams team,
            CharacterType[] characterType, GameManager gameManager)
        {
            this.grid = grid;
            currentTile.CurrentCharacter = this;
            this.team = team;
            SpellReady = false;
            this.characterType = characterType;
            this.gameManager = gameManager;
            brush = team == Teams.Blue ? Brushes.BlueViolet : Brushes.Red;

            Stats = CharacterType.statsCopy();
            spells = new List<Spells>();
            statsMultiplier = new Dictionary<StatusType, float>();
            statsAdder = new Dictionary<StatusType, int>();
            foreach (StatusType statusType in Enum.GetValues(typeof(StatusType)))
            {
                statsAdder.Add(statusType, 0);
                statsMultiplier.Add(statusType, 1f);
            }

            statusEffects = new List<StatusEffect>();

            IsDead = false;

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
            Stats[StatusType.HealthPoints] = Math.Min(Stats[StatusType.HealthPoints] + healValue,
                                                        Stats[StatusType.HealthPointsMax]);
        }

        public void learnSpell(Spells spell)
        {
            spells.Add(spell);
        }

        public void takeDamage(float dmgValue, DamageType damageType)
        {

            if (dmgValue < 0)
            {
                throw new ArgumentException("dmgValue should be positive: " + dmgValue);
            }
            Stats[StatusType.HealthPoints] -= (int)(dmgValue * 100 /
                (100 + (damageType == DamageType.MagicDamage ? Stats[StatusType.Armor] : Stats[StatusType.MagicResist])));
            if (Stats[StatusType.HealthPoints] <= 0)
            {
                Stats[StatusType.HealthPoints] = 0;
                IsDead = true;
                if (SpellReady == true)
                {
                    gameManager.removeRangeFromForm(chooseSpell);
                    SpellReady = false;
                }
                if (CurrentTile != null)
                {
                    //CurrentTile.Walkable = true;
                    //CurrentTile.CurrentCharacter = null;
                    //CurrentTile = null;
                    //causes an excepetion in path finding 
                }
            }
            else
            {
                Stats[StatusType.Charge] = Math.Min(Stats[StatusType.Charge] + 10, Stats[StatusType.ChargeMax]);//temp value
            }

        }

        public void reset()
        {
            Stats = CharacterType.statsCopy();
            statusEffects.Clear();
            IsDead = false;
            toMoveTo = null;
            CurrentTarget = null;
        }

        public void addStatusEffect(StatusEffect statusEffect)
        {
            statusEffect.RemoveEffectTimeStamp += gameManager.ElapsedTime;
            applyStatusEffect(statusEffect);
            statusEffects.Add(statusEffect);
        }

        public override void draw(Graphics graphics)
        {
            graphics.FillRectangle(brush,
                CurrentTile.centerX - CharacterType.WIDTH_HALF,
                CurrentTile.centerY - CharacterType.HEIGHT_HALF,
                CharacterType.WIDTH, CharacterType.HEIGHT);

            hpBar.setTrackedAndDraw(graphics, Stats[StatusType.HealthPoints], Stats[StatusType.HealthPointsMax]);
            charageBar.setTrackedAndDraw(graphics, Stats[StatusType.Charge], Stats[StatusType.ChargeMax]);
        }
        public void resetMana()
        {
            Stats[StatusType.Charge] = 0;
        }

        public bool tick()
        {
            if (toMoveTo != null)
            {
                CurrentTile.CurrentCharacter = null;
                CurrentTile.Walkable = true;
                toMoveTo.CurrentCharacter = this;
                toMoveTo = null;

                return true;
            }
            return false;
        }

        public bool update()
        {
            statusEffects = statusEffects.Where(effect =>
            {
                if (effect.RemoveEffectTimeStamp < gameManager.ElapsedTime)
                {
                    foreach (StatusEffect item in statusEffects)
                    {
                        if (statusEffects.IndexOf(effect) < statusEffects.IndexOf(item))
                        {
                            item.inverseValue();
                            applyStatusEffect(item);
                            effect.inverseValue();
                            applyStatusEffect(effect);
                            item.inverseValue();
                            applyStatusEffect(item);
                            return false;
                        }
                    }
                    effect.inverseValue();
                    applyStatusEffect(effect);
                    return false;
                }
                return true;
            }).ToList();

            if (Stats[StatusType.Charge] == Stats[StatusType.ChargeMax]
                && spells.Count != 0
                && !SpellReady)
            {
                chooseSpell = new ChooseSpell(this, spells);
                SpellReady = true;
                gameManager.addRangeToForm(chooseSpell);
            }

            if (toMoveTo == null)
            {
                List<Tile> path = null;
                if (CurrentTarget == null
                    || CurrentTarget.IsDead)
                {
                    try
                    {

                        (path, CurrentTarget) = PathFinding.findPathToClosestEnemy(CurrentTile, team, grid, gameManager);//temp
                    }
                    catch (PathFinding.PathNotFoundException)
                    {
                        return false;
                    }
                }

                if (PathFinding.getDistance(CurrentTile, CurrentTarget.CurrentTile) <= Stats[StatusType.Range])
                {

                    if (gameManager.ElapsedTime > nextAtttackTime)
                    {
                        nextAtttackTime = gameManager.ElapsedTime + Stats[StatusType.AttackSpeed];
                        CurrentTarget.takeDamage(Stats[StatusType.AttackDamage], DamageType.PhysicalDamage);//temp DamageType?
                        return true;
                    }
                }
                else
                {
                    if (path == null)
                    {
                        try
                        {
                            path = PathFinding.findPath(CurrentTile, CurrentTarget.CurrentTile, grid, (Tile[,])grid.Tiles.Clone());
                        }
                        catch (PathFinding.PathNotFoundException)
                        {
                            return false;
                        }
                    }
                    toMoveTo = path[0];
                }
            }
            return false;
        }

        private void levelUp()
        {
            if (CurrentLevel < CharacterType.MAX_CHAR_LVL)
            {
                CurrentLevel++;
                Stats = CharacterType.statsCopy();
            }
        }
        private void applyStatusEffect(StatusEffect statusEffect)
        {
            if (statusEffect.Type == StatusEffectType.Adder)
            {
                statsAdder[statusEffect.StatusType] += (int)statusEffect.Value;
                Stats[statusEffect.StatusType] = (int)Math.Round(Stats[statusEffect.StatusType] + statusEffect.Value);
            }
            else
            {
                statsMultiplier[statusEffect.StatusType] *= statusEffect.Value;

                Stats[statusEffect.StatusType] = (int)Math.Round(Stats[statusEffect.StatusType] * statusEffect.Value);
            }

        }
    }
}
