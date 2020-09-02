using Leo.ECS.Spacial;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Leo.ECS.Components.Colliders
{
    /// <summary>
    /// Represents a set of vertices that define an object's collision area
    /// </summary>
    public abstract class Collider : Component
    {
        #region Variables
        private Vector2[] verts;
        private Bounds lastAABB;    // cached bounding box
        private bool VertsChangedSinceLastCalculation = false;  // cache is valid ?
        private HashSet<Layer> LayersToIgnore;
        #endregion

        #region Properties
        /// <summary>
        /// A list of vertices with their positions relative to the objects center point
        /// </summary>
        protected Vector2[] Vertices
        {
            set
            {
                VertsChangedSinceLastCalculation = true;
                verts = value;
            }
            get => verts;
        }
        /// <summary>
        /// The Bounds of this object
        /// </summary>
        public Bounds Bounds
        {
            get
            {   // check if we have a valid Bounds that is cached
                lastAABB = (lastAABB == null || VertsChangedSinceLastCalculation) ? CreateBounds() : lastAABB;
                return lastAABB;
            }
        }
        #endregion

        #region Constructor
        public Collider(Vector2[] Vertices, IEnumerable<Layer> LayersToIgnore = null)
        {
            this.Vertices = Vertices;
            this.LayersToIgnore = new HashSet<Layer>();
            if (LayersToIgnore != null)
                this.LayersToIgnore.UnionWith(LayersToIgnore);
        }
        #endregion

        #region Functionality
        /// <summary>
        /// Does this specific collider ignore collisions with a given layer
        /// </summary>
        /// <param name="l">the layer to check</param>
        /// <returns>true if this collider ignores collisions with the given layer</returns>
        public bool IgnoresLayer(Layer l) => LayersToIgnore.Contains(l);

        /// <summary>
        /// easier to manage delegate for the CompositeAction
        /// </summary>
        private void OnTransformChange() => VertsChangedSinceLastCalculation = true;

        /// <summary>
        /// When added to gameobject, we will add callbacks to that object's transform
        /// </summary>
        protected override void OnAddedToGameObject()
        {
            transform.OnRotationChange += OnTransformChange;
            transform.OnPositionChange += OnTransformChange;
        }
        /// <summary>
        /// we must remove our callbacks when we are ripped from the object
        /// </summary>
        protected override void OnRemovedFromGameObject()
        {
            transform.OnRotationChange += OnTransformChange;
            transform.OnPositionChange += OnTransformChange;
        }
        /// <summary>
        /// register this collider
        /// </summary>
        public override void OnAddedToScene(Scene scene)
        {
            scene.RegisterCollider(gameObject.Layer, this);
        }

        /// <summary>
        /// deregister this collider
        /// </summary>
        public override void OnRemovedFromScene(Scene scene)
        {
            scene.RegisterCollider(gameObject.Layer, this);
        }
        #endregion

        protected abstract Bounds CreateBounds();
    }
}
