using Godot;
using System;
using System.Linq;

namespace framework.systems.state_machine
{
    [Tool, GlobalClass, Icon("res://addons/framework/assets/state_machine/compound_state.svg")]
    public partial class CompoundState : State
    {
        [Export]
        private State InitState
        {
            set
            {
                _initState = value;
                UpdateConfigurationWarnings();
            }

            get => _initState;
        }

        private State _initState;

        /// <summary> The current running and active state.</summary>
        public State ActiveState
        {
            get => _activeState;
            private set
            {
                _activeState?.OnExit();

                var oldState = _activeState;
                _activeState = value;
                EventStateChanged.Invoke(oldState, _activeState);

                _activeState?.OnEnter();
            }
        }

        /// <summary> The current running and active state.</summary>
        private State _activeState;

        public readonly Action<State, State> EventStateChanged = delegate { };

        public CompoundState()
        {

        }

#if TOOLS
        public override string[] _GetConfigurationWarnings()
        {
            var warnings = base._GetConfigurationWarnings().ToList();

            if (GetChildren().TakeWhile(child => child is not (ParallelState or CompoundState or AtomicState)).Any())
            {
                warnings.Add(
                    "The CompoundState must have exactly one child (ParallelState or CompoundState or Atomic Root)");
            }

            if (InitState == null)
            {
                warnings.Add("The CompoundState must have exactly one init state (ParallelState or CompoundState)");
            }

            return warnings.ToArray();
        }
#endif

        public override State ProcessTransition(TriggerType triggerType, StringName eventName)
        {
            var targetState = base.ProcessTransition(triggerType, eventName);

            if (IsInstanceValid(_activeState))
            {
                targetState = _activeState.ProcessTransition(triggerType, eventName);
            }

            if (targetState != null)
            {
                ActiveState = targetState;
            }

            return targetState;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            if (InitState != null)
            {
                _activeState = InitState;
                _activeState.OnEnter();
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (_activeState is not null)
            {
                _activeState.OnExit();
                _activeState = null;
            }
        }
    }
}