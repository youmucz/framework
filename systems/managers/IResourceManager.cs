// Scripts/Managers/ResourceManager.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using framework.systems.core.services;

namespace framework.systems.managers
{
    public partial class IResourceManager : IService
    {
        private readonly Dictionary<string, Resource> _resourceCache = new();
        private readonly Dictionary<string, int> _referenceCount = new();
        private readonly HashSet<string> _loadingResources = new();
        
        public event Action<string, float> OnResourceLoadProgress;
        public event Action<string> OnResourceLoaded;
        public event Action<string> OnResourceLoadFailed;

        public ServiceLocator Locator { get; set; }

        public void Initialize()
        {
            
        }
        
        public async Task<T> LoadAsync<T>(string path) where T : Resource
        {
            // 检查缓存
            if (_resourceCache.TryGetValue(path, out var cached))
            {
                _referenceCount[path]++;
                return cached as T;
            }
            
            // 检查是否正在加载
            if (_loadingResources.Contains(path))
            {
                while (_loadingResources.Contains(path))
                {
                    await Task.Delay(10);
                }
                
                if (_resourceCache.TryGetValue(path, out cached))
                {
                    _referenceCount[path]++;
                    return cached as T;
                }
            }
            
            // 开始加载
            _loadingResources.Add(path);
            
            try
            {
                var resource = await LoadResourceAsync<T>(path);
                
                if (resource != null)
                {
                    _resourceCache[path] = resource;
                    _referenceCount[path] = 1;
                    OnResourceLoaded?.Invoke(path);
                    return resource;
                }
                else
                {
                    OnResourceLoadFailed?.Invoke(path);
                    return null;
                }
            }
            finally
            {
                _loadingResources.Remove(path);
            }
        }
        
        public T Load<T>(string path) where T : Resource
        {
            if (_resourceCache.TryGetValue(path, out var cached))
            {
                _referenceCount[path]++;
                return cached as T;
            }
            
            var resource = GD.Load<T>(path);
            if (resource != null)
            {
                _resourceCache[path] = resource;
                _referenceCount[path] = 1;
            }
            
            return resource;
        }
        
        private async Task<T> LoadResourceAsync<T>(string path) where T : Resource
        {
            return await Task.Run(() => GD.Load<T>(path));
        }
        
        public void Unload(string path)
        {
            if (_referenceCount.TryGetValue(path, out var count))
            {
                count--;
                if (count <= 0)
                {
                    _resourceCache.Remove(path);
                    _referenceCount.Remove(path);
                }
                else
                {
                    _referenceCount[path] = count;
                }
            }
        }
        
        public void UnloadAll()
        {
            _resourceCache.Clear();
            _referenceCount.Clear();
            _loadingResources.Clear();
        }
        
        public void PreloadResources(string[] paths)
        {
            foreach (var path in paths)
            {
                _ = LoadAsync<Resource>(path);
            }
        }
        
        public int GetCacheSize()
        {
            return _resourceCache.Count;
        }
        
        public void Shutdown()
        {
            UnloadAll();
        }
    }
}
