# Leo.ECS
Leo.ECS is a rather well documented, lightweight, 2D Focused Entity-Component-System designed for use specifically with
Microsoft's XNA / MonoGame Development.  Although the system is rather well documented, the system is entirely still very
complex, and somebody with little experience working with ECS should likely not use this system.  Lightweight means many
features are not implemented, because the system is meant to be barebones.

With that said, the Leo.ECS system is capable of having multiple scenes, each scene managing multiple gameobjects, and each
gameobject managing multiple components.  The basic functionality is all there, ready to be expanded upon.

If you are experienced in using a few ECS and are ready to try this one, I will include the basic structure/functionality
of some of the major classes, so that you may see the features that come built in.

However since it is unlikely just anybody will be using this system, if you have questions or trouble implementing this,
please contact me directly at [mr.jaredmassa@gmail.com](mailto:mr.jaredmassa@gmail.com)
___
## The Scene Class
```c#
public class Scene : IUpdatable, IRenderable, IDestroyable
{
    public Scene(string Name, Vector2 HalfWorldBounds, Camera Camera);

    public void RegisterCollider(Layer l, Collider c);
    public void DeregisterCollider(Layer l, Collider c);

    public void Add(GameObject obj);
    public void Remove(GameObject obj);
}
```
___
## The GameObject Class
```c#
public sealed class GameObject : IUpdatable, IRenderable, IDestroyable
{
    public GameObject(string Name, Layer Layer = Layer.Default);

    public T GetComponent<T>();
    public IEnumerable<T> GetComponents<T>();
    public T GetComponentWhere<T>(Func<StaticComponent, bool> Conditional);
    public IEnumerable<T> GetComponentsWhere<T>(Func<StaticComponent, bool> Conditional);

    public void OnCollision(int Hash, GameObject other);

    public void AddToScene(Scene scene);
    public void RemoveFromScene(Scene scene);

    public void AddComponent(StaticComponent c);
    public void RemoveComponent(StaticComponent c);
}
```
___
## The Scene Class
```c#
public class StaticComponent : IDestroyable
{
        public StaticComponent() { }

        protected virtual void OnAddedToGameObject();
        protected virtual void OnRemovedFromGameObject();

        public virtual void OnAddedToScene(Scene scene);
        public virtual void OnRemovedFromScene(Scene scene);

        public void OnCollisionEnter(GameObject other);
        public void OnCollisionStay(GameObject other);
        public void OnCollisionExit(GameObject other);
}
```
Static Component also has a child `Component.cs` which implements `IUpdatable`, but `GameObjects` are structured to manage components
that also implement `IRenderable`. (Components can render themselves).