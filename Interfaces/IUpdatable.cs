namespace Leo.ECS.Interfaces
{
    public interface IUpdatable : IActivatable
    {
        void Update();
        void LateUpdate();
    }
}
