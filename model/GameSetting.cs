// Scripts/Data/GameSettings.cs
using System;
using System.Collections.Generic;
using Godot;
using framework.systems.core.services;
using framework.systems.managers;


namespace framework.model
{
    [Serializable]
    public class GameSettings
    {
        // 显示设置
        public bool Fullscreen { get; set; } = false;
        public int ResolutionWidth { get; set; } = 1920;
        public int ResolutionHeight { get; set; } = 1080;
        public int WindowMode { get; set; } = 0; // 0=Windowed, 1=Borderless, 2=Fullscreen
        public bool VSync { get; set; } = true;
        public int TargetFPS { get; set; } = 60;
        public int AntiAliasing { get; set; } = 2;
        public float Brightness { get; set; } = 1.0f;
        public float Contrast { get; set; } = 1.0f;
        public float Gamma { get; set; } = 1.0f;
        
        // 音频设置
        public float MasterVolume { get; set; } = 1.0f;
        public float BgmVolume { get; set; } = 0.8f;
        public float SfxVolume { get; set; } = 1.0f;
        public float VoiceVolume { get; set; } = 1.0f;
        public float AmbientVolume { get; set; } = 0.7f;
        public bool MuteOnFocusLoss { get; set; } = true;
        
        // 游戏设置
        public string Language { get; set; } = "en";
        public bool ShowFPS { get; set; } = false;
        public bool ShowTutorials { get; set; } = true;
        public float TextSpeed { get; set; } = 1.0f;
        public bool AutoSave { get; set; } = true;
        public int AutoSaveInterval { get; set; } = 300; // 秒
        
        // 控制设置
        public float MouseSensitivity { get; set; } = 1.0f;
        public bool InvertMouseY { get; set; } = false;
        public bool InvertMouseX { get; set; } = false;
        public float GamepadSensitivity { get; set; } = 1.0f;
        public bool GamepadVibration { get; set; } = true;
        public float GamepadDeadzone { get; set; } = 0.2f;
        
        // 辅助功能
        public bool Subtitles { get; set; } = false;
        public float SubtitleSize { get; set; } = 1.0f;
        public bool ColorblindMode { get; set; } = false;
        public int ColorblindType { get; set; } = 0; // 0=None, 1=Protanopia, 2=Deuteranopia, 3=Tritanopia
        public bool ReduceMotion { get; set; } = false;
        public bool ScreenShake { get; set; } = true;
        
        // 图形质量设置
        public int GraphicsQuality { get; set; } = 2; // 0=Low, 1=Medium, 2=High, 3=Ultra
        public int ShadowQuality { get; set; } = 2;
        public int TextureQuality { get; set; } = 2;
        public int EffectsQuality { get; set; } = 2;
        public bool PostProcessing { get; set; } = true;
        public bool MotionBlur { get; set; } = true;
        public bool DepthOfField { get; set; } = true;
        public bool Bloom { get; set; } = true;
        public bool AmbientOcclusion { get; set; } = true;
        
        // 网络设置（如果需要）
        public string PlayerName { get; set; } = "Player";
        public int NetworkPort { get; set; } = 7777;
        public int MaxPing { get; set; } = 200;
        
        // 键位绑定
        public Dictionary<string, InputBinding> KeyBindings { get; set; } = new();
        
