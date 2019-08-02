using ASU2019_NetworkedGameWorkshop.model.character;
using ASU2019_NetworkedGameWorkshop.model.spell.types;
using ASU2019_NetworkedGameWorkshop.Properties;
using System.Collections.Generic;
using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model.spell
{
    public class Spells
    {
        private readonly static Dictionary<int, Spells[]> spellsList;
        public int Id { get; set; }

        public static int ID = 0;

        public static Spells[] getSpell(int id)
        {
            return spellsList[id];
        }


        public static readonly Spells[] AwesomeFireball = new Spells[]{
            new Spells(
                100,
                new Target(false, CastTarget.CurrentTarget),
                SpellType.Damage,
                Resources.fireball_red_1,
                "AwesomeFireball"),
            new Spells(
                200,
                new Target(false, CastTarget.CurrentTarget),
                SpellType.Damage,
                Resources.fireball_red_2,
                "AwesomeFireball"),
            new Spells(
                300,
                new Target(false, CastTarget.CurrentTarget),
                SpellType.Damage,
                Resources.fireball_red_3,
                "AwesomeFireball")
            };

        public static readonly Spells[] AwesomeFireballAOE = new Spells[]{
            new Spells(
                200,
                new Target(false, true, 3, CastTarget.CurrentTarget),
                SpellType.Damage,
                Resources.fireball_acid_3,
                "AwesomeFireballAOE") };

        public static readonly Spells[] AwesomeFireballRandom = new Spells[]{
            new Spells(
                200,
                new Target(false, CastTarget.Random),
                SpellType.Damage,
                Resources.fireball_eerie_2,
                "AwesomeFireballRandom") };

        public static readonly Spells[] Heal = new Spells[]{
            new Spells(
                1000,
                new Target(false, CastTarget.Self),
                SpellType.Heal,
                Resources.heal_jade_1,
                "Heal") };

        public static readonly Spells[] VeigarDebug = new Spells[]{
            new Spells(
                0,
                new Target(false, CastTarget.Self),
                new SpellType(new StatusEffect(StatusType.HealthPoints, 1.2f, 1100, StatusEffect.StatusEffectType.Multiplier)),
                Resources.fireball_sky_1,
                "VeigarDebug") };

        public static readonly Spells[] Execute = new Spells[]{
            new Spells(
                200,
                new Target(false, CastTarget.LowHealth),
                SpellType.Damage,
                Resources.enchant_acid_1,
                "Execute") };

        public static IEnumerable<Spells[]> Values
        {
            get
            {
                yield return AwesomeFireball;
                yield return AwesomeFireballAOE;
                yield return AwesomeFireballRandom;
                yield return Heal;
                yield return VeigarDebug;
                yield return Execute;
            }
        }

        public int AbilityValue { get; private set; }
        public Target Target { get; private set; }
        public SpellType SpellType { get; private set; }
        public Image Image { get; private set; }
        public string Name { get; private set; }

        public Spells(int abilityValue, Target target, SpellType spellType, Image image, string spellName)
        {
            AbilityValue = abilityValue;
            Target = target;
            SpellType = spellType;
            Image = image;
            Name = spellName;
        }
        public void castSpell(Character caster)
        {
            Target.SpellType = SpellType;
            Target.Caster = caster;
            Target.getTargetAndCast(AbilityValue);
        }

        public override string ToString()
        {
            return $"{Name}\nType -> {SpellType.Name} : {AbilityValue}";
        }
    }
}
