using System.Collections;
using System.Collections.Generic;
using Godot;


namespace framework.systems.state_machine
{
    [Tool, GlobalClass, Icon("res://addons/framework/assets/state_machine/transition.svg")]
    public partial class Transition : HStateMachine
    {
        [Export]
        public State TargetState
        {
            private set
            {
                _targetState = value;
                UpdateConfigurationWarnings();
            }
        
            get => _targetState;
        }
        private State _targetState;
        private State _ownerState;
        private StateMachine _stateMachine;
        
        /// <summary>
        /// Fired when this transition is taken. For delayed transitions,
        /// this signal will be fired when the transition is actually executed
        /// (e.g., when its delay has elapsed and the transition has not been aborted before).
        /// The signal will always be fired before the state is exited.
        /// </summary>
        [Signal] public delegate void TokenEventHandler();

        public Transition()
        {
        
        }

        public virtual void Setup(State ownerState, StateMachine stateMachine)
        {
            _ownerState = ownerState;
            _stateMachine = stateMachine;
        }
    
#if TOOLS
        public override string[] _GetConfigurationWarnings()
        {
            var warnings = new List<string>();

            if (GetChildCount() > 0) warnings.Add("Transitions should not have children");
            if (TargetState == null) warnings.Add("The target state is not set");
            if (GetParent() is not State) warnings.Add("Transitions must be children of states.");
        
            return warnings.Count > 0 ? warnings.ToArray() : base._GetConfigurationWarnings();
        }
#endif

        public virtual bool CanTransition() { return true; }
        
        public virtual float GetDelay() { return 0; }

        public virtual StringName GetTargetName()
        {
            return _targetState.GetName();
        }
    }
}
