using System.Collections;
using System.Collections.Generic;
using Godot;


namespace framework.systems.state_machine
{
    [Tool, GlobalClass, Icon("res://addons/state_machine/assets/transition.svg")]
    public partial class Transition : NodeStateMachine
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
    }
}
