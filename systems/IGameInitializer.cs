using framework.model;
using Godot;
using framework.systems.core.events;
using framework.systems.core.services;
using framework.systems.managers;

namespace framework.systems
{
    public partial class IGameInitializer : Node
    {
        private bool _skipToMainMenu = true;
        
        public override void _Ready()
        {
            SetName("GameInitializer");
            InitializeGame();
        }
        
        public async void InitializeGame()
        {
            GD.Print("Game initialization started...");
            
            // 创建服务定位器
            var serviceLocator = new ServiceLocator();
            AddChild(serviceLocator);
            
            // 等待服务初始化
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            
            // 订阅全局事件
            SubscribeToGlobalEvents();
            
            // 加载游戏设置
            LoadGameSettings();
            
            // 预加载资源
            PreloadEssentialResources();
            
            GD.Print("Game initialization completed!");
        }
        
        private void SubscribeToGlobalEvents()
        {
            var eventBus = ServiceLocator.Instance.Get<EventBus>();
            if (eventBus != null)
            {
                // 订阅全局事件
                eventBus.Subscribe<ApplicationPausedEvent>(OnApplicationPaused);
                eventBus.Subscribe<ApplicationResumedEvent>(OnApplicationResumed);
            }
        }
        
        private void LoadGameSettings()
        {
            var saveSystem = ServiceLocator.Instance.Get<ISaveSystem>();
            var settings = saveSystem?.LoadData<GameSettings>("game_settings");
            
            if (settings != null)
            {
                ApplyGameSettings(settings);
            }
            else
            {
                // 使用默认设置
                ApplyGameSettings(new GameSettings());
            }
        }
        
        private void ApplyGameSettings(GameSettings settings)
        {
            // 应用显示设置
            if (settings.Fullscreen)
            {
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
            }
            else
            {
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                DisplayServer.WindowSetSize(new Vector2I(settings.ResolutionWidth, settings.ResolutionHeight));
            }
            
            // 应用音频设置
            var audioManager = ServiceLocator.Instance.Get<IAudioManager>();
            if (audioManager != null)
            {
                audioManager.MasterVolume = settings.MasterVolume;
                audioManager.BgmVolume = settings.BgmVolume;
                audioManager.SfxVolume = settings.SfxVolume;
            }
            
            // 应用其他设置
            // Engine.MaxFps = settings.TargetFps;
            ProjectSettings.SetSetting("rendering/anti_aliasing/quality/msaa", settings.AntiAliasing);
        }
        
        private void PreloadEssentialResources()
        {
            var resourceManager = ServiceLocator.Instance.Get<IResourceManager>();
            if (resourceManager != null)
            {
                // 预加载必要资源
                string[] essentialResources = {
                    "res://packages/scenes/ui/main_menu.tscn",
                    // "res://UI/LoadingScreen.tscn",
                    // "res://UI/PauseMenu.tscn",
                    // "res://Audio/UI/button_click.ogg",
                    // "res://Audio/UI/menu_open.ogg"
                };
                
                resourceManager.PreloadResources(essentialResources);
            }
        }
        
        private void OnApplicationPaused(ApplicationPausedEvent evt)
        {
            GetTree().Paused = true;
        }
        
        private void OnApplicationResumed(ApplicationResumedEvent evt)
        {
            GetTree().Paused = false;
        }
        
        public override void _Notification(int what)
        {
            var eventBus = ServiceLocator.Instance?.Get<EventBus>();
            long notification = what;
            
            switch (notification)
            {
                case NotificationWMCloseRequest:
                    SaveGameSettings();
                    GetTree().Quit();
                    break;
                    
                case NotificationApplicationPaused:
                    eventBus?.Publish(new ApplicationPausedEvent());
                    break;
                    
                case NotificationApplicationResumed:
                    eventBus?.Publish(new ApplicationResumedEvent());
                    break;
            }
        }
        
        private void SaveGameSettings()
        {
            var saveSystem = ServiceLocator.Instance.Get<ISaveSystem>();
            var settings = new GameSettings
            {
                Fullscreen = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen,
                ResolutionWidth = DisplayServer.WindowGetSize().X,
                ResolutionHeight = DisplayServer.WindowGetSize().Y,
                MasterVolume = ServiceLocator.Instance.Get<IAudioManager>()?.MasterVolume ?? 1.0f,
                BgmVolume = ServiceLocator.Instance.Get<IAudioManager>()?.BgmVolume ?? 1.0f,
                SfxVolume = ServiceLocator.Instance.Get<IAudioManager>()?.SfxVolume ?? 1.0f,
                // TargetFps = Engine.MaxFps,
                AntiAliasing = (int)ProjectSettings.GetSetting("rendering/anti_aliasing/quality/msaa")
            };
            
            saveSystem?.SaveData("game_settings", settings);
        }
    }
    
    public class ApplicationPausedEvent : IGameEvent { }
    public class ApplicationResumedEvent : IGameEvent { }
}

