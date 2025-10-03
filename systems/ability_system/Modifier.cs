using System.Collections;
using System.Collections.Generic;
using framework.systems.tag;

namespace framework.systems.ability_system
{
    public enum ModifierOperation
    {
        Add,
        Multiply,
        Override
    }

    public class Modifier
    {
        public Tag Tag { get; private set; }
        public ModifierOperation Operation { get; private set; }
        public float Value { get; private set; } = 0f;

        public Modifier(Tag tag, ModifierOperation operation, float value)
        {
            Tag = tag;
            Operation = operation;
            Value = value;
        }
    }
}
