// Scripts/Core/Services/ServiceLocator.cs
using System;
using System.Collections.Generic;
using croptails.scripts.managers;
using Godot;
using framework.systems.core.events;
using framework.systems.core.singleton;
using framework.systems.managers;


namespace framework.systems.core.services
{
    public interface IService
    {
        public ServiceLocator Locator { get; set; }
        void Initialize();
        void Shutdown();
    }
    
    public partial class ServiceLocator : Singleton<ServiceLocator>
    {
        private readonly Dictionary<Type, IService> _services = new();
        
        public override void _Ready()
        {
            SetName("ServiceLocator");
            ProcessMode = Node.ProcessModeEnum.Always;
            RegisterCoreServices();
        }
        
        private void RegisterCoreServices()
        {
            // 注册核心服务
            Register<EventBus>(new EventBus());
            Register<IUpdateManager>(new IUpdateManager());
            Register<ISaveSystem>(new ISaveSystem());
            Register<IAudioManager>(new IAudioManager());
            Register<IInputManager>(new IInputManager());
            Register<ISceneManager>(new ISceneManager());
            Register<IResourceManager>(new IResourceManager());
            Register<IGameDataManager>(new IGameDataManager());
            Register<IPoolManager>(new IPoolManager());
            Register<IUIManager>(new IUIManager());
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
        
        protected override void OnSingletonDestroyed()
        {
            foreach (var service in _services.Values)
            {
                service.Shutdown();
            }
            _services.Clear();
        }
    }
}
