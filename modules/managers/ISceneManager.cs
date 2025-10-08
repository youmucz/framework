// Scripts/Managers/SceneManager.cs
using System;
using System.Threading.Tasks;
using framework.core.events;
using framework.core.services;
using Godot;


namespace framework.modules.managers
{
    public partial class ISceneManager : Node, IService
    {
        private Node _currentScene;
        private SceneTransition _transition;
        private bool _isLoading;
        
        public event Action<string> OnSceneLoadStarted;
        public event Action<string> OnSceneLoadCompleted;

        public ServiceLocator Locator { get; set; }

        public void Initialize()
        {
            Locator.AddChild(this);
            
            _transition = new SceneTransition();
            CallDeferred(Node.MethodName.AddChild, _transition);
        }
        
        public async Task LoadSceneAsync<T>(string scenePath, TransitionType transitionType = TransitionType.Fade) where T : Node
        {
            if (_isLoading) return;
            
            _isLoading = true;
            OnSceneLoadStarted?.Invoke(scenePath);
            
            // 开始过渡效果
            await _transition.StartTransition(transitionType);
            
            // 加载新场景
            var packedScene = GD.Load<PackedScene>(scenePath);
            if (packedScene == null)
            {
                GD.PrintErr($"Failed to load scene: {scenePath}");
                _isLoading = false;
                return;
            }
            
            // 清理当前场景
            _currentScene?.QueueFree();
            
            // 实例化新场景
            _currentScene = packedScene.Instantiate<T>();
            GetTree().Root.AddChild(_currentScene);
            GetTree().CurrentScene = _currentScene;
            
            // 结束过渡效果
            await _transition.EndTransition();
            
            _isLoading = false;
            OnSceneLoadCompleted?.Invoke(scenePath);
            
            // 发布场景加载完成事件
            ServiceLocator.Instance.GetService<IEventBus>("EventBus")?.Publish(new SceneLoadedEvent { ScenePath = scenePath });
        }
        
        public void LoadScene<T>(string scenePath) where T : Node
        {
            _ = LoadSceneAsync<T>(scenePath);
        }
        
        public void ReloadCurrentScene<T>() where T : Node
        {
            if (_currentScene != null)
            {
                var scenePath = _currentScene.SceneFilePath;
                LoadScene<T>(scenePath);
            }
        }
        
        public void Shutdown()
        {
            _transition?.QueueFree();
        }
    }
    
    public enum TransitionType
    {
        None,
        Fade,
        Slide,
        Zoom
    }
    
    public partial class SceneTransition : CanvasLayer
    {
        private ColorRect _overlay;
        private Tween _tween;
        
        public override void _Ready()
        {
            Layer = 100;
            _overlay = new ColorRect
            {
                Color = Colors.Black,
                AnchorRight = 1,
                AnchorBottom = 1,
                MouseFilter = Control.MouseFilterEnum.Ignore
            };
            AddChild(_overlay);
            _overlay.Modulate = new Color(1, 1, 1, 0);
        }
        
        public async Task StartTransition(TransitionType type)
        {
            _tween?.Kill();
            _tween = CreateTween();

            if (_overlay != null)
            {
                switch (type)
                {
                    case TransitionType.Fade:
                        _tween.TweenProperty(_overlay, "modulate:a", 1.0f, 0.3f);
                        break;
                    case TransitionType.Slide:
                        _overlay.Position = new Vector2(-GetViewport().GetVisibleRect().Size.X, 0);
                        _overlay.Modulate = new Color(1, 1, 1, 1);
                        _tween.TweenProperty(_overlay, "position:x", 0, 0.3f);
                        break;
                    case TransitionType.Zoom:
                        _overlay.Scale = Vector2.Zero;
                        _overlay.Modulate = new Color(1, 1, 1, 1);
                        _overlay.PivotOffset = GetViewport().GetVisibleRect().Size / 2;
                        _tween.TweenProperty(_overlay, "scale", Vector2.One, 0.3f);
                        break;
                    case TransitionType.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            
                await ToSignal(_tween, Tween.SignalName.Finished);
            }
        }
        
        public async Task EndTransition()
        {
            _tween?.Kill();
            _tween = CreateTween();
            _tween.TweenProperty(_overlay, "modulate:a", 0.0f, 0.3f);
            await ToSignal(_tween, Tween.SignalName.Finished);
        }
    }
    
    public class SceneLoadedEvent : IGameEvent
    {
        public string ScenePath { get; set; }
    }
}
