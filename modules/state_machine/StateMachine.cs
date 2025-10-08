using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using framework.debug;
using framework.utility.hierarchical_coroutines;
using Godot;
using Godot.Collections;

namespace framework.modules.state_machine
{
    /// <summary>
    /// A simple state machine. It contains a root state (commonly a compound or parallel state) and is the entry point for the state machine.
    /// </summary>
    [Tool, GlobalClass, Icon("res://addons/framework/assets/state_machine/state_machine.svg")]
    public partial class StateMachine : HStateMachine
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
        
        /// <summary>
        /// Flag indicating if the state chart is frozen.  If the state chart is frozen, new events and transitions will be discarded.
        /// </summary>
        private bool _frozen = false;
        
        /// <summary> The running state node. </summary>
        private State _root;
        
        /// <summary> The root state of beginning </summary>
        private State _entryState;
        
        /// <summary> State machine tick coroutine. </summary>
        private Coroutine _tickCoroutine;
        
        /// <summary>
        /// A list of pending events 
        /// </summary>
        private Array<StringName> _queuedEvents;
        
        /// <summary>
        /// Whether a property change is pending.
        /// </summary>
        private bool _propertyChangePending = false;
        
        /// <summary>
        /// Whether a state change occured during processing, and we need to re-run automatic transitions that may have been triggered by the state change.
        /// </summary>
        private bool _stateChangePending = false;
        
        /// <summary>
        /// ## Flag indicating if the state chart is currently processing. Until a change is fully processed, no further changes can be introduced from the outside.
        /// </summary>
        private bool _lockedDown = false;
        
        private Queue<Godot.Collections.Dictionary<Transition, State>> _queueTransitions = new ();
        private bool _transitionsProcessingActive = false;
        
        /// <summary>
        /// Emitted when the state chart receives an event. This will be 
        /// emitted no matter which state is currently active and can be 
        /// useful to trigger additional logic elsewhere in the game 
        /// without having to create a custom event bus. It is also used
        /// by the state chart debugger. Note that this will emit the 
        /// events in the order in which they are processed, which may 
        /// be different from the order in which they were received. This is
        /// because the state chart will always finish processing one event
        /// fully before processing the next. If an event is received
        /// while another is still processing, it will be enqueued.
        /// </summary>
        [Signal] public delegate void EventReceivedEventHandler(StringName eventName);
        
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

            // if (_tickCoroutine == null)
            // {
            //     OnStarted.Invoke();
            //
            //     Root = _entryState;
            //     
            //     _tickCoroutine = CoroutineManager.Instance.StartCoroutine(Tick());
            // }
        }

        public void Stop()
        {
            GD.Print("Stopping state_machine ...");
            
            // if (_tickCoroutine != null)
            // {
            //     CoroutineManager.Instance.StopCoroutine(_tickCoroutine);
            //     _tickCoroutine = null;
            //
            //     Root = null;
            //
            //     OnStopped.Invoke();
            // }
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
                // Root.ProcessTransition();
                yield return null;
            }
        }
        
        /// <summary>
        /// We wait one frame before entering initial state, so parents of the state chart have a chance to run their
        /// _ready methods first and not get events from the state chart while they have not yet been initialized
        /// </summary>
        private void EnterInitialState()
        {
            _transitionsProcessingActive =  true;
            _lockedDown = true;
            
            _root?.OnEnter();
            
            // run any queued transitions that may have come up during the enter
            RunQueueTransition();
	
            // run any queued external events that may have come up during the enter
            RunChange();
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
            if (_frozen)
            {
                GD.PrintErr("State machine is frozen");
                return;
            }
            
            if (!IsNodeReady())
            {
                GD.PrintErr("\"State chart is not yet ready. If you call `send_event` in _ready, please call it deferred, e.g. `state_chart.send_event.call_deferred(\\\"my_event\\\").\"");
                return;
            }

            if (!IsInstanceValid(_root))
            {
                GD.PrintErr("State chart has no root state. Ignoring call to `send_event`.");
                return;
            }
            
            _queuedEvents.Add(name);
            
            if (_lockedDown)
                return;
            
            RunChange();
        }

        public void RunChange()
        {
            _lockedDown = true;

            while (_queuedEvents.Count != 0)
            {
                // 1.State changes
                if (_stateChangePending)
                {
                    _stateChangePending = false;
                    _root.ProcessTransition(TriggerType.StateChange);
                }

                // 2. Property changes
                if (_propertyChangePending)
                {
                    _propertyChangePending = false;
                    _root.ProcessTransition(TriggerType.PropertyChange);
                }
                
                // 3.event
                if (_queuedEvents.Count != 0)
                {
                    var next = _queuedEvents[0];
                    _queuedEvents.RemoveAt(0);
                    EmitSignal(SignalName.EventReceived, next);
                }
            }
            
            _lockedDown = false;
        }
        
        /// <summary>
        /// Allows states to queue a transition for running. This will eventually run the transition
        /// once all currently running transitions have finished. States should call this method
        /// when they want to transition away from themselves.
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="fromState"></param>
        public void RunTransition(Transition transition, State fromState)
        {
            _queueTransitions.Enqueue(new Godot.Collections.Dictionary<Transition, State>{{transition, fromState}});
            
            if (_transitionsProcessingActive) return;

            RunQueueTransition();
        }

        public void RunQueueTransition()
        {
            _transitionsProcessingActive = true;

            var executionCount = 1;
        
            // if we still have transitions
            while (_queueTransitions.Count > 0)
            {
                var nextTransitionEntry = _queueTransitions.Dequeue();
                var nextTransition = nextTransitionEntry.Keys.First();
                var nextTransitionFromState = nextTransitionEntry[nextTransition];
                DoRunTransition(nextTransition, nextTransitionFromState);
                executionCount += 1;

                if (executionCount > 50)
                {
                    GD.PrintErr("Infinite loop detected in transitions. Aborting. The state chart is now in an invalid state and no longer usable.");
                    break;
                }
            }
            
            _transitionsProcessingActive = false;
            
	        // transitions trigger a state change which can in turn activate other transitions, so we need to handle these
            if (!_lockedDown) RunChange();
        }
        
        /// <summary>
        /// Runs the transition. Used internally by the state chart, do not call this directly.	
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="fromState"></param>
        private void DoRunTransition(Transition transition, State fromState)
        {
            if (fromState.Active)
            {
                EmitSignal(Transition.SignalName.Token);
                fromState.RunTransition(transition);
                _stateChangePending =  true;
            }
            else
            {
                DebugLog.ErrorLog(
                    $"Ignoring request for transitioning from {fromState.GetName()} to {transition.GetTargetName()} " +
                    $"as the source state is no longer active. Check whether your trigger multiple state changes within a single frame."
                    );
            }
        }
        
        
    }

    public enum TriggerType
    {
        // No trigger type. This usually should not happen and is used as a default value.
        None = 0,
        // The transition will be triggered by an event.
        Event = 1,
        // The transition is automatic and thus will be triggered when the state is entered.
        StateEnter = 2,
        // The transition is automatic and will be triggered by a property change.
        PropertyChange = 4,
        // The transition is automatic and will be triggered by a state change.
        StateChange = 8,
    }
}
