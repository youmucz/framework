// Scripts/Managers/UIManager.cs
using System.Collections.Generic;
using framework.core.services;
using Godot;

namespace framework.modules.managers
{
    public partial class IUIManager : Node, IService
    {
        private CanvasLayer _uiLayer;
        private readonly Dictionary<string, Control> _uiPanels = new();
        private readonly Stack<Control> _uiStack = new();

        public ServiceLocator Locator { get; set; }

        public void Initialize()
        {
            _uiLayer = new CanvasLayer { Layer = 10 };
            AddChild(_uiLayer);
        }
        
        public T ShowPanel<T>(string panelPath) where T : Control
        {
            if (_uiPanels.TryGetValue(panelPath, out var existingPanel))
            {
                existingPanel.Show();
                return existingPanel as T;
            }
            
            var packedScene = GD.Load<PackedScene>(panelPath);
            if (packedScene == null)
            {
                GD.PrintErr($"Failed to load UI panel: {panelPath}");
                return null;
            }
            
            var panel = packedScene.Instantiate<T>();
            _uiLayer.AddChild(panel);
            _uiPanels[panelPath] = panel;
            _uiStack.Push(panel);
            
            return panel;
        }
        
        public void HidePanel(string panelPath)
        {
            if (_uiPanels.TryGetValue(panelPath, out var panel))
            {
                panel.Hide();
            }
        }
        
        public void ClosePanel(string panelPath)
        {
            if (_uiPanels.TryGetValue(panelPath, out var panel))
            {
                panel.QueueFree();
                _uiPanels.Remove(panelPath);
                
                // 从栈中移除
                var tempStack = new Stack<Control>();
                while (_uiStack.Count > 0)
                {
                    var stackPanel = _uiStack.Pop();
                    if (stackPanel != panel)
                    {
                        tempStack.Push(stackPanel);
                    }
                }
                while (tempStack.Count > 0)
                {
                    _uiStack.Push(tempStack.Pop());
                }
            }
        }
        
        public void CloseTopPanel()
        {
            if (_uiStack.Count > 0)
            {
                var topPanel = _uiStack.Pop();
                topPanel.QueueFree();
                
                // 从字典中移除
                string keyToRemove = null;
                foreach (var kvp in _uiPanels)
                {
                    if (kvp.Value == topPanel)
                    {
                        keyToRemove = kvp.Key;
                        break;
                    }
                }
                if (keyToRemove != null)
                {
                    _uiPanels.Remove(keyToRemove);
                }
            }
        }
        
        public void CloseAllPanels()
        {
            foreach (var panel in _uiPanels.Values)
            {
                panel.QueueFree();
            }
            _uiPanels.Clear();
            _uiStack.Clear();
        }
        
        public void Shutdown()
        {
            CloseAllPanels();
            _uiLayer?.QueueFree();
        }
    }
}
