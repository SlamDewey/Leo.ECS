using Leo.ECS.Interfaces;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Leo.ECS
{
    /// <summary>
    /// Represents a position and rotation for a gameobject, as well as hierarchies
    /// </summary>
    public class Transform : IDestroyable
    {
        #region Variables
        /// <summary>
        /// The Parent of this transform
        /// </summary>
        public Transform Parent = null;

        /// <summary>
        /// The rotation of this transform
        /// </summary>
        public float _rotation = 0f;

        private HashSet<Transform> children = null;
        private Vector2 _position = Vector2.Zero;
        // event callbacks
        public CompositeAction OnPositionChange;
        public CompositeAction OnRotationChange;
        #endregion

        #region Properties
        public Vector2 Position
        {
            set 
            {
                Vector2 offset = value - _position;
                _position = value;
                OnPositionChange?.Invoke();
                foreach (Transform t in children)
                    t.Translate(offset);
            }
            get => _position;
        }
        public float Rotation
        {
            set
            {
                _rotation = value;
                OnRotationChange?.Invoke();
            }
            get => _rotation;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new transform with an empty set of children
        /// </summary>
        public Transform()
        {
            children = new HashSet<Transform>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Append a child to this transform
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(Transform child) => children.Add(child);
        /// <summary>
        /// Remove a child transform from this transform's chilren list
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(Transform child) => children.Remove(child);

        /// <summary>
        /// translate the position by a given offset
        /// </summary>
        /// <param name="offset"></param>
        public void Translate(Vector2 offset)
        {
            Position += offset;
        }
        /// <summary>
        /// translate the position by a given offset
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Translate(float x, float y) => Translate(new Vector2(x, y));
        #endregion

        #region Destroy
        /// <summary>
        /// Remove a reference to this in the parent and child transforms.
        /// </summary>
        public void Destroy()
        {
            if (Parent != null)
                Parent.RemoveChild(this);
            foreach (Transform child in children)
                child.Parent = null;
        }
        #endregion
    }
}
