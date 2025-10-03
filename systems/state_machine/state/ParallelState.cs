using Godot;
using System;
using System.Linq;

namespace framework.systems.state_machine
{
    /// <summary>
    /// ## A parallel state is a state which can have sub-states, all of which are active when the parallel state is active.
    /// </summary>
    [Tool, GlobalClass, Icon("res://addons/state_machine/assets/parallel_state.svg")]
    public partial class ParallelState : State
    {
        public ParallelState()
        {
        }

#if TOOLS
        public override string[] _GetConfigurationWarnings()
        {
            var warnings = base._GetConfigurationWarnings().ToList();
        
            if (GetChildren().TakeWhile(child => child is not (ParallelState or CompoundState or AtomicState)).Any())
            {
                warnings.Add("The CompoundState must have exactly one child (ParallelState or CompoundState or Atomic Root)");
            }
        
            return warnings.ToArray();
        }
#endif

        public override State ProcessTransition()
        {
            var targetState = base.ProcessTransition();

            foreach (var state in States)
            {
                targetState = state.ProcessTransition();
            }
        
            return targetState;
        }

        public override void OnEnter()
        {
            base.OnEnter();
        
            foreach (var state in States)
            {
                state.OnEnter();
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            foreach (var state in States)
            {
                state.OnExit();
            }
        }
    }

}
