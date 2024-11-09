using Godot;
using System;

namespace HCoroutines.Utils;

public class PausableTimer
{
    private readonly SceneTreeTimer _timer;
    private float _remainingTime;

    public PausableTimer(SceneTree sceneTree, float timeout, bool ignoreTimeScale, Action callback)
    {
        _timer = sceneTree.CreateTimer(timeout, ignoreTimeScale: ignoreTimeScale);
        _timer.Timeout += callback;
    }

    public void Pause()
    {
        _remainingTime = (float)_timer.TimeLeft;
        _timer.TimeLeft = double.MaxValue;
    }

    public void Resume()
    {
        _timer.TimeLeft = _remainingTime;
    }
}