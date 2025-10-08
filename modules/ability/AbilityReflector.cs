using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using framework.modules.tag;
using Godot;

namespace framework.modules.ability.Internal
{
    public static class AbilityReflector
    {
        private static readonly Dictionary<Tag, Type> RegisteredAbilities = new();
        private static readonly Dictionary<Tag, Type> RegisteredEffects = new();
        
        static AbilityReflector()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(Ability))
                        && !type.IsAbstract) // Ignore abstract ability classes since we don't want to register them
                    {
                        var tagFieldInfo = type.GetField(Ability.__typeTagFieldName);
                        var abilityTypeTag = (Tag)tagFieldInfo!.GetValue(null);
                        if (abilityTypeTag != null)
                        {
                            RegisteredAbilities.Add(abilityTypeTag, type);

                            continue;
                        }
                        else
                        {
                            GD.PrintErr($"Failed to register {nameof(Ability)} because field {Ability.__typeTagFieldName} wasn't overridden");
                            continue;
                        }
                    }

                    if (type.IsSubclassOf(typeof(Effect))
                        && !type.IsAbstract)
                    {
                        FieldInfo tagFieldInfo = type.GetField(Effect.__typeTagFieldName);
                        Tag abilityTypeTag = (Tag)tagFieldInfo!.GetValue(null);
                        if (abilityTypeTag != null)
                        {
                            RegisteredEffects.Add(abilityTypeTag, type);

                            continue;
                        }
                        else
                        {
                            GD.PrintErr($"Failed to register {nameof(Effect)} because field {Effect.__typeTagFieldName} wasn't overridden");
                            continue;
                        }
                    }
                }
            }
        }
        
        public static Type GetRegisteredAbilityType(Tag tag)
        {
            if (RegisteredAbilities.TryGetValue(tag, out var type))
            {
                return type;
            }

            GD.PrintErr($"Failed to get registered {nameof(Ability)} type from tag {tag.Key}");
            return null;
        }

        public static Type GetRegisteredEffectType(Tag tag)
        {
            if (RegisteredEffects.TryGetValue(tag, out var type))
            {
                return type;
            }

            GD.PrintErr($"Failed to get registered {nameof(Effect)} type from tag {tag.Key}");
            return null;
        }
    }
}
