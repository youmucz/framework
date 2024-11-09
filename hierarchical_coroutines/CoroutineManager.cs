using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using HCoroutines.Utils;

namespace HCoroutines;

public partial class CoroutineManager : Node
{
    public static CoroutineManager Instance { get; private set; }
    private static CoroutineManager _globalInstance;
    
    public float DeltaTime { get; private set; }
    public double DeltaTimeDouble { get; private set; }
    
    public float PhysicsDeltaTime { get; private set; }
    public double PhysicsDeltaTimeDouble { get; private set; }

    public bool IsPaused { get; private set; }

    private readonly DeferredHashSet<CoroutineBase> _activeProcessCoroutines = new();
    private readonly DeferredHashSet<CoroutineBase> _activePhysicsProcessCoroutines = new();
    private readonly HashSet<CoroutineBase> _aliveRootCoroutines = new();
    
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

    /// <summary>
    /// Starts and initializes the given coroutine.
    /// </summary>
    public void StartCoroutine(CoroutineBase coroutine)
    {
        coroutine.Manager = this;
        coroutine.Stopped += () => _aliveRootCoroutines.Remove(coroutine);
        coroutine.Init();
        _aliveRootCoroutines.Add(coroutine);
    }
    
    /// <summary>
    /// Starts and initializes the given coroutine.
    /// </summary>
    public Coroutine StartCoroutine(IEnumerator routine)
    {
        var coroutine = new Coroutine(routine);
        
        StartCoroutine(coroutine);
        
        return coroutine;
    }
    
    /// <summary>
    /// Stop and deactivate the given coroutine.
    /// </summary>
    /// <param name="coroutine"></param>
    public void StopCoroutine(Coroutine coroutine)
    {
        DeactivateCoroutine(coroutine);
    }

    /// <summary>
    /// Enables Update() calls to the coroutine.
    /// </summary>
    public void ActivateCoroutine(CoroutineBase coroutine)
    {
        GetUpdatePoolOfCoroutine(coroutine).Add(coroutine);
    }

    /// <summary>
    /// Disables Update() calls to the coroutine.
    /// </summary>
    public void DeactivateCoroutine(CoroutineBase coroutine)
    {
        GetUpdatePoolOfCoroutine(coroutine).Remove(coroutine);
    }

    private DeferredHashSet<CoroutineBase> GetUpdatePoolOfCoroutine(CoroutineBase coroutine)
    {
        return coroutine.ProcessMode switch {
            CoProcessMode.Normal or CoProcessMode.Inherit => _activeProcessCoroutines,
            CoProcessMode.Physics => _activePhysicsProcessCoroutines,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public override void _Process(double delta)
    {
        DeltaTime = (float)delta;
        DeltaTimeDouble = delta;
        SetGamePaused(GetTree().Paused);

        UpdateCoroutines(_activeProcessCoroutines);
    }

    public override void _PhysicsProcess(double delta)
    {
        PhysicsDeltaTime = (float)delta;
        PhysicsDeltaTimeDouble = delta;

        UpdateCoroutines(_activePhysicsProcessCoroutines);
    }

    private void UpdateCoroutines(DeferredHashSet<CoroutineBase> coroutines)
    {
        coroutines.Lock();

        foreach (var coroutine in coroutines.Items)
        {
            if (coroutine.IsAlive && coroutine.ShouldReceiveUpdates)
            {
                try
                {
                    coroutine.Update();
                }
                catch (Exception e)
                {
                    GD.PrintErr(e.ToString());
                }
            }
        }

        coroutines.Unlock();
    }

    private void SetGamePaused(bool isPaused)
    {
        if (this.IsPaused == isPaused)
        {
            return;
        }

        this.IsPaused = isPaused;

        foreach (CoroutineBase coroutine in _aliveRootCoroutines)
        {
            coroutine.OnGamePausedChanged(isPaused);
        }
    }
}