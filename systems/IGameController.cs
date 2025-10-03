// Scripts/Core/GameController.cs
using Godot;
using System.Collections.Generic;
using framework.systems.core.events;
using framework.systems.core.interfaces;
using framework.systems.core.services;
using framework.systems.managers;
using IUpdateable = framework.systems.core.services.IUpdateable;

namespace framework.systems
{
    public abstract partial class IGameController : Node
    {
        protected ServiceLocator Services => ServiceLocator.Instance;
        protected EventBus EventBus => Services.Get<EventBus>();
        protected ISceneManager SceneManager => Services.Get<ISceneManager>();
        protected IAudioManager AudioManager => Services.Get<IAudioManager>();
        protected ISaveSystem SaveSystem => Services.Get<ISaveSystem>();
        protected IResourceManager ResourceManager => Services.Get<IResourceManager>();
        
        private readonly List<IUpdateable> _updateables = new();
        private readonly List<IFixedUpdateable> _fixedUpdateables = new();
        
        public override void _Ready()
        {
            base._Ready();
            OnInitialize();
            RegisterUpdateables();
        }
        
        protected virtual void OnInitialize() { }
        
        protected virtual void RegisterUpdateables()
        {
            // 子类注册需要更新的组件
        }
        
        protected void RegisterUpdateable(IUpdateable updateable)
        {
            if (!_updateables.Contains(updateable))
            {
                _updateables.Add(updateable);
            }
        }
        
        protected void RegisterFixedUpdateable(IFixedUpdateable fixedUpdateable)
        {
            if (!_fixedUpdateables.Contains(fixedUpdateable))
            {
                _fixedUpdateables.Add(fixedUpdateable);
            }
        }
        
        public override void _Process(double delta)
        {
            var deltaTime = (float)delta;
            foreach (var updateable in _updateables)
            {
                updateable.Update(deltaTime);
            }
            
            OnUpdate(deltaTime);
        }
        
        public override void _PhysicsProcess(double delta)
        {
            var fixedDeltaTime = (float)delta;
            foreach (var fixedUpdateable in _fixedUpdateables)
            {
                fixedUpdateable.FixedUpdate(fixedDeltaTime);
            }
            
            OnFixedUpdate(fixedDeltaTime);
        }
        
        protected virtual void OnUpdate(float deltaTime) { }
        protected virtual void OnFixedUpdate(float fixedDeltaTime) { }
        
        public override void _ExitTree()
        {
            OnCleanup();
            _updateables.Clear();
            _fixedUpdateables.Clear();
            base._ExitTree();
        }
        
        protected virtual void OnCleanup() { }
        
        protected void PublishEvent<T>(T gameEvent) where T : IGameEvent
        {
            EventBus?.Publish(gameEvent);
        }
        
        protected void Subscribe<T>(System.Action<T> handler) where T : IGameEvent
        {
            EventBus?.Subscribe(handler);
        }
        
        protected void Unsubscribe<T>(System.Action<T> handler) where T : IGameEvent
        {
            EventBus?.Unsubscribe(handler);
        }
    }
}
