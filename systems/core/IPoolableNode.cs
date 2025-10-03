// Scripts/Core/PoolableNode.cs

using framework.systems.core.interfaces;
using Godot;

namespace framework.systems.core
{
    public abstract partial class PoolableNode : Node, IPoolable
    {
        public virtual void OnSpawn()
        {
            // 子类可以重写此方法来初始化对象
        }
        
        public virtual void OnDespawn()
        {
            // 子类可以重写此方法来清理对象
        }
        
        public virtual void ResetState()
        {
            // 子类可以重写此方法来重置对象状态
        }
    }
    
    public abstract partial class PoolableNode2D : Node2D, IPoolable
    {
        public virtual void OnSpawn()
        {
            // 子类可以重写此方法来初始化对象
        }
        
        public virtual void OnDespawn()
        {
            // 子类可以重写此方法来清理对象
        }
        
        public virtual void ResetState()
        {
            // 子类可以重写此方法来重置对象状态
        }
    }
    
    public abstract partial class PoolableNode3D : Node3D, IPoolable
    {
        public virtual void OnSpawn()
        {
            // 子类可以重写此方法来初始化对象
        }
        
        public virtual void OnDespawn()
        {
            // 子类可以重写此方法来清理对象
        }
        
        public virtual void ResetState()
        {
            // 子类可以重写此方法来重置对象状态
        }
    }
}