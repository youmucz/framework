using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace framework.systems.state_machine
{
    [Tool, GlobalClass, Icon("res://addons/framework/assets/state_machine/atomic_state.svg")]
    public partial class State : HStateMachine
    {
        // Called when the state is entered.
        [Signal] public delegate void StateEnteredEventHandler();

        // Called when the state is exited.
        [Signal] public delegate void StateExitedEventHandler();

        // Called when the state receives an event. Only called if the state is active.
        [Signal] public delegate void EventReceivedEventHandler(StringName eventName);

        // Called when the state is processing.
        [Signal] public delegate void StateProcessingEventHandler(float delta);

        // Called when the state is physics processing.
        [Signal] public delegate void StatePhysicsProcessingEventHandler(float delta);

        // Called when the state chart step function is called.
        [Signal] public delegate void StateSteppedEventHandler(float delta);

        // Called when the state is receiving input.
        [Signal] public delegate void StateInputEventHandler(InputEvent inputEvent);

        // Called when the state is receiving unhandled input.
        [Signal] public delegate void StateUnhandledInputEventHandler(InputEvent inputEvent);
        
        /// <summary>
        /// Called every frame while a delayed transition is pending for this state. Returns the initial delay and the remaining delay of the transition.
        /// </summary>
        [Signal] public delegate void TransitionPendingEventHandler(float initialDelay, float remainingDelay);
        
        // The currently active pending transition.
        private Transition _pendingTransition;

        // Remaining time in seconds until the pending transition is triggered.
        private float _pendingTransitionRemainingDelay = 0f;
            
        // The initial time of the pending transition.
        private float _pendingTransitionInitialDelay = 0f;
            
        public bool Active { get; set; }
        public Array<State> States { get; private set; } = new();
        public Array<Transition> Transitions { get; set; } = new ();
        
        private StateMachine _stateMachine;
        
        protected State()
        {
            
        }

        public override void _Ready()
        {
            base._Ready();
            
            if (Engine.IsEditorHint()) return;
            
            GD.Print($"Root {GetStateName()} Ready.");
        }
        
    #if TOOLS
        public override string[] _GetConfigurationWarnings()
        {
            var warnings = new List<string>();
            
            var parent = GetParent();
            var found = false;
            while (IsInstanceValid(parent))
            {
                if (parent is StateMachine)
                {
                    found = true;
                    break;
                }
                
                parent = parent.GetParent();
            }
            
            if (!found) warnings.Add($"The parent '{parent}' is not a state machine.");
            
            return warnings.ToArray();
        }
    #endif
        
        public virtual void Setup(StateMachine stateMachine)
        {
            Active = false;
            _stateMachine = stateMachine;
            
            Transitions = new();
            States = new();
            
            foreach (var child in GetChildren())
            {
                switch (child)
                {
                    case Transition transition:
                        transition.Setup(this, _stateMachine);
                        Transitions.Add(transition);
                        break;
                    case State state:
                        state.Setup(_stateMachine);
                        States.Add(state);
                        break;
                }
            }
        }

        public virtual string GetStateName() {return GetType().Name;}
        
        /// <summary>
        /// Handle any transitions that want to take place
        /// </summary>
        /// <returns></returns>
        public virtual State ProcessTransition(TriggerType triggerType, StringName eventName = null) 
        {         
            if (!Active) return null;
            
            foreach (var transition in Transitions)
            {
                if (transition.CanTransition())
                {
                    RunTransition(transition);
                    return transition.TargetState;
                }
            }

            return null;
        }
        
        /// <summary>
        /// Runs a transition either immediately or delayed depending on the transition settings.
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="immediately"></param>
        public virtual void RunTransition(Transition transition, bool immediately = false)
        {
            if (!Active) return;
            
            var initialDelay = transition.GetDelay();

            if (!immediately && initialDelay > 0f)
            {
                _pendingTransition = transition;
                _pendingTransitionInitialDelay = initialDelay;
                _pendingTransitionRemainingDelay = initialDelay;
            }
            else
            {
                _stateMachine.RunTransition(transition, this);
            }
        }

        public virtual void OnEnter()
        {
            Active = true; 
            
            
        }

        public virtual void OnExit() { Active = false; }
    }

}
