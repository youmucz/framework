// Scripts/Core/Interfaces/IPoolable.cs
namespace framework.systems.core.interfaces
{
    public interface IPoolable
    {
        void OnSpawn();
        void OnDespawn();
        void ResetState();
    }
}