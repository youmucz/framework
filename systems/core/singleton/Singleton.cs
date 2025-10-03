using Godot;
using System;

namespace framework.systems.core.singleton
{
    public abstract partial class Singleton<T> : Node where T : Node
    {
        private static T _instance;
        
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    GD.Print($"Singleton {typeof(T).Name} is not initialized!");
                }
                return _instance;
            }
        }
        
        public static bool HasInstance => _instance != null;
        
        public override void _EnterTree()
        {
            if (_instance != null && _instance != this)
            {
                QueueFree();
                return;
            }
            
            _instance = this as T;
            OnSingletonInitialized();
        }
        
        public override void _ExitTree()
        {
            if (_instance == this)
            {
                _instance = null;
                OnSingletonDestroyed();
            }
        }
        
        protected virtual void OnSingletonInitialized() { }
        protected virtual void OnSingletonDestroyed() { }
    }
}
