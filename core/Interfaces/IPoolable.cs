// Scripts/Core/Interfaces/IPoolable.cs
namespace framework.core.interfaces
{
    public interface IPoolable
    {
        void OnSpawn();
        void OnDespawn();
        void ResetState();
    }
}