        // 获取默认设置
        public static GameSettings GetDefault()
        {
            var settings = new GameSettings();
            settings.InitializeDefaultKeyBindings();
            return settings;
        }
        
private void InitializeDefaultKeyBindings()
{
    KeyBindings = new Dictionary<string, InputBinding>
    {
        // 移动使用摇杆轴而不是按钮
        ["move_forward"] = new InputBinding 
        { 
            Keys = new[] { Key.W }, 
            GamepadAxes = new[] { JoyAxis.LeftY }, // 负值表示向上
            AxisValue = -1.0f 
        },
        ["move_backward"] = new InputBinding 
        { 
            Keys = new[] { Key.S }, 
            GamepadAxes = new[] { JoyAxis.LeftY }, // 正值表示向下
            AxisValue = 1.0f 
        },
        ["move_left"] = new InputBinding 
        { 
            Keys = new[] { Key.A }, 
            GamepadAxes = new[] { JoyAxis.LeftX }, // 负值表示向左
            AxisValue = -1.0f 
        },
        ["move_right"] = new InputBinding 
        { 
            Keys = new[] { Key.D }, 
            GamepadAxes = new[] { JoyAxis.LeftX }, // 正值表示向右
            AxisValue = 1.0f 
        },
        
        // 按钮映射保持不变
        ["jump"] = new InputBinding 
        { 
            Keys = new[] { Key.Space }, 
            GamepadButtons = new[] { JoyButton.A } 
        },
        ["interact"] = new InputBinding 
        { 
            Keys = new[] { Key.E }, 
            GamepadButtons = new[] { JoyButton.X } 
        },
        ["attack"] = new InputBinding 
        { 
            MouseButtons = new[] { MouseButton.Left }, 
            GamepadButtons = new[] { JoyButton.RightShoulder } 
        },
        ["block"] = new InputBinding 
        { 
            MouseButtons = new[] { MouseButton.Right }, 
            GamepadButtons = new[] { JoyButton.LeftShoulder } 
        },
        ["pause"] = new InputBinding 
        { 
            Keys = new[] { Key.Escape }, 
            GamepadButtons = new[] { JoyButton.Start } 
        },
        ["inventory"] = new InputBinding 
        { 
            Keys = new[] { Key.I, Key.Tab }, 
            GamepadButtons = new[] { JoyButton.Back } 
        },
        
        // 额外的常用绑定
        ["sprint"] = new InputBinding 
        { 
            Keys = new[] { Key.Shift }, 
            GamepadButtons = new[] { JoyButton.LeftStick } // 按下左摇杆
        },
        ["crouch"] = new InputBinding 
        { 
            Keys = new[] { Key.Ctrl }, 
            GamepadButtons = new[] { JoyButton.B } 
        },
        ["reload"] = new InputBinding 
        { 
            Keys = new[] { Key.R }, 
            GamepadButtons = new[] { JoyButton.Y } 
        },
        ["map"] = new InputBinding 
        { 
            Keys = new[] { Key.M }, 
            GamepadButtons = new[] { JoyButton.Back } 
        },
        
        // 使用扳机的动作
        ["aim"] = new InputBinding 
        { 
            MouseButtons = new[] { MouseButton.Right }, 
            GamepadAxes = new[] { JoyAxis.TriggerLeft },
            AxisValue = 0.5f // 扳机按下超过50%
        },
        ["fire"] = new InputBinding 
        { 
            MouseButtons = new[] { MouseButton.Left }, 
            GamepadAxes = new[] { JoyAxis.TriggerRight },
            AxisValue = 0.5f
        },
        
        // 相机控制（右摇杆）
        ["camera_up"] = new InputBinding 
        { 
            GamepadAxes = new[] { JoyAxis.RightY },
            AxisValue = -1.0f
        },
        ["camera_down"] = new InputBinding 
        { 
            GamepadAxes = new[] { JoyAxis.RightY },
            AxisValue = 1.0f
        },
        ["camera_left"] = new InputBinding 
        { 
            GamepadAxes = new[] { JoyAxis.RightX },
            AxisValue = -1.0f
        },
        ["camera_right"] = new InputBinding 
        { 
            GamepadAxes = new[] { JoyAxis.RightX },
            AxisValue = 1.0f
        }
    };
}
        
        public void ApplySettings()
        {
            // 应用显示设置
            ApplyDisplaySettings();
            
            // 应用音频设置
            ApplyAudioSettings();
            
            // 应用图形设置
            ApplyGraphicsSettings();
            
            // 应用输入设置
            ApplyInputSettings();
        }
        
        private void ApplyDisplaySettings()
        {
            // 设置窗口模式
            switch (WindowMode)
            {
                case 0: // Windowed
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                    DisplayServer.WindowSetSize(new Vector2I(ResolutionWidth, ResolutionHeight));
                    break;
                case 1: // Borderless
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.Maximized);
                    DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
                    break;
                case 2: // Fullscreen
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                    break;
            }
            
            // 设置垂直同步
            if (VSync)
            {
                DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
            }
            else
            {
                DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
            }
            
            // 设置目标帧率
            Engine.MaxFps = TargetFPS;
            
            // 设置抗锯齿
            ProjectSettings.SetSetting("rendering/anti_aliasing/quality/msaa_2d", AntiAliasing);
            ProjectSettings.SetSetting("rendering/anti_aliasing/quality/msaa_3d", AntiAliasing);
        }
        
        private void ApplyAudioSettings()
        {
            var audioManager = ServiceLocator.Instance?.Get<IAudioManager>();
            if (audioManager != null)
            {
                audioManager.MasterVolume = MasterVolume;
                audioManager.BgmVolume = BgmVolume;
                audioManager.SfxVolume = SfxVolume;
                // 如果AudioManager支持更多音频类型
                // audioManager.VoiceVolume = VoiceVolume;
                // audioManager.AmbientVolume = AmbientVolume;
            }
            
            // 设置音频总线音量
            var masterBusIdx = AudioServer.GetBusIndex("Master");
            if (masterBusIdx >= 0)
            {
                AudioServer.SetBusVolumeDb(masterBusIdx, LinearToDb(MasterVolume));
            }
        }
        
