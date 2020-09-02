using Leo.ECS.Components;
using Leo.ECS.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Leo.ECS
{
    /// <summary>
    /// A GameObject to attach components to, and add to a scene
    /// </summary>
    public sealed class GameObject : IUpdatable, IRenderable, IDestroyable
    {
        #region Variables
        /// <summary>
        /// The name of this GameObject
        /// </summary>
        public string Name;

        /// <summary>
        /// The layer this GameObject lives on in a scene
        /// </summary>
        public Layer Layer { get; private set; }

        /// <summary>
        /// The Transform of this gameobject
        /// </summary>
        public Transform transform { get; private set; }

        /// <summary>
        /// Whether or not this GameObject is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The scene this gameobject belongs to
        /// </summary>
        private Scene scene;

        /// <summary>
        /// All Updatable Components
        /// </summary>
        private HashSet<StaticComponent> components;
        /// <summary>
        /// All Updatable Components
        /// </summary>
        private HashSet<IUpdatable> updatables;
        /// <summary>
        /// All Renderable Components
        /// </summary>
        private HashSet<IRenderable> renderables;

        // manages collisions from last frame
        private HashSet<int> lastCollisionSet;

        // manages collisions that are ongoing
        private Dictionary<int, GameObject> collisionLookup;

        // manages collisions from this frame
        private HashSet<int> collisionSet;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new GameObject
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Layer"></param>
        public GameObject(string Name, Layer Layer = Layer.Default)
        {
            this.Name = Name;
            this.Layer = Layer;
            transform = new Transform();
            components = new HashSet<StaticComponent>();
            updatables = new HashSet<IUpdatable>();
            renderables = new HashSet<IRenderable>();
        }
        #endregion

        #region Getters
        /// <summary>
        /// Get the first instance of a specified component that is attached to this object
        /// </summary>
        /// <typeparam name="T">the type of component to get</typeparam>
        /// <returns>the first instance of the specified component or null</returns>
        public T GetComponent<T>() where T : StaticComponent
        {
            foreach (StaticComponent c in components)
                if (c is T) return c as T;
            return null;
        }

        /// <summary>
        /// Get all instances of a specified component type that are attached to this object
        /// </summary>
        /// <typeparam name="T">the type of component to get</typeparam>
        /// <returns>a set of all unique components found</returns>
        public IEnumerable<T> GetComponents<T>() where T : StaticComponent
        {
            HashSet<T> res = new HashSet<T>();
            foreach (StaticComponent c in components)
                if (c is T) res.Add(c as T);
            return res;
        }

        /// <summary>
        /// Get the first instance of a specified component that is attached to this object
        /// and fits the given conditional
        /// </summary>
        /// <typeparam name="T">the type of component to check</typeparam>
        /// <param name="Conditional">A user defined conditional function</param>
        /// <returns>the first component of the specified type that fits the conditional, or null</returns>
        public T GetComponentWhere<T>(Func<StaticComponent, bool> Conditional) where T : StaticComponent
        {
            foreach (StaticComponent c in components)
                if (c is T && Conditional(c)) return c as T;
            return null;
        }

        /// <summary>
        /// Get all instances of a specified component type that are attached to this object
        /// </summary>
        /// <typeparam name="T">the type of component to get</typeparam>
        /// <returns>a set of all unique components found that match the conditional</returns>
        public IEnumerable<T> GetComponentsWhere<T>(Func<StaticComponent, bool> Conditional) where T : StaticComponent
        {
            HashSet<T> res = new HashSet<T>();
            foreach (StaticComponent c in components)
                if (c is T && Conditional(c)) res.Add(c as T);
            return res;
        }

        /// <summary>
        /// Get the unique HashCode for this GameObject
        /// </summary>
        /// <returns>a unique hash code based on this objects Name and Transform</returns>
        public override int GetHashCode()
        {
            int result = Name == null ? 0 : Name.GetHashCode();
            result = (result * 397) ^ transform.GetHashCode();
            return result;
        }
        #endregion

        #region Event Management
        /// <summary>
        /// Register a collision event on this game object
        /// </summary>
        /// <param name="Hash">the hash of the collision event</param>
        /// <param name="other">the object we collided  with</param>
        public void OnCollision(int Hash, GameObject other)
        {
            collisionSet.Add(Hash);
            if (!collisionLookup.ContainsKey(Hash))
                collisionLookup.Add(Hash, other);
            foreach (StaticComponent c in components)
            {
                if (lastCollisionSet.Contains(Hash))
                    c.OnCollisionStay(other);
                else c.OnCollisionEnter(other);
            }
        }

        /// <summary>
        /// Called every time this GameObject is attached to a scene
        /// </summary>
        /// <param name="scene">the scene in question</param>
        public void AddToScene(Scene scene)
        {
            if (this.scene != null)
                RemoveFromScene(this.scene);
            this.scene = scene;
            foreach (StaticComponent c in components)
                c.OnAddedToScene(scene);
        }
        /// <summary>
        /// Called every time this GameObject is removed from a scene
        /// </summary>
        /// <param name="scene">the scene in question</param>
        public void RemoveFromScene(Scene scene) 
        {
            if (this.scene != scene) return;
            this.scene = null;
            foreach (StaticComponent c in components)
                c.OnRemovedFromScene(scene);
        }
        /// <summary>
        /// Add a component to this GameObject
        /// </summary>
        /// <param name="c">the component to add</param>
        public void AddComponent(StaticComponent c)
        {
            components.Add(c);
            if (c is IUpdatable)
                updatables.Add(c as IUpdatable);
            if (c is IRenderable)
                renderables.Add(c as IRenderable);
            c.gameObject = this;

            if (scene != null)
                c.OnAddedToScene(scene);
        }
        /// <summary>
        /// Remove a component to this GameObject
        /// </summary>
        /// <param name="c">the component to remove</param>
        public void RemoveComponent(StaticComponent c)
        {
            if (scene != null)
                c.OnRemovedFromScene(scene);

            components.Remove(c);
            if (c is IUpdatable)
                updatables.Remove(c as IUpdatable);
            if (c is IRenderable)
                renderables.Remove(c as IRenderable);
            c.gameObject = null;
        }
        #endregion

        #region Updates and Render
        /// <summary>
        /// Update this GameObject, and each of its updatable components
        /// </summary>
        public void Update() 
        {
            foreach (IUpdatable u in updatables)
                if (u.IsActive)
                    u.Update();
        }
        /// <summary>
        /// LateUpdate this GameObject, and each of its updatable components, and
        /// handle OnExit collision events
        /// </summary>
        public void LateUpdate()
        {
            // get only collisions that are no longer ongoing
            lastCollisionSet.ExceptWith(collisionSet);
            foreach (int Hash in lastCollisionSet)
            {
                GameObject other = collisionLookup[Hash];
                // remove the reference to that object
                collisionLookup.Remove(Hash);
                foreach (StaticComponent c in components)
                    c.OnCollisionExit(other);
            }

            foreach (IUpdatable u in updatables)
                if (u.IsActive)
                    u.LateUpdate();

            lastCollisionSet.Clear();
            lastCollisionSet.UnionWith(collisionSet);
            collisionSet.Clear();
        }
        /// <summary>
        /// Render this component
        /// </summary>
        /// <param name="batch">the batch to render with</param>
        public void Render(SpriteBatch batch)
        {
            foreach (IRenderable r in renderables)
                if (r.IsActive)
                    r.Render(batch);
        }
        /// <summary>
        /// Ask each component to draw any gizmos
        /// </summary>
        public void OnDrawGizmos()
        {
            foreach (StaticComponent s in components)
                if (s.IsActive)
                    s.OnDrawGizmos();
        }
        #endregion

        #region Destroy
        /// <summary>
        /// Removes and Destroys all Components, and removes references to other objects
        /// </summary>
        public void Destroy()
        {
            transform.Destroy();
            foreach (StaticComponent c in components)
            {
                RemoveComponent(c);
                c.Destroy();
            }
            lastCollisionSet.Clear();
            collisionLookup.Clear();
            collisionSet.Clear();
        }
        #endregion
    }
}
