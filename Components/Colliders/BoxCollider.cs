using Leo.ECS.Spacial;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Leo.ECS.Components.Colliders
{
    public class BoxCollider : Collider
    {
        #region Static Creation
        public static BoxCollider Create(float width, float height, IEnumerable<Layer> LayersToIgnore = null)
        {
            return Create(new Vector2(-width / 2f, -height / 2f),
                        new Vector2(width / 2f, height / 2f),
                        LayersToIgnore);
        }
        public static BoxCollider Create(Vector2 TopLeft, Vector2 BottomRight, IEnumerable<Layer> LayersToIgnore = null)
        {
            float halfWidth = (BottomRight.X - TopLeft.X) / 2f;
            float halfHeight = (BottomRight.Y - TopLeft.Y) / 2f;
            return new BoxCollider(
                    new Vector2[]
                    {
                        TopLeft,
                        new Vector2(TopLeft.X, BottomRight.Y),  // top left
                        BottomRight,
                        new Vector2(BottomRight.X, TopLeft.Y)   // bottom right
                    }, halfWidth, halfHeight,
                    LayersToIgnore
                );
        }
        #endregion

        float halfWidth, halfHeight;

        public BoxCollider(Vector2[] Vertices, float halfWidth, float halfHeight,
            IEnumerable<Layer> LayersToIgnore = null) : base(Vertices, LayersToIgnore)
        {
            this.halfWidth = halfWidth;
            this.halfHeight = halfHeight;
        }

        protected override Bounds CreateBounds()
        {
            return new Bounds(gameObject.transform.Position, halfWidth, halfHeight);
        }
    }
}
