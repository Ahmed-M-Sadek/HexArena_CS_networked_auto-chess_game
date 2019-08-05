using ASU2019_NetworkedGameWorkshop.Properties;
using System.Collections.Generic;
using System.Drawing;

namespace ASU2019_NetworkedGameWorkshop.model.character.types
{
    public class CharacterTypePhysical : CharacterType
    {
        public static readonly CharacterTypePhysical[] Archer = new CharacterTypePhysical[]{
            new CharacterTypePhysical(
                "Archer",
                500,
                50, 100,
                2,
                90, 0.8f,
                25, 20,Resources.Archer1Blue,Resources.Archer1Red),
            new CharacterTypePhysical(
                "Archer",
                600,
                50, 100,
                2,
                110, 0.8f,
                30, 30,Resources.Archer2Blue,Resources.Archer2Red),
            new CharacterTypePhysical(
                "Archer",
                700,
                50, 100,
                2,
                125, 0.8f,
                40, 35,Resources.Archer3Blue,Resources.Archer3Red),
        };// low range ranged mid hp,  dmg

        public static readonly CharacterTypePhysical[] Assassin = new CharacterTypePhysical[]{
            new CharacterTypePhysical(
                "Assassin",
                500,
                50, 100,
                1,
                120, 0.55f,
                25, 20,Resources.Assassin1Blue,Resources.Assassin1Red),
            new CharacterTypePhysical(
                "Assassin",
                600,
                50, 100,
                1,
                150, 0.55f,
                35, 30,Resources.Assassin2Blue,Resources.Assassin1Red),
            new CharacterTypePhysical(
                "Assassin",
                700,
                50, 100,
                1,
                190, 0.55f,
                45, 40,Resources.Assassin3Blue,Resources.Assassin1Red),
        };//low atk speed high dmg mid hp melee

        public static readonly CharacterTypePhysical[] Monk = new CharacterTypePhysical[]{
            new CharacterTypePhysical(
                "Monk",
                500,
                20, 100,
                1,
                60, 1.2f,
                35, 20,Resources.Monk1Blue,Resources.Monk1Red),
            new CharacterTypePhysical(
                "Monk",
                600,
                30, 100,
                1,
                70, 1.2f,
                40, 30,Resources.Monk2Blue,Resources.Monk2Red),
            new CharacterTypePhysical(
                "Monk",
                700,
                40, 100,
                1,
                80, 0.7f,
                45, 40,Resources.Monk3Blue,Resources.Monk3Red),
        };//high atk speed mid dmg, hp

        public static readonly CharacterTypePhysical[] Ranger = new CharacterTypePhysical[]{
            new CharacterTypePhysical(
                "Ranger",
                500,
                50, 100,
                3,
                60, 0.75f,
                25, 20,Resources.Ranger1Blue,Resources.Ranger1Red),
            new CharacterTypePhysical(
                "Ranger",
                600,
                50, 100,
                3,
                70, 0.75f,
                25, 20,Resources.Ranger2Blue,Resources.Ranger2Red),
            new CharacterTypePhysical(
                "Ranger",
                700,
                50, 100,
                3,
                80, 0.75f,
                25, 20,Resources.Ranger3Blue,Resources.Ranger3Red),
        };//high atk speed mid hp mid range

        public static readonly CharacterTypePhysical[] Sentinal = new CharacterTypePhysical[]{
            new CharacterTypePhysical(
                "Sentinal",
                600,
                50, 100,
                1,
                50, 0.7f,
                50, 40,Resources.Sentinal1Blue,Resources.Sentinal1Red),
            new CharacterTypePhysical(
                "Sentinal",
                700,
                50, 100,
                1,
                70, 0.7f,
                60, 50,Resources.Sentinal2Blue,Resources.Sentinal2Red),
            new CharacterTypePhysical(
                "Sentinal",
                800,
                70, 100,
                1,
                80, 0.7f,
                70, 60,Resources.Sentinal3Blue,Resources.Sentinal3Red),
        };//mid hp below avg dmg high armour

        public static readonly CharacterTypePhysical[] Slime = new CharacterTypePhysical[]{
            new CharacterTypePhysical(
                "Slime",
                300,
                10,100,
                1,
                25, 1f,
                10, 10,Resources.Slime1Blue,Resources.Slime1Red),
            new CharacterTypePhysical(
                "Slime",
                400,
                10, 100,
                1,
                35, 1f,
                15, 15,Resources.Slime2Blue,Resources.Slime2Red),
            new CharacterTypePhysical(
                "Slime",
                800,
                50, 100,
                1,
                150, 1f,
                40, 40,Resources.Slime3Blue,Resources.Slime3Red),
        };//low hp, dmg, armor then explodes


        public static readonly CharacterTypePhysical[] Marksman = new CharacterTypePhysical[]{
            new CharacterTypePhysical(
                "Marksman",
                250,
                10, 100,
                4,
                150, 0.45f,
                15, 20,Resources.Marksman1Blue,Resources.Marksman1Red),
            new CharacterTypePhysical(
                "Marksman",
                350,
                20, 100,
                4,
                190, 0.45f,
                20, 25,Resources.Marksman2Blue,Resources.Marksman2Red),
            new CharacterTypePhysical(
                "Marksman",
                450,
                30, 100,
                4,
                230, 0.45f,
                25, 20,Resources.Marksman3Blue,Resources.Marksman3Red),
        };//long range low atk speed low hp high dmg

        public static readonly CharacterTypePhysical[] Warrior = new CharacterTypePhysical[]{
            new CharacterTypePhysical(
                "Warrior",
                700,
                20, 100,
                1,
                60, 0.7f,
                25, 20,Resources.Warrior1Blue,Resources.Warrior1Red),
            new CharacterTypePhysical(
                "Warrior",
                800,
                40, 100,
                1,
                80, 0.7f,
                35, 30,Resources.Warrior2Blue,Resources.Warrior2Red),
            new CharacterTypePhysical(
                "Warrior",
                900,
                50, 100,
                1,
                100, 0.7f,
                45, 40,Resources.Warrior3Blue,Resources.Warrior3Red),
        };//high hp mid dmg, atk speed

        private CharacterTypePhysical(
            string name,
            int healthPoints,
            int charge, int chargeMax,
            int range,
            int attackDamage, float attackSpeed,
            int armor, int magicResist, Bitmap imageBlue, Bitmap imageRed)
            : base(name, healthPoints, charge, chargeMax, range, attackDamage, attackSpeed, armor, magicResist, imageBlue, imageRed) { }

        /// <summary>
        /// All of the static instances of CharacterTypePhysical
        /// </summary>
        public new static IEnumerable<CharacterTypePhysical[]> Values
        {
            get
            {
                yield return Archer;
                yield return Assassin;
                yield return Monk;
                yield return Ranger;
                yield return Sentinal;
                yield return Slime;
                yield return Marksman;
                yield return Warrior;
            }
        }
    }
}
