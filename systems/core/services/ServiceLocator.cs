// Scripts/Core/Services/ServiceLocator.cs
using System;
using System.Collections.Generic;
using Godot;
using framework.systems.core.events;
using framework.systems.core.singleton;
using framework.systems.managers;


namespace framework.systems.core.services
{
    public interface IService
    {
        public abstract ServiceLocator Locator { get; set; }
        void Initialize();
        void Shutdown();
    }
    
    public partial class ServiceLocator : Singleton<ServiceLocator>
    {
        private readonly Dictionary<Type, IService> _services = new();
        private readonly List<IService> _updateableServices = new();
        
        public override void _Ready()
        {
            ProcessMode = Node.ProcessModeEnum.Always;
            RegisterCoreServices();
        }
        
        private void RegisterCoreServices()
        {
            // 注册核心服务
            Register<EventBus>(new EventBus());
            Register<ISaveSystem>(new ISaveSystem());
            Register<IAudioManager>(new IAudioManager());
            Register<InputManager>(new InputManager());
            Register<ISceneManager>(new ISceneManager());
            Register<IResourceManager>(new IResourceManager());
            Register<IUIManager>(new IUIManager());
            Register<IGameDataManager>(new IGameDataManager());
            Register<IPoolManager>(new IPoolManager());
        }
        
        public void Register<T>(T service) where T : class, IService
        {
            var type = typeof(T);
            
            if (!_services.TryAdd(type, service))
            {
                GD.PrintErr($"Service {type.Name} already registered!");
                return;
            }
            
            service.Locator = this;
            service.Initialize();
            
            if (service is IUpdateable updateable)
            {
                _updateableServices.Add(service);
            }
            
            GD.Print($"Service {type.Name} registered successfully");
        }
        
        public T Get<T>() where T : class, IService
        {
            var type = typeof(T);
            
            if (_services.TryGetValue(type, out var service))
            {
                return service as T;
            }
            
            GD.PrintErr($"Service {type.Name} not found!");
            return null;
        }
        
        public bool TryGet<T>(out T service) where T : class, IService
        {
            service = Get<T>();
            return service != null;
        }
        
        public override void _Process(double delta)
        {
            foreach (var service in _updateableServices)
            {
                if (service is IUpdateable updateable)
                {
                    updateable.Update((float)delta);
                }
            }
        }
        
        protected override void OnSingletonDestroyed()
        {
            foreach (var service in _services.Values)
            {
                service.Shutdown();
            }
            _services.Clear();
            _updateableServices.Clear();
        }
    }
    
    public interface IUpdateable
    {
        void Update(float deltaTime);
    }
}
