namespace ASU2019_NetworkedGameWorkshop.model.character
{
    public struct StatusEffect
    {
        public enum StatusEffectType { Adder, Multiplier }

        public StatusType StatusType { get; private set; }
        public float Value { get; private set; }
        public long RemoveEffectTimeStamp { get; set; }
        public StatusEffectType Type { get; private set; }

        public StatusEffect(StatusType statusType, float value, long removeEffectTimeStamp, StatusEffectType type)
        {
            StatusType = statusType;
            Value = value;
            RemoveEffectTimeStamp = removeEffectTimeStamp;
            Type = type;
        }

        public void inverseValue()
        {
            Value = (Type == StatusEffectType.Adder) ? (Value * -1) : (1 / Value);
        }
    }
}
