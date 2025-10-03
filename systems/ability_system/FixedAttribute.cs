using Godot;
using System;
using framework.systems.tag;

namespace framework.systems.ability_system
{
    /// <summary> Fixed attributes cannot undo any modifiers that are applied. Best use cases for fixed attributes are values that have their base changed often </summary>
    public class FixedAttribute
    {
        public readonly Action<float, float> OnValueChanged = delegate { };

        public Tag Tag { get; private set; }
        public float Value { get; private set; }

        public FixedAttribute(Tag tag, float value)
        {
            Tag = tag;
            Value = value;
        }


        /// <returns> The value after being altered </returns>
        public float AlterValue(float delta, float clampMin = float.MinValue, float clampMax = float.MaxValue)
        {
            var oldValue = Value;
            Value = Mathf.Clamp(Value + delta, clampMin, clampMax);
            OnValueChanged.Invoke(oldValue, Value);

            return Value;
        }

        public void SetValue(float value)
        {
            var oldValue = Value;
            Value = value;
            OnValueChanged.Invoke(oldValue, Value);
        }

        /// <summary> Applies a modifier a single time. This cannot be undone or removed, it permanently alters the value of this attribute </summary>
        public bool BakeModifier(Modifier modifier)
        {
            var oldValue = Value;

            switch (modifier.Operation)
            {
                case ModifierOperation.Add:
                    Value += modifier.Value;
                    break;
                case ModifierOperation.Multiply:
                    Value *= modifier.Value;
                    break;
                case ModifierOperation.Override:
                    Value = modifier.Value;
                    break;
                default:
                    // Invalid operation
                    return false;
            }

            OnValueChanged.Invoke(oldValue, Value);
            return true;
        }
    }
}
