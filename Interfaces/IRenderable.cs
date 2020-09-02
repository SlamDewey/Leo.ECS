using Microsoft.Xna.Framework.Graphics;

namespace Leo.ECS.Interfaces
{
    /// <summary>
    /// An interface used to define if an object is renderable or not.
    /// </summary>
    public interface IRenderable : IActivatable
    {
        /// <summary>
        /// The definition of how to render this object
        /// </summary>
        /// <param name="batch">the batch to render the object onto</param>
        void Render(SpriteBatch batch);
    }
}
