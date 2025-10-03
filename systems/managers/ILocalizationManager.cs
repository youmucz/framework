// Scripts/Managers/LocalizationManager.cs
using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;
using framework.systems.core.services;


namespace framework.systems.managers
{
    public abstract class ILocalizationManager : IService
    {
        private Dictionary<string, Dictionary<string, string>> _translations = new();
        private string _currentLanguage = "en";
        private const string LocalizationPath = "res://Localization/";
        
        public event Action<string> OnLanguageChanged;
        
        public string CurrentLanguage => _currentLanguage;
        
        public void Initialize()
        {
            LoadAllLanguages();
            
            // 从设置中加载语言
            // var saveSystem = ServiceLocator.Instance.Get<SaveSystem>();
            // var settings = saveSystem?.LoadData<GameSettings>("game_settings");
            // if (settings != null && !string.IsNullOrEmpty(settings.Language))
            // {
            //     SetLanguage(settings.Language);
            // }
        }
        
        private void LoadAllLanguages()
        {
            var dir = DirAccess.Open(LocalizationPath);
            if (dir == null)
            {
                GD.PrintErr($"Localization directory not found: {LocalizationPath}");
                return;
            }
            
            dir.ListDirBegin();
            var fileName = dir.GetNext();
            
            while (!string.IsNullOrEmpty(fileName))
            {
                if (fileName.EndsWith(".json"))
                {
                    var langCode = System.IO.Path.GetFileNameWithoutExtension(fileName);
                    LoadLanguage(langCode);
                }
                fileName = dir.GetNext();
            }
        }
        
        private void LoadLanguage(string langCode)
        {
            try
            {
                var filePath = $"{LocalizationPath}{langCode}.json";
                var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
                if (file == null) return;
                
                var json = file.GetAsText();
                file.Close();
                
                var translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (translations != null)
                {
                    _translations[langCode] = translations;
                    GD.Print($"Loaded language: {langCode}");
                }
            }
            catch (Exception e)
            {
                GD.PrintErr($"Failed to load language {langCode}: {e.Message}");
            }
        }
        
        public void SetLanguage(string langCode)
        {
            if (_translations.ContainsKey(langCode))
            {
                _currentLanguage = langCode;
                OnLanguageChanged?.Invoke(langCode);
                
                // 保存语言设置
                // var saveSystem = ServiceLocator.Instance.Get<ISaveSystem>();
                // var settings = saveSystem?.LoadData<GameSettings>("game_settings") ?? new GameSettings();
                // settings.Language = langCode;
                // saveSystem?.SaveData("game_settings", settings);
            }
            else
            {
                GD.PrintErr($"Language not found: {langCode}");
            }
        }
        
        public string GetText(string key, params object[] args)
        {
            if (_translations.TryGetValue(_currentLanguage, out var langDict))
            {
                if (langDict.TryGetValue(key, out var text))
                {
                    if (args.Length > 0)
                    {
                        return string.Format(text, args);
                    }
                    return text;
                }
            }
            
            // 如果找不到翻译，返回key
            return key;
        }
        
        public string[] GetAvailableLanguages()
        {
            var languages = new string[_translations.Count];
            _translations.Keys.CopyTo(languages, 0);
            return languages;
        }
        
        public void Shutdown()
        {
            _translations.Clear();
        }
    }
    
    // 本地化文本组件
    [Tool]
    public partial class LocalizedLabel : Label
    {
        [Export] private string _localizationKey = "";
        private ILocalizationManager _localizationManager;
        
        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;
            
            _localizationManager = ServiceLocator.Instance?.Get<ILocalizationManager>();
            if (_localizationManager != null)
            {
                _localizationManager.OnLanguageChanged += OnLanguageChanged;
                UpdateText();
            }
        }
        
        public override void _ExitTree()
        {
            if (_localizationManager != null)
            {
                _localizationManager.OnLanguageChanged -= OnLanguageChanged;
            }
        }
        
        private void OnLanguageChanged(string language)
        {
            UpdateText();
        }
        
        private void UpdateText()
        {
            if (!string.IsNullOrEmpty(_localizationKey) && _localizationManager != null)
            {
                Text = _localizationManager.GetText(_localizationKey);
            }
        }
        
        public void SetKey(string key)
        {
            _localizationKey = key;
            UpdateText();
        }
    }
}
