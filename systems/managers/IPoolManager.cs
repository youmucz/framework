// Scripts/Managers/PoolManager.cs
using System.Collections.Generic;
using Godot;
using framework.systems.core.interfaces;
using framework.systems.core.services;

namespace framework.systems.managers
{
    public partial class IPoolManager : Node, IService
    {
        private readonly Dictionary<string, ObjectPool> _pools = new();
        private Node _poolContainer;

        public ServiceLocator Locator { get; set; }

        public void Initialize()
        {
            // 创建对象池容器
            _poolContainer = new Node { Name = "ObjectPools" };
            CallDeferred(Node.MethodName.AddChild, _poolContainer);
        }
        
        public void CreatePool<T>(string poolName, PackedScene prefab, int initialSize = 10) where T : Node
        {
            if (_pools.ContainsKey(poolName))
            {
                GD.PrintErr($"Pool '{poolName}' already exists!");
                return;
            }
            
            var pool = new ObjectPool<T>(prefab, initialSize, _poolContainer);
            _pools[poolName] = pool;
        }
        
        public T Get<T>(string poolName) where T : Node
        {
            if (_pools.TryGetValue(poolName, out var pool))
            {
                if (pool is ObjectPool<T> typedPool)
                {
                    return typedPool.Get();
                }
            }
            
            GD.PrintErr($"Pool '{poolName}' not found!");
            return null;
        }
        
        public void Return<T>(string poolName, T obj) where T : Node
        {
            if (_pools.TryGetValue(poolName, out var pool))
            {
                if (pool is ObjectPool<T> typedPool)
                {
                    typedPool.Return(obj);
                }
            }
        }
        
        public void ClearPool(string poolName)
        {
            if (_pools.TryGetValue(poolName, out var pool))
            {
                pool.Clear();
                _pools.Remove(poolName);
            }
        }
        
        public void ClearAllPools()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Clear();
            }
            _pools.Clear();
        }
        
        public void Shutdown()
        {
            ClearAllPools();
            _poolContainer?.QueueFree();
        }
        
        private abstract class ObjectPool
        {
            public abstract void Clear();
        }
        
        private class ObjectPool<T> : ObjectPool where T : Node
        {
            private readonly Queue<T> _available = new();
            private readonly HashSet<T> _inUse = new();
            private readonly PackedScene _prefab;
            private readonly Node _container;
            
            public ObjectPool(PackedScene prefab, int initialSize, Node container)
            {
                _prefab = prefab;
                _container = container;
                
                // 预创建对象
                for (int i = 0; i < initialSize; i++)
                {
                    CreateNewObject();
                }
            }
            
            public T Get()
            {
                T obj;
                
                if (_available.Count > 0)
                {
                    obj = _available.Dequeue();
                }
                else
                {
                    obj = CreateNewObject();
                }
                
                if (obj != null)
                {
                    _inUse.Add(obj);
                    ActivateObject(obj);
                }
                
                return obj;
            }
            
            public void Return(T obj)
            {
                if (obj == null || !_inUse.Contains(obj))
                    return;
                
                _inUse.Remove(obj);
                DeactivateObject(obj);
                _available.Enqueue(obj);
            }
            
            private T CreateNewObject()
            {
                var instance = _prefab.Instantiate<T>();
                if (instance != null)
                {
                    _container.AddChild(instance);
                    DeactivateObject(instance);
                    _available.Enqueue(instance);
                }
                return instance;
            }
            
            private void ActivateObject(T obj)
            {
                obj.SetProcess(true);
                obj.SetPhysicsProcess(true);
                // obj.Visible = true;
                
                // 如果对象实现了 IPoolable 接口，调用 OnSpawn
                if (obj is IPoolable poolable)
                {
                    poolable.OnSpawn();
                }
            }
            
            private void DeactivateObject(T obj)
            {
                obj.SetProcess(false);
                obj.SetPhysicsProcess(false);
                // obj.Visible = false;
                
                // 如果对象实现了 IPoolable 接口，调用 OnDespawn
                if (obj is IPoolable poolable)
                {
                    poolable.OnDespawn();
                }
            }
            
            public override void Clear()
            {
                foreach (var obj in _available)
                {
                    obj?.QueueFree();
                }
                
                foreach (var obj in _inUse)
                {
                    obj?.QueueFree();
                }
                
                _available.Clear();
                _inUse.Clear();
            }
        }
    }
}
