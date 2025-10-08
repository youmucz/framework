// Scripts/Core/Services/ServiceLocator.cs
using System;
using System.Collections.Generic;
using croptails.scripts.managers;
using framework.core.events;
using framework.core.interfaces;
using framework.core.singleton;
using framework.modules.managers;
using Godot;


namespace framework.core.services
{
    public interface IService
    {
        public ServiceLocator Locator { get; set; }
        void Initialize();
        void Shutdown();
    }
    
    public partial class ServiceLocator : Singleton<ServiceLocator>
    {
        private readonly Dictionary<StringName, IService> _services = new();
        
        public override void _Ready()
        {
            SetName("ServiceLocator");
            ProcessMode = Node.ProcessModeEnum.Always;
            RegisterCoreServices();
        }
        
        private void RegisterCoreServices()
        {
            // 注册核心服务
            Register<IEventBus>("EventBus");
            Register<IUpdateManager>("UpdateManager");
            Register<ISaveSystem>("SaveSystem");
            Register<IAudioManager>("AudioManager");
            Register<IInputManager>("InputManager");
            Register<ISceneManager>("SceneManager");
            Register<IResourceManager>("ResourceManager");
            Register<IGameDataManager>("GameDataManager");
            Register<IPoolManager>("PoolManager");
            Register<IUIManager>("UIManage");
        }
        
        public void Register<T>(StringName serviceName) where T : class, IService
        {
            var type = typeof(T);
            
            var service = (T)Activator.CreateInstance(type, []);

            if (service != null)
            {
                _services.Remove(serviceName);
                _services.Add(serviceName, service);
            
                service.Locator = this;
                service.Initialize();
            }
            
            GD.Print($"Service {serviceName} registered successfully");
        }
        
        public T GetService<T>(StringName serviceName) where T : class, IService
        {
            if (_services.TryGetValue(serviceName, out var service))
            {
                return service as T;
            }
            
            GD.PrintErr($"Service {serviceName} not found!");
            return null;
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
