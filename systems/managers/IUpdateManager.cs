// Scripts/Core/GameController.cs
using Godot;
using System.Collections.Generic;
using framework.systems.core.events;
using framework.systems.core.interfaces;
using framework.systems.core.services;

namespace framework.systems.managers
{
    public partial class IUpdateManager : Node, IService
    {
        private EventBus EventBus => ServiceLocator.Instance.Get<EventBus>();
        
        private readonly List<IUpdateable> _updateable = new();
        private readonly List<IFixedUpdateable> _fixedUpdateable = new();
        
        public ServiceLocator Locator { get; set; }
        
        public void Initialize()
        {
            SetName("UpdateManager");
            Locator.AddChild(this);
            RegisterUpdateable();
        }

        public void Shutdown()
        {
            
        }
        
        protected virtual void RegisterUpdateable()
        {
            // 子类注册需要更新的组件
        }
        
        protected void RegisterUpdateable(IUpdateable updateable)
        {
            if (!_updateable.Contains(updateable))
            {
                _updateable.Add(updateable);
            }
        }
        
        protected void RegisterFixedUpdater(IFixedUpdateable fixedUpdateable)
        {
            if (!_fixedUpdateable.Contains(fixedUpdateable))
            {
                _fixedUpdateable.Add(fixedUpdateable);
            }
        }
        
        public override void _Process(double delta)
        {
            var deltaTime = (float)delta;
            foreach (var updater in _updateable)
            {
                updater.Update(deltaTime);
            }
            
            OnUpdate(deltaTime);
        }
        
        public override void _PhysicsProcess(double delta)
        {
            var fixedDeltaTime = (float)delta;
            foreach (var fixedUpdateable in _fixedUpdateable)
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
            _updateable.Clear();
            _fixedUpdateable.Clear();
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
