using Leo.ECS.Interfaces;

namespace Leo.ECS.Components
{
    /// <summary>
    /// Represents an updatable component
    /// </summary>
    public abstract class Component : StaticComponent, IUpdatable
    {
        public virtual void LateUpdate() { }
        public virtual void Update() { }
    }
}
