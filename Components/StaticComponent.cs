using Leo.ECS.Interfaces;

namespace Leo.ECS.Components
{
    /// <summary>
    /// Represents a component that cannot be updated or rendered, but still responds to basic events
    /// </summary>
    public class StaticComponent : IDestroyable
    {
        #region Variables
        /// <summary>
        /// a private reference to the gameobject we are attached to
        /// </summary>
        private GameObject _gameObject;

        /// <summary>
        /// Whether or not this Component is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The transform of the GameObject this component is attached to
        /// </summary>
        public Transform transform => _gameObject?.transform;
        #endregion

        #region Properties
        /// <summary>
        /// The gameobject attached to this component
        /// </summary>
        public GameObject gameObject
        {
            set
            {
                if (_gameObject != null)
                    OnRemovedFromGameObject();
                _gameObject = value;
                if (_gameObject != null)
                    OnAddedToGameObject();
            }
            get => _gameObject;
        }
        #endregion

        public StaticComponent() { }

        #region Virtual Functions
        /// <summary>
        /// Called anytime this component is attached to a GameObject
        /// </summary>
        protected virtual void OnAddedToGameObject() { }
        /// <summary>
        /// Called anytime this component is removed from a GameObject
        /// </summary>
        protected virtual void OnRemovedFromGameObject() { }

        /// <summary>
        /// Called anytime this component is added to a new scene
        /// </summary>
        /// <param name="scene">the scene in question</param>
        public virtual void OnAddedToScene(Scene scene) { }

        /// <summary>
        /// Called anytime this component is removed from a new scene
        /// </summary>
        /// <param name="scene">the scene in question</param>
        public virtual void OnRemovedFromScene(Scene scene) { }

        /// <summary>
        /// Called when another object collided with us this frame
        /// </summary>
        /// <param name="other">the object we collided with</param>
        public void OnCollisionEnter(GameObject other) { }

        /// <summary>
        /// Called when an object we collided with some time ago, is still colliding with us
        /// </summary>
        /// <param name="other">the object we are colliding with</param>
        public void OnCollisionStay(GameObject other) { }
        
        /// <summary>
        /// Called when an object was colliding with us last frame, and no longer is
        /// </summary>
        /// <param name="other">the object we had collided with</param>
        public void OnCollisionExit(GameObject other) { }

        /// <summary>
        /// Called when the scene is rendering Gizmos
        /// </summary>
        public virtual void OnDrawGizmos() { }

        /// <summary>
        /// Called when this component is about to be destroyed
        /// </summary>
        protected virtual void OnDestroy() { }
        #endregion

        public void Destroy()
        {
            OnDestroy();
            gameObject.RemoveComponent(this);
        }
    }
}
