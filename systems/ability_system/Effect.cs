using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using framework.systems.ability_system.Internal;
using framework.systems.tag;

namespace framework.systems.ability_system
{
    public partial class Effect : Node
    {
        // ----- INTERNAL -----
        // --------------------
        public static string __typeTagFieldName = "__typeTag"; // NOTE: Internal, do not edit
        public static Tag __typeTag = null; // Override in child classes with the new keyword
        // ------------------------
        // ----- END INTERNAL -----

        // ----- SETTINGS -----
        // --------------------
        /// <summary> A unique tag for this ability's class type </summary>
        public Tag TypeTag { get; private set; } = null;
        /// <summary> Tags that are granted to the owning MASComponent while this effect is applied </summary>
        public List<Tag> GrantedTags { get; } = new();
        public int MaxStacks { get; protected set; } = 1; // -1 for infinite
        protected readonly float Duration = 0f;
        // ------------------------
        // ----- END SETTINGS -----

        // ----- INSTANCE -----
        // --------------------
        public int Stacks { get; private set; } = 0;

        protected AbilityComponent AbilityComponent;
        protected float TimeOfApplied = 0f;
        // ------------------------
        // ----- END INSTANCE -----
        
        public Effect(Tag typeTag)
        {
            TypeTag = typeTag;
        }
        public void PostConstruct()
        {

        }
        
        public void Added(AbilityComponent abilityComponent)
        {
            AbilityComponent = abilityComponent;
            TimeOfApplied = GetTime();

            OnAdded();
        }

        protected virtual void OnAdded()
        {

        }
        
        public float GetTime()
        {
            return (float)(Time.GetTicksUsec() / 1000000.0);
        }

        public void Tick(double deltaTime)
        {
            OnActiveTick(deltaTime);

            if (GetDurationRemaining() < 0f)
            {
                AbilityComponent.RemoveEffect(TypeTag);
            }
        }

        protected virtual void OnActiveTick(double deltaTime)
        {

        }

        public void Removed()
        {

        }

        protected virtual void OnRemoved()
        {

        }

        public virtual int AddStacks(int stacks)
        {
            if (stacks <= 0)
            {
                return 0;
            }

            int oldStacks = Stacks;
            Stacks = Mathf.Clamp(Stacks + stacks, 0, MaxStacks == -1 ? int.MaxValue : MaxStacks);

            return Stacks - oldStacks;
        }

        public virtual int RemoveStacks(int stacks)
        {
            if (stacks <= 0)
            {
                return 0;
            }

            int oldStacks = Stacks;
            Stacks = Mathf.Clamp(Stacks - stacks, 0, MaxStacks == -1 ? int.MaxValue : MaxStacks);

            return oldStacks - Stacks;
        }

        public float GetDuration()
        {
            return Duration;
        }

        public float GetDurationRemaining()
        {
            return GetDuration() - (GetTime() - TimeOfApplied);
        }

        public static Effect Create(Tag typeTag)
        {
            Type effectType = AbilityReflector.GetRegisteredEffectType(typeTag);
            if (effectType != null)
            {
                if (Activator.CreateInstance(effectType, typeTag) is Effect effectInstance)
                {
                    effectInstance.PostConstruct();
                    return effectInstance;
                }
                else
                {
                    GD.PrintErr($"Failed to create instance of {nameof(Effect)} because created instance was null");
                    return null;
                }
            }
            else
            {
                GD.PrintErr($"Failed to create instance of {nameof(Effect)} because type was null");
                return null;
            }
        }
    }
}
