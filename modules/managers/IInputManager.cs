// Scripts/Managers/InputManager.cs
using System;
using System.Collections.Generic;
using framework.core.interfaces;
using framework.core.services;
using Godot;

namespace framework.modules.managers
{
    public partial class IInputManager : Node, IService, IUpdateable
    {
        private readonly Dictionary<string, InputAction> _actions = new();
        private readonly Dictionary<string, float> _axes = new();
        private Vector2 _mousePosition =  Vector2.Zero;
        private Vector2 _mouseDelta =  Vector2.Zero;
        private bool _inputEnabled = true;
        
        public bool InputEnabled
        {
            get => _inputEnabled;
            set => _inputEnabled = value;
        }
        
        public Vector2 MousePosition => _mousePosition;
        public Vector2 MouseDelta => _mouseDelta;

        public ServiceLocator Locator { get; set; }

        public void Initialize()
        {
            SetupDefaultActions();
        }
        
        private void SetupDefaultActions()
        {
            // 设置默认输入动作
            RegisterAction("move_left", new[] { Key.A, Key.Left });
            RegisterAction("move_right", new[] { Key.D, Key.Right });
            RegisterAction("move_up", new[] { Key.W, Key.Up });
            RegisterAction("move_down", new[] { Key.S, Key.Down });
            RegisterAction("jump", new[] { Key.Space });
            RegisterAction("interact", new[] { Key.E });
            RegisterAction("pause", new[] { Key.Escape });
            
            // 设置轴
            RegisterAxis("horizontal", "move_left", "move_right");
            RegisterAxis("vertical", "move_up", "move_down");
        }
        
        public void RegisterAction(string name, Key[] keys, MouseButton[] mouseButtons = null)
        {
            _actions[name] = new InputAction
            {
                Name = name,
                Keys = keys ?? Array.Empty<Key>(),
                MouseButtons = mouseButtons ?? Array.Empty<MouseButton>()
            };
        }
        
        public void RegisterAxis(string name, string negativeAction, string positiveAction)
        {
            _axes[name] = 0f;
        }
        
        public bool IsActionPressed(string action)
        {
            if (!_inputEnabled || !_actions.ContainsKey(action))
                return false;
            
            return Input.IsActionPressed(action);
        }
        
        public bool IsActionJustPressed(string action)
        {
            if (!_inputEnabled || !_actions.ContainsKey(action))
                return false;
            
            return Input.IsActionJustPressed(action);
        }
        
        public bool IsActionJustReleased(string action)
        {
            if (!_inputEnabled || !_actions.ContainsKey(action))
                return false;
            
            return Input.IsActionJustReleased(action);
        }
        
        public float GetAxis(string axis)
        {
            if (!_inputEnabled)
                return 0f;
            
            return Input.GetAxis(axis + "_negative", axis + "_positive");
        }
        
        public Vector2 GetVector(string negativeX, string positiveX, string negativeY, string positiveY)
        {
            if (!_inputEnabled)
                return Vector2.Zero;
            
            return Input.GetVector(negativeX, positiveX, negativeY, positiveY);
        }
        
        public void Update(float deltaTime)
        {
            if (!_inputEnabled)
                return;
            
            // 更新鼠标位置
            // var newMousePos = GetViewport().GetMousePosition();
            // _mouseDelta = newMousePos - _mousePosition;
            // _mousePosition = newMousePos;
        }
        
        public void StartVibration(int device, float weakMagnitude, float strongMagnitude, float duration)
        {
            Input.StartJoyVibration(device, weakMagnitude, strongMagnitude, duration);
        }
        
        public void StopVibration(int device)
        {
            Input.StopJoyVibration(device);
        }
        
        public void SetMouseMode(Input.MouseModeEnum mode)
        {
            Input.MouseMode = mode;
        }
        
        public void Shutdown()
        {
            _actions.Clear();
            _axes.Clear();
        }
        
        private class InputAction
        {
            public string Name { get; set; }
            public Key[] Keys { get; set; }
            public MouseButton[] MouseButtons { get; set; }
        }
    }
}
