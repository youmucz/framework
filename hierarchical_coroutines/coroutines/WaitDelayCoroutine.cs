using Godot;
using HCoroutines.Utils;

namespace HCoroutines;

/// <summary>
/// Waits until a certain delay has passed.
/// </summary>
public class WaitDelayCoroutine : CoroutineBase
{
    private readonly float _delay;
    private readonly bool _ignoreTimeScale;

    private PausableTimer _timer;

    public WaitDelayCoroutine(float delay, bool ignoreTimeScale = false, CoRunMode runMode = CoRunMode.Inherit)
        : base(CoProcessMode.Inherit, runMode)
    {
        _delay = delay;
        _ignoreTimeScale = ignoreTimeScale;
    }

    protected override void OnStart()
    {
        _timer = new PausableTimer(Manager.GetTree(), _delay, _ignoreTimeScale, callback: Kill);
    }

    protected override void OnPause()
    {
        _timer.Pause();
    }

    protected override void OnResume()
    {
        _timer.Resume();
    }
}