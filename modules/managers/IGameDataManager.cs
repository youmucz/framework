// Scripts/Managers/GameDataManager.cs
using System.Collections.Generic;
using framework.core.services;
using Godot;

namespace framework.modules.managers
{
    public class IGameDataManager : IService
    {
        private readonly Dictionary<string, Resource> _gameData = new();

        public ServiceLocator Locator { get; set; }

        public void Initialize()
        {
            LoadAllGameData();
        }
        
        private void LoadAllGameData()
        {
            // 加载所有游戏数据
            // LoadDataDirectory("res://Data/Items/");
            // LoadDataDirectory("res://Data/Characters/");
            // LoadDataDirectory("res://Data/Levels/");
            // LoadDataDirectory("res://Data/Dialogues/");
        }
        
        private void LoadDataDirectory(string path)
        {
            var dir = DirAccess.Open(path);
            if (dir == null) return;
            
            dir.ListDirBegin();
            var fileName = dir.GetNext();
            
            while (!string.IsNullOrEmpty(fileName))
            {
                if (fileName.EndsWith(".tres") || fileName.EndsWith(".res"))
                {
                    var fullPath = path + fileName;
                    var data = GD.Load<Resource>(fullPath);
                    if (data != null)
                    {
                        var key = System.IO.Path.GetFileNameWithoutExtension(fileName);
                        _gameData[key] = data;
                    }
                }
                fileName = dir.GetNext();
            }
        }
        
        public T GetData<T>(string key) where T : Resource
        {
            if (_gameData.TryGetValue(key, out var data))
            {
                return data as T;
            }
            
            GD.PrintErr($"Game data '{key}' not found!");
            return null;
        }
        
        public T[] GetAllData<T>() where T : Resource
        {
            var result = new List<T>();
            
            foreach (var data in _gameData.Values)
            {
                if (data is T typedData)
                {
                    result.Add(typedData);
                }
            }
            
            return result.ToArray();
        }
        
        public void Shutdown()
        {
            _gameData.Clear();
        }
    }
}
