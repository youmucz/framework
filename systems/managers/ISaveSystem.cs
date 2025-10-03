// Scripts/Systems/SaveSystem.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using framework.systems.core.events;
using framework.systems.core.services;

namespace framework.systems.managers
{
    public class ISaveSystem : IService
    {
        private string _savePath;
        private const string SaveFileName = "save_{0}.json";
        private const string SettingsFileName = "settings.json";
        
        // 配置 JsonSerializerOptions 以获得更好的性能
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = 
            {
                new Vector2JsonConverter(),
                new DateTimeJsonConverter()
            }
        };
        
        public void Initialize()
        {
            _savePath = OS.GetUserDataDir() + "/saves/";
            
            if (!Directory.Exists(_savePath))
            {
                Directory.CreateDirectory(_savePath);
            }
        }
        
        public void SaveGame(int slot, GameSaveData data)
        {
            try
            {
                var fileName = string.Format(SaveFileName, slot);
                var filePath = Path.Combine(_savePath, fileName);
                
                data.SaveTime = DateTime.Now;
                data.Version = ProjectSettings.GetSetting("application/config/version").ToString();
                
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                File.WriteAllText(filePath, json);
                
                GD.Print($"Game saved to slot {slot}");
                ServiceLocator.Instance.Get<EventBus>()?.Publish(new GameSavedEvent { Slot = slot });
            }
            catch (Exception e)
            {
                GD.PrintErr($"Failed to save game: {e.Message}");
            }
        }
        
        public GameSaveData LoadGame(int slot)
        {
            try
            {
                var fileName = string.Format(SaveFileName, slot);
                var filePath = Path.Combine(_savePath, fileName);
                
                if (!File.Exists(filePath))
                {
                    GD.Print($"Save file not found for slot {slot}");
                    return null;
                }
                
                var json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<GameSaveData>(json, _jsonOptions);
                
                GD.Print($"Game loaded from slot {slot}");
                ServiceLocator.Instance.Get<EventBus>()?.Publish(new GameLoadedEvent { Slot = slot });
                
                return data;
            }
            catch (Exception e)
            {
                GD.PrintErr($"Failed to load game: {e.Message}");
                return null;
            }
        }
        
        public void DeleteSave(int slot)
        {
            var fileName = string.Format(SaveFileName, slot);
            var filePath = Path.Combine(_savePath, fileName);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                GD.Print($"Save deleted from slot {slot}");
            }
        }
        
        public bool SaveExists(int slot)
        {
            var fileName = string.Format(SaveFileName, slot);
            var filePath = Path.Combine(_savePath, fileName);
            return File.Exists(filePath);
        }
        
        public SaveInfo[] GetAllSaves()
        {
            var saves = new List<SaveInfo>();
            
            for (int i = 0; i < 10; i++) // 支持10个存档槽
            {
                if (SaveExists(i))
                {
                    var data = LoadGame(i);
                    if (data != null)
                    {
                        saves.Add(new SaveInfo
                        {
                            Slot = i,
                            SaveTime = data.SaveTime,
                            PlayTime = data.PlayTime,
                            Level = data.Level,
                            PlayerName = data.PlayerName
                        });
                    }
                }
            }
            
            return saves.ToArray();
        }
        
        public void SaveData<T>(string key, T data)
        {
            try
            {
                var filePath = Path.Combine(_savePath, $"{key}.json");
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                GD.PrintErr($"Failed to save data {key}: {e.Message}");
            }
        }
        
        public T LoadData<T>(string key) where T : class
        {
            try
            {
                var filePath = Path.Combine(_savePath, $"{key}.json");
                if (!File.Exists(filePath))
                    return null;
                
                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }
            catch (Exception e)
            {
                GD.PrintErr($"Failed to load data {key}: {e.Message}");
                return null;
            }
        }
        
        public void Shutdown()
        {
            // 清理资源
        }
    }
    
    public class GameSaveData
    {
        public string Version { get; set; }
        public DateTime SaveTime { get; set; }
        public float PlayTime { get; set; }
        public string PlayerName { get; set; }
        public int Level { get; set; }
        
        [JsonConverter(typeof(Vector2JsonConverter))]
        public Vector2 PlayerPosition { get; set; }
        
        public Dictionary<string, object> CustomData { get; set; } = new();
    }
    
    public class SaveInfo
    {
        public int Slot { get; set; }
        public DateTime SaveTime { get; set; }
        public float PlayTime { get; set; }
        public int Level { get; set; }
        public string PlayerName { get; set; }
    }
    
    public class GameSavedEvent : IGameEvent
    {
        public int Slot { get; set; }
    }
    
    public class GameLoadedEvent : IGameEvent
    {
        public int Slot { get; set; }
    }
    
    // 自定义 Vector2 转换器
    public class Vector2JsonConverter : JsonConverter<Vector2>
    {
        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();
            
            float x = 0, y = 0;
            
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return new Vector2(x, y);
                
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();
                    
                    switch (propertyName?.ToLower())
                    {
                        case "x":
                            x = reader.GetSingle();
                            break;
                        case "y":
                            y = reader.GetSingle();
                            break;
                    }
                }
            }
            
            throw new JsonException();
        }
        
        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("x", value.X);
            writer.WriteNumber("y", value.Y);
            writer.WriteEndObject();
        }
    }
    
    // 自定义 DateTime 转换器（可选，用于控制日期格式）
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        private const string DateFormat = "yyyy-MM-dd HH:mm:ss";
        
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString(), DateFormat, null);
        }
        
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormat));
        }
    }
}
