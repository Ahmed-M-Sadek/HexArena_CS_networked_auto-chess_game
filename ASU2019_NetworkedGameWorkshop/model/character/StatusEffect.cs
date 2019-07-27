namespace ASU2019_NetworkedGameWorkshop.model.character
{
    public struct StatusEffect
    {
        public enum StatusEffectType { Adder, Multiplier }

        public StatusType StatusType { get; private set; }
        public float Value { get; private set; }

        public long removeEffectTimeStamp { get; private set; }
        public StatusEffectType Type { get; private set; }
        public StatusEffect(StatusType statusType, float value, long removeEffectTimeStamp, StatusEffectType type)
        {
            StatusType = statusType;
            Value = value;
            this.removeEffectTimeStamp = removeEffectTimeStamp;
            Type = type;
        }

        public void inverseValue()
        {
            if (Type == StatusEffectType.Adder)
            {
                Value *= -1;
            }
            else
            {
                Value = 1 / Value;
            }
        }
    }
}
