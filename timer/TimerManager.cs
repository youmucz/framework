using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using HCoroutines;

namespace Minikit
{
    public partial class TimerManager : Node
    {
        public bool IsPaused { get; private set; }
        
        public static TimerManager Instance { get; private set; }
        private static TimerManager _globalInstance;
        
        private readonly List<TimerHandle_Tick> _timerHandles = new();
        
        public override void _EnterTree()
        {
            Instance = this;
            ProcessMode = ProcessModeEnum.Always;
            IsPaused = GetTree().Paused;

            if (IsAutoload())
            {
                // This instance is the global (autoload) instance that is shared between scenes.
                _globalInstance = this;
            }
        }

        public override void _ExitTree()
        {
            if (Instance == this)
            {
                // Switch back to the global (autoload) manager when the scene-local instance is removed (e.g. when
                // the current scene is changed).
                Instance = _globalInstance;
            }
        }

        private bool IsAutoload()
        {
            return GetParent() == GetTree().Root && GetTree().CurrentScene != this;
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            
            foreach (var timerHandle in _timerHandles.ToArray())
            {
                if (timerHandle.CancelRequested)
                {
                    _timerHandles.Remove(timerHandle);
                    continue;
                }

                if (timerHandle.IsCompleted)
                {
                    timerHandle.Finish();
                    _timerHandles.Remove(timerHandle);
                }
            }
        }
        
        public TimerHandle_Tick NewTimer_Tick(float delay, Action action)
        {
            var timerHandle = new TimerHandle_Tick(delay, action);
            _timerHandles.Add(timerHandle);
            return timerHandle;
        }

        public TimerHandle_Coroutine NewTimer_Coroutine(float delay, Action action)
        {
            return new TimerHandle_Coroutine(CoroutineManager.Instance.StartCoroutine(DoCoroutineTimer(delay, action)), action);
        }
        
        IEnumerator DoCoroutineTimer(float delay, Action action)
        {
            yield return new WaitDelayCoroutine(delay);
        
            action?.Invoke();
        }
        
        public TimerHandle_Coroutine NewTimer_Coroutine_NextFrame(Action action)
        {
            return NewTimer_Coroutine_Frames(1, action);
        }
        
        public TimerHandle_Coroutine NewTimer_Coroutine_Frames(int frames, Action action)
        {
            return new TimerHandle_Coroutine(CoroutineManager.Instance.StartCoroutine(DoCoroutineTimer_Frames(frames, action)), action);
        }
        
        /// <summary>
        /// Wait util end of current frame.
        /// </summary>
        /// <param name="frames"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private IEnumerator DoCoroutineTimer_Frames(int frames, Action action)
        {
            if (frames > 0)
            {
                for (var i = 0; i < frames; i++)
                {
                    yield return new WaitForSignalCoroutine(GetTree(), SceneTree.SignalName.ProcessFrame);
                }
            }
        
            action?.Invoke();
        }
    }
    
    public class TimerHandle_Tick
    {
        private readonly Action _action;
        private readonly float _timeOfStart;
        private readonly float _duration;
        public bool CancelRequested { get; private set; } = false;
        public bool IsCompleted => GetTime() > _timeOfStart + _duration;
        public float TimeLeft => Mathf.Clamp(GetTime()- _timeOfStart + _duration, 0f, _duration);
        
        public TimerHandle_Tick(float duration, Action action)
        {
            _action = action;
            _timeOfStart = GetTime();
            _duration = duration;
        }

        public float GetTime()
        {
            return (float)(Time.GetTicksUsec() / 1000000.0);
        }

        public void Cancel()
        {
            CancelRequested = true;
        }

        public void Finish()
        {
            Cancel();
            _action?.Invoke();
        }
    }

    public class TimerHandle_Coroutine
    {
        private readonly Coroutine _coroutine;
        private readonly Action _action;
    
        public TimerHandle_Coroutine(Coroutine coroutine, Action action)
        {
            _coroutine = coroutine;
            _action = action;
        }
    
        public void Cancel()
        {
            CoroutineManager.Instance.StopCoroutine(_coroutine);
        }
    
        public void Finish()
        {
            Cancel();
            _action?.Invoke();
        }
    }
}
