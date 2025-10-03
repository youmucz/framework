using System;
using System.Collections;
using framework.utility.hierarchical_coroutines;
using Godot;

namespace framework.systems.state_machine
{
    [Tool, GlobalClass, Icon("res://addons/state_machine/assets/state_machine.svg")]
    public partial class StateMachine : NodeStateMachine
    {
        [Export] private bool _autoStart = false;
        
        /// <summary> The running state node. </summary>
        public State Root
        {
            get => _root;
            private set
            {
                _root?.OnExit();

                var oldState = _root;
                _root = value;
                OnStateChanged.Invoke(oldState, _root);

                _root?.OnEnter();
            }
        }
        /// <summary> The running state node. </summary>
        private State _root;
        
        /// <summary> The root state of beginning </summary>
        private State _entryState;
        
        /// <summary> State machine tick coroutine. </summary>
        private Coroutine _tickCoroutine;
        
        public readonly Action<State, State> OnStateChanged = delegate { };
        public readonly Action OnStarted = delegate { };
        public readonly Action OnStopped = delegate { };

        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;
            
            Setup();
            
            if (_autoStart) Start();
        }
        
#if TOOLS
        public override string[] _GetConfigurationWarnings()
        {
            if (GetChildCount() != 1)
            {
                return new[] { "Entered state_machine must have exactly one child (ParallelState or CompoundState)" };
            }

            foreach (var child in GetChildren())
            {
                if (child is ParallelState parallelState || child is CompoundState compoundState) break;
                
                return new[] { "Entered state_machine must have exactly one child (ParallelState or CompoundState)" };
            }
            
            return base._GetConfigurationWarnings();
        }
#endif
        
        /// <returns> The entry state for the state machine (from the list of states passed out) </returns>
        private void Setup()
        {
            foreach (var child in GetChildren())
            {
                _entryState = child switch
                {
                    ParallelState parallelState => parallelState,
                    CompoundState compoundState => compoundState,
                    State state => state,
                    _ => _entryState
                };
            }
            
            _entryState.Setup(this);
        }

        public void Start()
        {
            GD.Print("Starting state_machine ...");
            
            if (_entryState == null)
            {
                Setup();
            }

            if (_tickCoroutine == null)
            {
                OnStarted.Invoke();

                Root = _entryState;
                
                _tickCoroutine = CoroutineManager.Instance.StartCoroutine(Tick());
            }
        }

        public void Stop()
        {
            GD.Print("Stopping state_machine ...");
            
            if (_tickCoroutine != null)
            {
                CoroutineManager.Instance.StopCoroutine(_tickCoroutine);
                _tickCoroutine = null;

                Root = null;

                OnStopped.Invoke();
            }
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public virtual IEnumerator Tick()
        {
            while (Root != null)
            {
                Root.ProcessTransition();
                yield return null;
            }
        }
        
        /// <summary>
        /// Sends an event to this state machine. The event will be passed to the innermost active state first and
        /// is then moving up in the tree until it is consumed. Events will trigger transitions and actions via emitted
        /// signals. There is no guarantee when the event will be processed. The state machine will process the event
        /// as soon as possible but there is no guarantee that the event will be fully processed when this method returns.
        /// </summary>
        /// <param name="name"></param>
        public void SendEvent(StringName name)
        {
            
        }
        
        /// <summary>
        /// ## Allows states to queue a transition for running. This will eventually run the transition
        /// once all currently running transitions have finished. States should call this method
        /// when they want to transition away from themselves.
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="fromState"></param>
        public void RunTransition(Transition transition, State fromState)
        {
            
        }

        public void DoTransitionTo(State newState)
        {
            
        }
    }
}
