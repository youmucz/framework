// Scripts/Managers/SettingsManager.cs
using System;
using framework.model;
using framework.core.services;
using Godot;


namespace framework.modules.managers
{
    public class ISettingsManager : IService
    {
        private GameSettings _currentSettings;
        private GameSettings _pendingSettings;
        
        public event Action<GameSettings> OnSettingsChanged;
        public event Action<string, object> OnSettingChanged;
        
        public GameSettings CurrentSettings => _currentSettings;

        public ServiceLocator Locator { get; set; }

        public void Initialize()
        {
            LoadSettings();
        }
        
        private void LoadSettings()
        {
            /*var saveSystem = ServiceLocator.Instance.Get<ISaveSystem>();
            _currentSettings = saveSystem?.LoadData<GameSettings>("game_settings") ?? GameSettings.GetDefault();
            _currentSettings.ApplySettings();*/
        }
        
        public void SaveSettings()
        {
            var saveSystem = ServiceLocator.Instance.GetService<ISaveSystem>("SaveSystem");
            saveSystem?.SaveData("game_settings", _currentSettings);
        }
        
        public void ApplySettings(GameSettings settings)
        {
            _currentSettings = settings;
            _currentSettings.ApplySettings();
            SaveSettings();
            OnSettingsChanged?.Invoke(_currentSettings);
        }
        
        public void BeginSettingsChange()
        {
            _pendingSettings = new GameSettings
            {
                // 复制当前设置
                Fullscreen = _currentSettings.Fullscreen,
                ResolutionWidth = _currentSettings.ResolutionWidth,
                ResolutionHeight = _currentSettings.ResolutionHeight,
                WindowMode = _currentSettings.WindowMode,
                VSync = _currentSettings.VSync,
                TargetFPS = _currentSettings.TargetFPS,
                AntiAliasing = _currentSettings.AntiAliasing,
                MasterVolume = _currentSettings.MasterVolume,
                BgmVolume = _currentSettings.BgmVolume,
                SfxVolume = _currentSettings.SfxVolume,
                Language = _currentSettings.Language,
                GraphicsQuality = _currentSettings.GraphicsQuality,
                // ... 复制其他设置
            };
        }
        
        public void SetPendingSetting<T>(string settingName, T value)
        {
            if (_pendingSettings == null) return;
            
            var property = typeof(GameSettings).GetProperty(settingName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(_pendingSettings, value);
                OnSettingChanged?.Invoke(settingName, value);
            }
        }
        
        public void ApplyPendingSettings()
        {
            if (_pendingSettings != null)
            {
                ApplySettings(_pendingSettings);
                _pendingSettings = null;
            }
        }
        
        public void CancelPendingSettings()
        {
            _pendingSettings = null;
        }
        
        public void ResetToDefault()
        {
            ApplySettings(GameSettings.GetDefault());
        }
        
        public void Shutdown()
        {
            SaveSettings();
        }
    }
}
