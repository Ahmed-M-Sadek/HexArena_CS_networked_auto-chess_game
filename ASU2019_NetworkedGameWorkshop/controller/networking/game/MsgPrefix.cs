using System;

namespace ASU2019_NetworkedGameWorkshop.controller.networking.game
{
    public enum NetworkMsgPrefix
    {
        [Prefix("NChar")]
        NewCharacter,
        [Prefix("SChar")]
        CharacterSwap,
        [Prefix("PlayerHP")]
        PlayerHealthUpdate,
        [Prefix("NPlayer")]
        NewPlayer,
        [Prefix("SLChar")]
        SellCharacter,
        [Prefix("LUChar")]
        LevelUpCharacter,
        [Prefix("LUSpell")]
        LevelUpSpell,
        [Prefix("LSpell")]
        LearnSpell,
        [Prefix("DefSkill")]
        DefaultSkill,
        [Prefix("AddActSkill")]
        AddActiveSpells,
        [Prefix("RemActSkill")]
        RemActiveSpells,
        [Prefix("ExchActSkill")]
        ExchActiveSpells
    }

    public static class NetworkMsgPrefixExtensions
    {
        public static string getPrefix(this NetworkMsgPrefix val)
        {
            PrefixAttribute[] attributes = (PrefixAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(PrefixAttribute), false);
            return attributes[0].Prefix;
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class PrefixAttribute : Attribute
    {
        public PrefixAttribute(string prefix)
        {
            Prefix = prefix;
        }

        public string Prefix { get; }
    }
}
