using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.spell.types;
using System;
using System.Collections.Generic;

namespace ASU2019_NetworkedGameWorkshop.model.spell
{
    public class Spells
    {
        public static readonly Spells AwesomeFireball = new Spells(200,
            new Target(false, PriorityType.Current),
            SpellType.Damage,
            "../../assets/sprites/spells/fireball-red-1.png");

        public static readonly Spells AwesomeFireballAOE = new Spells(200,
            new Target(false, true, 3, PriorityType.Current),
            SpellType.Damage,
            "../../assets/sprites/spells/fireball-acid-3.png");

        public static readonly Spells AwesomeFireballRandom = new Spells(200,
            new Target(false, PriorityType.Random),
            SpellType.Damage,
            "../../assets/sprites/spells/fireball-sky-1.png");

        public static readonly Spells Heal = new Spells(1000,
            new Target(false, PriorityType.Self),
            new SpellType(new StatusEffect(StatusType.HealthPoints, 1000, 2000, StatusEffect.StatusEffectType.Adder)),
            "../../assets/sprites/spells/heal-jade-1.png");

        public static readonly Spells VeigarDebug = new Spells(1.2f,
            new Target(false, PriorityType.Self),
            new SpellType(new StatusEffect(StatusType.HealthPoints, 1.2f, 1100, StatusEffect.StatusEffectType.Multiplier)),
            "../../assets/sprites/spells/fireball-sky-1.png");

        public static readonly Spells Execute = new Spells(200,
            new Target(false, PriorityType.LowHealth),
            SpellType.Damage,
            "../../assets/sprites/spells/fireball-red-1.png");



        public static IEnumerable<Spells> Values
        {
            get
            {
                yield return AwesomeFireball;
                yield return AwesomeFireballAOE;
                yield return AwesomeFireballRandom;

            }
        }
        public float AbilityValue { get; private set; }

        public Target Target { get; private set; }
        public SpellType spellType { get; private set; }
        public String image { get; private set; }
        public Spells(float abilityValue, Target target, SpellType spellType, String image)
        {

            AbilityValue = abilityValue;
            this.Target = target;
            this.spellType = spellType;
            this.image = (image);
        }

        private List<Character> specifyTargets(Character caster)
        {
            this.Target.caster = caster;
            return Target.getTargets();
        }
        public void castSpell(Character caster)
        {
            spellType.cast(specifyTargets(caster), AbilityValue);
        }

    }
}