        private void ApplyGraphicsSettings()
        {
            // 根据图形质量预设应用设置
            switch (GraphicsQuality)
            {
                case 0: // Low
                    ApplyLowGraphicsSettings();
                    break;
                case 1: // Medium
                    ApplyMediumGraphicsSettings();
                    break;
                case 2: // High
                    ApplyHighGraphicsSettings();
                    break;
                case 3: // Ultra
                    ApplyUltraGraphicsSettings();
                    break;
            }
            
            // 应用单独的图形设置
            ProjectSettings.SetSetting("rendering/shadows/directional_shadow/size", GetShadowSize(ShadowQuality));
            ProjectSettings.SetSetting("rendering/textures/default_filters/texture_mipmap_bias", GetTextureMipmapBias(TextureQuality));
            
            // 后处理效果
            ProjectSettings.SetSetting("rendering/environment/glow/enabled", Bloom);
            ProjectSettings.SetSetting("rendering/environment/ssao/enabled", AmbientOcclusion);
            ProjectSettings.SetSetting("rendering/camera/depth_of_field/enabled", DepthOfField);
        }
        
        private void ApplyInputSettings()
        {
            // 应用键位绑定
            foreach (var binding in KeyBindings)
            {
                ApplyInputBinding(binding.Key, binding.Value);
            }
            
            // 设置输入灵敏度
            ProjectSettings.SetSetting("input_devices/pointing/emulate_mouse_from_touch", true);
        }
        
        private void ApplyInputBinding(string action, InputBinding binding)
        {
            // 清除现有绑定
            if (InputMap.HasAction(action))
            {
                InputMap.ActionEraseEvents(action);
            }
            else
            {
                InputMap.AddAction(action);
            }
    
            // 添加键盘按键
            foreach (var key in binding.Keys)
            {
                var keyEvent = new InputEventKey { Keycode = key };
                InputMap.ActionAddEvent(action, keyEvent);
            }
    
            // 添加鼠标按钮
            foreach (var button in binding.MouseButtons)
            {
                var mouseEvent = new InputEventMouseButton { ButtonIndex = button };
                InputMap.ActionAddEvent(action, mouseEvent);
            }
    
            // 添加手柄按钮
            foreach (var button in binding.GamepadButtons)
            {
                var joyEvent = new InputEventJoypadButton { ButtonIndex = button };
                InputMap.ActionAddEvent(action, joyEvent);
            }
    
            // 添加手柄轴
            foreach (var axis in binding.GamepadAxes)
            {
                var joyAxisEvent = new InputEventJoypadMotion 
                { 
                    Axis = axis,
                    AxisValue = binding.AxisValue
                };
                InputMap.ActionAddEvent(action, joyAxisEvent);
            }
    
            // 设置动作的死区
            if (binding.GamepadAxes.Length > 0)
            {
                InputMap.ActionSetDeadzone(action, binding.DeadZone);
            }
        }
        
        private void ApplyLowGraphicsSettings()
        {
            ShadowQuality = 0;
            TextureQuality = 0;
            EffectsQuality = 0;
            PostProcessing = false;
            MotionBlur = false;
            DepthOfField = false;
            Bloom = false;
            AmbientOcclusion = false;
        }
        
        private void ApplyMediumGraphicsSettings()
        {
            ShadowQuality = 1;
            TextureQuality = 1;
            EffectsQuality = 1;
            PostProcessing = true;
            MotionBlur = false;
            DepthOfField = false;
            Bloom = true;
            AmbientOcclusion = false;
        }
        
        private void ApplyHighGraphicsSettings()
        {
            ShadowQuality = 2;
            TextureQuality = 2;
            EffectsQuality = 2;
            PostProcessing = true;
            MotionBlur = true;
            DepthOfField = true;
            Bloom = true;
            AmbientOcclusion = true;
        }
        
        private void ApplyUltraGraphicsSettings()
        {
            ShadowQuality = 3;
            TextureQuality = 3;
            EffectsQuality = 3;
            PostProcessing = true;
            MotionBlur = true;
            DepthOfField = true;
            Bloom = true;
            AmbientOcclusion = true;
        }
        
        private int GetShadowSize(int quality)
        {
            return quality switch
            {
                0 => 1024,
                1 => 2048,
                2 => 4096,
                3 => 8192,
                _ => 2048
            };
        }
        
        private float GetTextureMipmapBias(int quality)
        {
            return quality switch
            {
                0 => 1.0f,
                1 => 0.5f,
                2 => 0.0f,
                3 => -0.5f,
                _ => 0.0f
            };
        }
        
        private float LinearToDb(float linear)
        {
            return 20.0f * Mathf.Log(linear) / Mathf.Log(10.0f);
        }
    }
    
    [Serializable]
    public class InputBinding
    {
        public Key[] Keys { get; set; } = Array.Empty<Key>();
        public MouseButton[] MouseButtons { get; set; } = Array.Empty<MouseButton>();
        public JoyButton[] GamepadButtons { get; set; } = Array.Empty<JoyButton>();
        public JoyAxis[] GamepadAxes { get; set; } = Array.Empty<JoyAxis>();
        public float AxisValue { get; set; } = 0.0f; // 轴的目标值
        public float DeadZone { get; set; } = 0.2f; // 死区
    }
}
