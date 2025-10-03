// Scripts/Managers/AudioManager.cs
using System.Collections.Generic;
using Godot;
using framework.systems.core;
using framework.systems.core.services;


namespace framework.systems.managers
{
    public partial class IAudioManager : Node, IService
    {
        private readonly Dictionary<string, AudioStream> _audioCache = new();
        private readonly List<AudioStreamPlayer> _sfxPlayers = new();
        private readonly List<AudioStreamPlayer2D> _sfx2DPlayers = new();
        private AudioStreamPlayer _bgmPlayer;
        private AudioStreamPlayer _ambientPlayer;
        
        private const int MaxSfxPlayers = 16;
        private float _masterVolume = 1.0f;
        private float _bgmVolume = 1.0f;
        private float _sfxVolume = 1.0f;
        
        public float MasterVolume
        {
            get => _masterVolume;
            set
            {
                _masterVolume = Mathf.Clamp(value, 0, 1);
                UpdateVolumes();
            }
        }
        
        public float BgmVolume
        {
            get => _bgmVolume;
            set
            {
                _bgmVolume = Mathf.Clamp(value, 0, 1);
                UpdateVolumes();
            }
        }
        
        public float SfxVolume
        {
            get => _sfxVolume;
            set
            {
                _sfxVolume = Mathf.Clamp(value, 0, 1);
                UpdateVolumes();
            }
        }
        
        public void Initialize()
        {
            // 初始化BGM播放器
            _bgmPlayer = new AudioStreamPlayer { Bus = "BGM" };
            AddChild(_bgmPlayer);
            
            // 初始化环境音播放器
            _ambientPlayer = new AudioStreamPlayer { Bus = "Ambient" };
            AddChild(_ambientPlayer);
            
            // 初始化SFX播放器池
            for (int i = 0; i < MaxSfxPlayers; i++)
            {
                var player = new AudioStreamPlayer { Bus = "SFX" };
                AddChild(player);
                _sfxPlayers.Add(player);
                
                var player2D = new AudioStreamPlayer2D { Bus = "SFX" };
                AddChild(player2D);
                _sfx2DPlayers.Add(player2D);
            }
            
            LoadAudioSettings();
        }
        
        public void PlayBGM(string audioPath, bool loop = true, float fadeTime = 1.0f)
        {
            var audio = LoadAudio(audioPath);
            if (audio == null) return;
            
            if (_bgmPlayer.Playing && fadeTime > 0)
            {
                var tween = CreateTween();
                tween.TweenProperty(_bgmPlayer, "volume_db", -80.0f, fadeTime);
                tween.TweenCallback(Callable.From(() =>
                {
                    _bgmPlayer.Stream = audio;
                    _bgmPlayer.VolumeDb = LinearToDb(_bgmVolume * _masterVolume);
                    _bgmPlayer.Play();
                }));
            }
            else
            {
                _bgmPlayer.Stream = audio;
                _bgmPlayer.VolumeDb = LinearToDb(_bgmVolume * _masterVolume);
                _bgmPlayer.Play();
            }
        }
        
        public void StopBGM(float fadeTime = 1.0f)
        {
            if (!_bgmPlayer.Playing) return;
            
            if (fadeTime > 0)
            {
                var tween = CreateTween();
                tween.TweenProperty(_bgmPlayer, "volume_db", -80.0f, fadeTime);
                tween.TweenCallback(Callable.From(() => _bgmPlayer.Stop()));
            }
            else
            {
                _bgmPlayer.Stop();
            }
        }
        
        public void PlaySFX(string audioPath, float volumeScale = 1.0f)
        {
            var audio = LoadAudio(audioPath);
            if (audio == null) return;
            
            var player = GetAvailableSfxPlayer();
            if (player != null)
            {
                player.Stream = audio;
                player.VolumeDb = LinearToDb(_sfxVolume * _masterVolume * volumeScale);
                player.Play();
            }
        }
        
        public void PlaySFX2D(string audioPath, Vector2 position, float volumeScale = 1.0f)
        {
            var audio = LoadAudio(audioPath);
            if (audio == null) return;
            
            var player = GetAvailable2DSfxPlayer();
            if (player != null)
            {
                player.GlobalPosition = position;
                player.Stream = audio;
                player.VolumeDb = LinearToDb(_sfxVolume * _masterVolume * volumeScale);
                player.Play();
            }
        }
        
        private AudioStreamPlayer GetAvailableSfxPlayer()
        {
            foreach (var player in _sfxPlayers)
            {
                if (!player.Playing)
                    return player;
            }
            return _sfxPlayers[0]; // 如果都在播放，使用第一个
        }
        
        private AudioStreamPlayer2D GetAvailable2DSfxPlayer()
        {
            foreach (var player in _sfx2DPlayers)
            {
                if (!player.Playing)
                    return player;
            }
            return _sfx2DPlayers[0];
        }
        
        private AudioStream LoadAudio(string path)
        {
            if (_audioCache.TryGetValue(path, out var cached))
                return cached;
            
            var audio = GD.Load<AudioStream>(path);
            if (audio != null)
            {
                _audioCache[path] = audio;
            }
            else
            {
                GD.PrintErr($"Failed to load audio: {path}");
            }
            
            return audio;
        }
        
        private void UpdateVolumes()
        {
            _bgmPlayer.VolumeDb = LinearToDb(_bgmVolume * _masterVolume);
            _ambientPlayer.VolumeDb = LinearToDb(_masterVolume);
            
            foreach (var player in _sfxPlayers)
            {
                if (player.Playing)
                    player.VolumeDb = LinearToDb(_sfxVolume * _masterVolume);
            }
        }
        
        private float LinearToDb(float linear)
        {
            return linear > 0 ? 20 * Mathf.Log(linear) : -80.0f;
        }
        
        private void LoadAudioSettings()
        {
            var saveSystem = ServiceLocator.Instance.Get<ISaveSystem>();
            if (saveSystem != null)
            {
                var settings = saveSystem.LoadData<AudioSettings>("audio_settings");
                if (settings != null)
                {
                    MasterVolume = settings.MasterVolume;
                    BgmVolume = settings.BgmVolume;
                    SfxVolume = settings.SfxVolume;
                }
            }
        }
        
        public void SaveAudioSettings()
        {
            var saveSystem = ServiceLocator.Instance.Get<ISaveSystem>();
            saveSystem?.SaveData("audio_settings", new AudioSettings
            {
                MasterVolume = MasterVolume,
                BgmVolume = BgmVolume,
                SfxVolume = SfxVolume
            });
        }
        
        public void Shutdown()
        {
            SaveAudioSettings();
            _audioCache.Clear();
        }
    }
    
    [System.Serializable]
    public class AudioSettings
    {
        public float MasterVolume { get; set; } = 1.0f;
        public float BgmVolume { get; set; } = 1.0f;
        public float SfxVolume { get; set; } = 1.0f;
    }
}
