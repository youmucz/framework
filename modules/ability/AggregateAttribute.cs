using System;
using Godot;
using System.Collections.Generic;
using framework.modules.tag;

namespace framework.modules.ability
{
    /// <summary> Aggregate attributes can add and remove modifiers as desired. Best use cases for aggregate attributes are values that don't change their base, but will be modifier and unmodifier often </summary>
    public class AggregateAttribute
    {
        public readonly Action<float, float> OnBaseValueChanged = delegate { };
        public readonly Action<float, float> OnCurrentValueChanged = delegate { };

        public Tag Tag { get; private set; }
        public float BaseValue { get; private set; }
        public float CurrentValue { get; private set; }

        private readonly Dictionary<Tag, Modifier> _modifiers = new();


        public AggregateAttribute(Tag tag, float value)
        {
            Tag = tag;
            BaseValue = value;
            CurrentValue = BaseValue;
        }


        public void SetBaseValue(float value)
        {
            if (Math.Abs(BaseValue - value) < float.Epsilon)
            {
                return;
            }

            var oldBaseValue = BaseValue;
            BaseValue = value;
            OnBaseValueChanged.Invoke(oldBaseValue, BaseValue);
            UpdateCurrentValue();
        }

        public bool ApplyModifier(Modifier modifier)
        {
            if (_modifiers.ContainsKey(modifier.Tag))
            {
                // This modifier has already been applied
                return false;
            }

            _modifiers.Add(modifier.Tag, modifier);
            UpdateCurrentValue();

            return true;
        }

        public bool RemoveModifier(Modifier modifier)
        {
            return RemoveModifierByTag(modifier.Tag);
        }

        public bool RemoveModifierByTag(Tag tag)
        {
            if (!_modifiers.ContainsKey(tag))
            {
                // There is no modifier with this tag
                return false;
            }

            _modifiers.Remove(tag);
            UpdateCurrentValue();

            return true;
        }

        private void UpdateCurrentValue()
        {
            var oldCurrentValue = CurrentValue;
            var newCurrentValueBase = BaseValue;
            var newCurrentValueMultiplier = 1f;
            Stack<float> newCurrentValueOverride = new();

            foreach (Modifier modifier in _modifiers.Values)
            {
                switch (modifier.Operation)
                {
                    case ModifierOperation.Add:
                        newCurrentValueBase += modifier.Value;
                        break;
                    case ModifierOperation.Multiply:
                        newCurrentValueMultiplier *= modifier.Value;
                        break;
                    case ModifierOperation.Override:
                        newCurrentValueOverride.Push(modifier.Value);
                        break;
                }
            }

            if (newCurrentValueOverride.Count > 0)
            {
                CurrentValue = newCurrentValueOverride.Pop();
            }
            else
            {
                CurrentValue = newCurrentValueBase * newCurrentValueMultiplier;
            }

            OnCurrentValueChanged.Invoke(oldCurrentValue, CurrentValue);
        }
    }
}
