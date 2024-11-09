using Godot;

namespace HCoroutines;

/// <summary>
/// Waits until a certain signal has been fired.
/// </summary>
public class WaitForSignalCoroutine : CoroutineBase
{
    private readonly GodotObject _targetObject;
    private readonly string _targetSignal;

    public WaitForSignalCoroutine(GodotObject obj, string signal) 
        : base(CoProcessMode.Inherit, CoRunMode.Inherit)
    {
        _targetObject = obj;
        _targetSignal = signal;
    }

    protected override void OnEnter() {
        Manager.ToSignal(_targetObject, _targetSignal).OnCompleted(Kill);
    }
}