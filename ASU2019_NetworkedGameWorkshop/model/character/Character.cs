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
        private static readonly Font DEBUG_FONT = new Font("Roboto", 10f);

        public readonly Teams team;
        public readonly GameManager gameManager;
        public readonly Grid grid;

        private readonly StatBar hpBar, charageBar;
        private readonly Brush brush;
        private readonly CharacterType[] characterType;
        private readonly Dictionary<StatusType, int> statsAdder;
        private readonly Dictionary<StatusType, float> statsMultiplier;
        public List<Spells> LearnedSpells { get; }


        private bool spellsUIVisible = false;
        private List<StatusEffect> statusEffects;
        private long nextAtttackTime;

        public ChooseSpell ChooseSpell { get; set; }
        public InactiveSpell InactiveSpell { get; set; }
        public List<Spells> ActiveSpells { get; set; }
        public List<Spells> InactiveSpells { get; set; }
        public bool SpellReady { get; set; }
        public Dictionary<StatusType, int> Stats { get; private set; }
        public Tile CurrentTile { get; set; }//public set ?
        public Character CurrentTarget { get; private set; }

        /// <summary>
        /// CharacterType according to the Character's current level.
        /// </summary>
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
            SpellReady = false;
            this.characterType = characterType;
            this.gameManager = gameManager;

            Stats = CharacterType.statsCopy();
            ActiveSpells = new List<Spells>();
            InactiveSpells = new List<Spells>();
            LearnedSpells = new List<Spells>();
            brush = (team == Teams.Blue) ? Brushes.BlueViolet : Brushes.Red;
            statusEffects = new List<StatusEffect>();
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

        /// <summary>
        /// Increases the character's Health Points by healValue after applying modifiers.
        /// 
        /// <para>Can NOT increases the Character's Health Points above the Character's type max Health Points.</para>
        /// </summary>
        /// <param name="healValue">the amount the character should heal.</param>
        /// <exception cref="ArgumentException">if the healValue is Negative.</exception>
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
            LearnedSpells.Add(spell);
            InactiveSpells.Add(spell);
        }

        /// <summary>
        /// Decreases the character's Health Points by healValue after applying modifiers.
        /// </summary>
        /// <param name="dmgValue">The damage the character took.</param>
        /// <param name="damageType">The type of damage the Character took.</param>
        /// <exception cref="ArgumentException">if the dmgValue is Negative.</exception>
        public void takeDamage(int dmgValue, DamageType damageType)
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
                    hideSpellUI();
                }

                CurrentTile.CurrentCharacter = null;
                CurrentTile = null; //why?
                if (ToMoveTo != null)
                    ToMoveTo.Walkable = true;
            }
            else
            {
                Stats[StatusType.Charge] = Math.Min(Stats[StatusType.Charge] + 10, Stats[StatusType.ChargeMax]);//temp value
            }

        }

        private void hideSpellUI()
        {
            gameManager.removeRangeFromForm(ChooseSpell);
            SpellReady = false;
        }

        public void reset()
        {
            Stats = CharacterType.statsCopy();
            statusEffects.Clear();
            IsDead = false;
            CurrentTarget = null;
            ToMoveTo = null;
            hideSpellUI();
        }

        public void addStatusEffect(StatusEffect statusEffect)
        {
            statusEffect.RemoveEffectTimeStamp += gameManager.ElapsedTime;
            applyStatusEffect(statusEffect);
            statusEffects.Add(statusEffect);
        }


        public void resetMana()
        {
            Stats[StatusType.Charge] = 0;
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
        public bool updateBuy()
        {
            if (gameManager.CurrentGameStage == StageManager.GameStage.Buy && !spellsUIVisible && this.CurrentTile == gameManager.SelectedTile)
            {
                InactiveSpell = new InactiveSpell(this, InactiveSpells);
                ChooseSpell = new ChooseSpell(this, ActiveSpells);
                gameManager.addRangeToForm(InactiveSpell, ChooseSpell);
                spellsUIVisible = true;
                return true;
            }
            else if (this.CurrentTile != gameManager.SelectedTile)
            {
                gameManager.removeRangeFromForm(InactiveSpell, ChooseSpell);
                foreach (Spells spell in ActiveSpells)
                {
                    InactiveSpells.Remove(spell);
                }
                spellsUIVisible = false;
                return true;
            }
            return false;
        }
        public bool update()
        {
            if (gameManager.CurrentGameStage != StageManager.GameStage.Buy && spellsUIVisible)
            {
                gameManager.removeRangeFromForm(InactiveSpell, ChooseSpell);
                spellsUIVisible = false;
                return true;
            }
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
                && ActiveSpells.Count != 0
                && !SpellReady)
            {
                ChooseSpell = new ChooseSpell(this, ActiveSpells);
                SpellReady = true;
                gameManager.addRangeToForm(ChooseSpell);
            }

            if (ToMoveTo == null)
            {
                if (CurrentTarget == null
                    || CurrentTarget.IsDead
                    || PathFinding.getDistance(CurrentTile, CurrentTarget.CurrentTile) > Stats[StatusType.Range])
                {
                    List<Tile> path = null;
                    try
                    {
                        (path, CurrentTarget) = PathFinding.findPathToClosestEnemy(CurrentTile, team, grid, gameManager);//temp

                    }
                    catch (PathFinding.PathNotFoundException)
                    {
                        return false;
                    }
                    if (PathFinding.getDistance(CurrentTile, CurrentTarget.CurrentTile) > Stats[StatusType.Range])
                    {
                        ToMoveTo = path[0];
                        ToMoveTo.Walkable = false;
                    }
                }
                else
                {
                    if (gameManager.ElapsedTime > nextAtttackTime)
                    {
                        nextAtttackTime = gameManager.ElapsedTime + Stats[StatusType.AttackSpeed];
                        CurrentTarget.takeDamage(Stats[StatusType.AttackDamage], DamageType.PhysicalDamage);//temp DamageType?
                        return true;
                    }
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
        public override void draw(Graphics graphics)
        {

            if (!IsDead)
            {
                graphics.FillRectangle(brush,
                    CurrentTile.centerX - CharacterType.WIDTH_HALF,
                    CurrentTile.centerY - CharacterType.HEIGHT_HALF,
                    CharacterType.WIDTH, CharacterType.HEIGHT);

                hpBar.updateTrackedAndDraw(graphics, Stats[StatusType.HealthPoints], Stats[StatusType.HealthPointsMax]);
                charageBar.updateTrackedAndDraw(graphics, Stats[StatusType.Charge], Stats[StatusType.ChargeMax]);
            }
        }

        /// <summary>
        /// Draws a string containing the Characters Classes on the character.
        /// 
        /// <para>Calls the DrawDebug() of the Character's Statbars.</para>
        /// </summary>
        /// <param name="graphics">graphics object to draw on.</param>
        public override void drawDebug(Graphics graphics)
        {
            if (!IsDead)
            {
                graphics.DrawString(CharacterType.ToString(),
                DEBUG_FONT, Brushes.White,
                CurrentTile.centerX - CharacterType.WIDTH_HALF,
                CurrentTile.centerY);

                hpBar.drawDebug(graphics);
                charageBar.drawDebug(graphics);
            }
        }
    }
}
