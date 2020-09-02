using Leo.ECS.Components.Colliders;
using Leo.ECS.Interfaces;
using Leo.ECS.Spacial;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Leo.ECS
{
    public enum Layer
    {
        Default
    }
    public class Scene : IUpdatable, IRenderable, IDestroyable
    {
        #region Variables

        /// <summary>
        /// The Name of this Scene
        /// </summary>
        public string Name;

        /// <summary>
        /// An instantiated scene is always marked as active
        /// </summary>
        public bool IsActive => true;

        /// <summary>
        /// The Camera for this scene
        /// </summary>
        public Camera Camera;

        /// <summary>
        /// Half of this scene's dimensions.  Used for creating QuadTrees.
        /// </summary>
        public Vector2 HalfWorldBounds;

        /// <summary>
        /// objects this scene is managing
        /// </summary>
        protected HashSet<GameObject> objects;

        /// <summary>
        /// objects this scene will add to the set of managed objects at the end of this frame
        /// </summary>
        protected Queue<GameObject> toAdd;

        /// <summary>
        /// objects this scene will remove from the managed set at the end of this frame
        /// </summary>
        protected Queue<GameObject> toRemove;

        /// <summary>
        /// A dictionary of quadtrees (one per layer)
        /// </summary>
        protected Dictionary<Layer, QuadTree<GameObject>> Trees;

        /// <summary>
        /// A lookup dictionary for all colliders associated with a layer
        /// </summary>
        protected Dictionary<Layer, List<Collider>> ColliderLookup;

        /// <summary>
        /// A list of all registered colliders in this scene
        /// </summary>
        protected HashSet<Collider> ColliderRegistry;

        /// <summary>
        /// All layers that exist in this scene
        /// </summary>
        protected readonly Array Layers = Enum.GetValues(typeof(Layer));
        #endregion

        #region Constructors
        public Scene(string Name, Vector2 HalfWorldBounds, Camera Camera)
        {
            this.Name = Name;
            this.HalfWorldBounds = HalfWorldBounds;

            objects = new HashSet<GameObject>();
            toAdd = new Queue<GameObject>();
            toRemove = new Queue<GameObject>();
            Trees = new Dictionary<Layer, QuadTree<GameObject>>();

            foreach (Layer l in Layers)
                Trees.Add(l, new QuadTree<GameObject>(HalfWorldBounds.X, HalfWorldBounds.Y));

            ColliderLookup = new Dictionary<Layer, List<Collider>>();
            ColliderRegistry = new HashSet<Collider>();

            foreach (Layer l in Enum.GetValues(typeof(Layer)))
                ColliderLookup[l] = new List<Collider>();

            GameObject CameraHolder = new GameObject("Main Camera");
            CameraHolder.AddComponent(Camera);
            this.Camera = Camera;
            Add(CameraHolder);
        }
        #endregion

        #region Object Management
        /// <summary>
        /// Register a collider for a specified layer
        /// </summary>
        /// <param name="l">the layer of the collider</param>
        /// <param name="c">the collider to register</param>
        public void RegisterCollider(Layer l, Collider c)
        {
            ColliderLookup[l].Add(c);
            ColliderRegistry.Add(c);
        }
        /// <summary>
        /// Deregister a collider for a specified layer
        /// </summary>
        /// <param name="l">the layer of the collider</param>
        /// <param name="c">the collider to deregister</param>
        public void DeregisterCollider(Layer l, Collider c)
        {
            ColliderLookup[l].Remove(c);
            ColliderRegistry.Remove(c);
        }
        /// <summary>
        /// Add a GameObject to this Scene
        /// </summary>
        /// <param name="obj">the object to add</param>
        public void Add(GameObject obj) => toAdd.Enqueue(obj);
        /// <summary>
        /// Remove a GameObject to this Scene
        /// </summary>
        /// <param name="obj">the object to remove</param>
        public void Remove(GameObject obj) => toRemove.Enqueue(obj);

        /// <summary>
        /// handle adding an object to the scene
        /// </summary>
        /// <param name="obj">the object being added</param>
        private void HandleAdd(GameObject obj)
        {
            objects.Add(obj);
            obj.AddToScene(this);
        }

        /// <summary>
        /// handle the removal of an object from the scene
        /// </summary>
        /// <param name="obj">the object being removed</param>
        private void HandleRemove(GameObject obj)
        {
            objects.Remove(obj);
            obj.RemoveFromScene(this);
            obj.Destroy();
        }
        #endregion

        #region Updates and Renders
        public virtual void Update()
        {
            // update objects
            foreach (GameObject obj in objects)
                obj.Update();

            // check for collisions
            // start by creating collision registry
            HashSet<int> CollisionRegistry = new HashSet<int>();
            // Initialize the quadtrees for each layer and build the trees
            foreach (Layer layer in Layers)
            {
                Trees[layer].Reset();
                foreach(Collider c in ColliderLookup[layer])
                    Trees[layer].Insert(c.gameObject, c.Bounds);
            }
            foreach (Collider c in ColliderRegistry)
            {
                GameObject source = c.gameObject;
                foreach (Layer layer in Layers)
                {
                    if (c.IgnoresLayer(layer)) continue;
                    List<GameObject> Collisions = Trees[layer].QueryBounds(c.Bounds, true);
                    foreach (GameObject other in Collisions)
                    {
                        if (source == other) continue;
                        int CollisionHash = (source.GetHashCode() > other.GetHashCode()) ? 
                            (source.GetHashCode() * 397) ^ other.GetHashCode() :
                            (other.GetHashCode() * 397) ^ source.GetHashCode();
                        if (CollisionRegistry.Contains(CollisionHash)) continue;

                        // TODO SAT collision instead of this AABB shit

                        CollisionRegistry.Add(CollisionHash);
                        source.OnCollision(CollisionHash, other);
                        other.OnCollision(CollisionHash, source);
                    }
                }
            }
            
            while (toAdd.Count > 0)
                HandleAdd(toAdd.Dequeue());
            while (toRemove.Count > 0)
                HandleRemove(toRemove.Dequeue());
        }
        /// <summary>
        /// Late Update this scene and it's objects
        /// </summary>
        public virtual void LateUpdate()
        {
            foreach (GameObject obj in objects)
                obj.LateUpdate();
        }
        /// <summary>
        /// render this scene to the given spritebatch
        /// </summary>
        /// <param name="batch">the batch to render to</param>
        public virtual void Render(SpriteBatch batch)
        {
            foreach (GameObject obj in objects)
                obj.Render(batch);
        }
        /// <summary>
        /// Draw gizmos for this scene
        /// </summary>
        /// <param name="batch">the batch for gizmos to draw to</param>
        public virtual void OnDrawGizmos(SpriteBatch batch)
        {
            Gizmos.SetSpriteBatch(batch);
            foreach (GameObject obj in objects)
                obj.OnDrawGizmos();
            Gizmos.SetSpriteBatch(null);
        }
        #endregion

        #region Destroy
        /// <summary>
        /// destroy all of our game objects and anywhere we may contain a reference to a game object
        /// </summary>
        public virtual void Destroy()
        {
            foreach (GameObject obj in objects)
            {   // force remove objects from this scene and destroy them
                HandleRemove(obj);
                obj.Destroy();
            }
            foreach (Layer l in Layers)
                ColliderLookup[l].Clear();
            ColliderLookup.Clear();
        }
        #endregion
    }
}
