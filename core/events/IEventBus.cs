// Scripts/Core/Events/EventBus.cs
using System;
using System.Collections.Generic;
using framework.core.services;
using framework.debug;

namespace framework.core.events
{
    public class IEventBus : IService
    {
        private readonly Dictionary<Type, List<Delegate>> _eventHandlers = new();
        private readonly Queue<object> _eventQueue = new();
        private bool _isProcessing;

        public ServiceLocator Locator { get; set; }

        public void Initialize()
        {
            DebugLog.InfoLog("EventBus initialized");
        }
        
        public void Subscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var eventType = typeof(T);
            
            if (!_eventHandlers.TryGetValue(eventType, out var value))
            {
                value = new List<Delegate>();
                _eventHandlers[eventType] = value;
            }

            value.Add(handler);
        }
        
        public void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var eventType = typeof(T);
            
            if (_eventHandlers.TryGetValue(eventType, out var handlers))
            {
                handlers.Remove(handler);
            }
        }
        
        public void Publish<T>(T gameEvent) where T : IGameEvent
        {
            if (_isProcessing)
            {
                _eventQueue.Enqueue(gameEvent);
                return;
            }
            
            ProcessEvent(gameEvent);
            ProcessQueuedEvents();
        }
        
        private void ProcessEvent<T>(T gameEvent) where T : IGameEvent
        {
            var eventType = gameEvent.GetType();
            
            if (_eventHandlers.TryGetValue(eventType, out var handlers))
            {
                _isProcessing = true;
                
                foreach (var handler in handlers.ToArray())
                {
                    try
                    {
                        (handler as Action<T>)?.Invoke(gameEvent);
                    }
                    catch (Exception e)
                    {
                        DebugLog.ErrorLog($"Error handling event {eventType.Name}: {e.Message}");
                    }
                }
                
                _isProcessing = false;
            }
        }
        
        private void ProcessQueuedEvents()
        {
            while (_eventQueue.Count > 0)
            {
                var queuedEvent = _eventQueue.Dequeue();
                var processMethod = GetType().GetMethod(nameof(ProcessEvent), 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var genericMethod = processMethod.MakeGenericMethod(queuedEvent.GetType());
                genericMethod.Invoke(this, new[] { queuedEvent });
            }
        }
        
        public void Shutdown()
        {
            _eventHandlers.Clear();
            _eventQueue.Clear();
        }
    }
    
    public interface IGameEvent { }
}
