
namespace framework.core.interfaces
{
    public interface IUpdateable
    {
        void Update(float deltaTime);
    }
    
    public interface IFixedUpdateable
    {
        void FixedUpdate(float fixedDeltaTime);
    }
}