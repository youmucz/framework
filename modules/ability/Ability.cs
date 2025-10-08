using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using framework.modules.ability.Internal;
using framework.modules.tag;

namespace framework.modules.ability
{
    [GlobalClass]
    public partial class Ability : Resource
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
        /// <summary> Tags that are granted to the owning MASComponent while this ability is active </summary>
        public List<Tag> GrantedTags { get; } = new();
        /// <summary> This ability cannot be activated if the owning MASComponent has any of these tags </summary>
        public List<Tag> BlockedByTags { get; } = new();
        /// <summary> When this ability is activated successfully, any active abilities on the owning MASComponent that matches one of these tags will be cancelled </summary>
        public List<Tag> CancelAbilityTags { get; } = new();
        /// <summary> Tags that, when granted to the owning MASComponent, will cancel this ability (only includes grantedLooseTags) </summary>
        public List<Tag> CancelledByGrantedLooseTags { get; } = new();
        /// <summary> The tag for the effect used to track this ability's cooldown </summary>
        public Tag CooldownEffectTag { get; protected set; } = null;
        // -----------------------
        // ----- END SETTINGS -----

        // ----- INSTANCE -----
        // --------------------
        public AbilityComponent AbilityComponent { get; private set; } = null;
        public bool Active { get; private set; } = false;
        protected object[] ActivationParams;
        // ------------------------
        // ----- END INSTANCE -----

        public Ability(Tag typeTag)
        {
            TypeTag = typeTag;
        }
        public void OnPostConstruct()
        {
            if (CooldownEffectTag != null)
            {
                BlockedByTags.Add(CooldownEffectTag);
            }
        }


        public void Tick(double deltaTime)
        {
            if (Active)
            {
                OnActiveTick(deltaTime);
            }
        }

        protected virtual void OnActiveTick(double deltaTime)
        {

        }

        public virtual bool CanActivate()
        {
            if (AbilityComponent == null)
            {
                return false;
            }

            if (AbilityComponent.HasAnyGrantedTags(BlockedByTags))
            {
                return false;
            }

            return true;
        }

        public void Activate(params object[] @params)
        {
            ActivationParams = @params;

            Active = true;

            List<Tag> cancelledAbilities = AbilityComponent.GetAllActiveAbilitiesWithTags(CancelAbilityTags);
            if (cancelledAbilities.Count > 0)
            {
                AbilityComponent.CancelAbilities(cancelledAbilities);
            }

            OnActivate();
        }

        protected virtual void OnActivate()
        {

        }

        public void End()
        {
            if (Active)
            {
                Active = false;

                OnEnd(false);
            }
        }

        public void Cancel()
        {
            if (Active)
            {
                Active = false;

                OnEnd(true);
            }
        }

        protected virtual void OnEnd(bool cancelled)
        {

        }

        protected void StartCooldown()
        {
            if (CooldownEffectTag != null)
            {
                AbilityComponent.AddEffect(Effect.Create(CooldownEffectTag));
            }
        }

        public void Added(AbilityComponent abilityComponent)
        {
            AbilityComponent = abilityComponent;

            OnAdded();
        }

        protected virtual void OnAdded()
        {

        }

        public void Removed(AbilityComponent abilityComponent)
        {
            AbilityComponent = null;

            Cancel();
            OnRemoved();
        }

        protected virtual void OnRemoved()
        {

        }

        public static Ability Create(Tag typeTag)
        {
            return Create<Ability>(typeTag);
        }

        public static T Create<T>(Tag typeTag) where T : Ability
        {
            Type abilityType = AbilityReflector.GetRegisteredAbilityType(typeTag);
            if (abilityType != null)
            {
                if (Activator.CreateInstance(abilityType, typeTag) is T abilityInstance)
                {
                    abilityInstance.OnPostConstruct();
                    return abilityInstance;
                }
                else
                {
                    GD.PrintErr($"Failed to create instance of {nameof(Ability)} because created instance was null");
                    return null;
                }
            }
            else
            {
                GD.PrintErr($"Failed to create instance of {nameof(Ability)} because type was null");
                return null;
            }
        }
    }
}
