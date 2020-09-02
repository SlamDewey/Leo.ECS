using Leo.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Leo.ECS
{
    /// <summary>
    /// This is a basic Camera.  It does not receive update events.
    /// When you need to control the camera position or rotation with code, 
    /// create a component to manage the transform of the Camera's GameObject.
    /// </summary>
    public sealed class Camera : StaticComponent
    {
        #region Variables and Constants
        /// <summary>
        /// Zoom Constants
        /// </summary>
        protected const float MaxZoom = 10f, MinZoom = 0.05f;

        private float zoom = 1f;
        /// <summary>
        /// The Zoom of this Camera
        /// </summary>
        public float Zoom
        {
            set => zoom = Mathf.Clamp(value, MinZoom, MaxZoom);
            get => zoom;
        }
        /// <summary>
        /// Center of the screen
        /// </summary>
        private Vector2 Origin;

        private GraphicsDevice graphicsDevice;
        private Viewport viewport => graphicsDevice.Viewport;

        /// <summary>
        /// Virtual Width (viewport Width)
        /// </summary>
        public int VirtualWidth => graphicsDevice.Viewport.Width;

        /// <summary>
        /// Virtual Height (viewport Height)
        /// </summary>
        public int VirtualHeight => graphicsDevice.Viewport.Height;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new Camera
        /// </summary>
        /// <param name="graphicsDevice">the Graphics Device for this game</param>
        public Camera(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            Origin = new Vector2(VirtualWidth / 2f, VirtualHeight / 2f);
        }

        /// <summary>
        /// Create a new Camera
        /// </summary>
        /// <param name="graphicsDevice">the Graphics Device for this game</param>
        /// <param name="InToOutRatio">a ratio of the Internal Render Size to Output Render Size</param>
        public Camera(GraphicsDevice graphicsDevice, Vector2 InToOutRatio)
        {
            this.graphicsDevice = graphicsDevice;
            Origin = new Vector2(VirtualWidth / 2f, VirtualHeight / 2f) * InToOutRatio;
        }
        #endregion

        protected override void OnAddedToGameObject()
        {
            base.OnAddedToGameObject();
            transform.Position -= Origin;
        }

        #region Transformations

        /// <summary>
        /// Transform a point in world space into a vector represented as a draw location in screen space.
        /// </summary>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// <returns>The Vector2 location of the point in screenspace</returns>
        public Vector2 WorldToScreenPoint(float x, float y) => WorldToScreenPoint(new Vector2(x, y));

        /// <summary>
        /// Transform a point in world space into a vector represented as a draw location in screen space.
        /// </summary>
        /// <param name="worldPosition">The point to be transformed</param>
        /// <returns>The Vector2 location of the point in screenspace</returns>
        public Vector2 WorldToScreenPoint(Vector2 worldPosition)
        {
            return Vector2.Transform(
                worldPosition + new Vector2(viewport.X, viewport.Y),
                GetViewMatrix()
            );
        }

        /// <summary>
        /// Transforms a position in screenspace into worldspace
        /// </summary>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// <returns>the point as a worldspace coordinate</returns>
        /// 
        public Vector2 ScreenToWorld(float x, float y) => ScreenToWorld(new Vector2(x, y));

        /// <summary>
        /// Transforms a position in screenspace into worldspace
        /// </summary>
        /// <param name="screenPosition">the position to tranform</param>
        /// <returns>the transformed position</returns>
        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(
                screenPosition - new Vector2(viewport.X, viewport.Y),
                GetInverseViewMatrix()
            );
        }
        #endregion

        #region GetViewMatrix
        public Matrix GetViewMatrix()
        {
            return
                Matrix.CreateTranslation(new Vector3(-transform.Position, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                Matrix.CreateRotationZ(transform.Rotation) *
                Matrix.CreateScale(Zoom, Zoom, 1) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }
        public Matrix GetInverseViewMatrix() => Matrix.Invert(GetViewMatrix());
        #endregion
    }
}
