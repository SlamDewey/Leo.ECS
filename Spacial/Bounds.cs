using Microsoft.Xna.Framework;

namespace Leo.ECS.Spacial
{
    /// <summary>
    /// Represents an Axis Aligned Bounding Box
    /// </summary>
    public class Bounds
    {
        #region Variables
        /// <summary>
        /// The TopLeft Corner of this Bounds
        /// </summary>
        public Vector2 TopLeft;
        /// <summary>
        /// The BottomRightCorner of this Bounds
        /// </summary>
        public Vector2 BottomRight;
        /// <summary>
        /// The centerpoint of this Bounds
        /// </summary>
        public Vector2 Center;
        /// <summary>
        /// A Vector containing Half the Width and Height Dimensions for this Bounds
        /// </summary>
        public Vector2 HalfDimensions;

        public float Width => HalfDimensions.X * 2f;

        public float Height => HalfDimensions.Y * 2f;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new Bounds by specifying the center, half width, and half height
        /// </summary>
        /// <param name="Center">the center coordinate</param>
        /// <param name="halfWidth">half the width measurement</param>
        /// <param name="halfHeight">half the height measurement</param>
        public Bounds(Vector2 Center, float halfWidth, float halfHeight)
        {
            this.Center = Center;
            HalfDimensions = new Vector2(halfWidth, halfHeight);
            TopLeft = Center - HalfDimensions;
            BottomRight = Center + HalfDimensions;
        }
        /// <summary>
        /// Create a new Bounds by specifying the TopLeft and BottomRight Corners
        /// </summary>
        /// <param name="TopLeft">the top left coordinate</param>
        /// <param name="BottomRight">the bottom right coordinate</param>
        public Bounds(Vector2 TopLeft, Vector2 BottomRight)
        {
            this.TopLeft = TopLeft;
            this.BottomRight = BottomRight;
            HalfDimensions = (BottomRight - TopLeft) / 2f;
            Center = TopLeft + HalfDimensions;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Gets the closest point within this Bounds to a specified position
        /// </summary>
        /// <param name="Position">the position strive to</param>
        /// <returns>the closest possible point to Position inside this bounds</returns>
        public Vector2 GetClosestPointTo(Vector2 Position)
        {
            Vector2 r = Center - Position;
            int sign = (r.X < 0) ? -1 : 1;
            r.X = Mathf.Max(Mathf.Abs(r.X), HalfDimensions.X) * sign;
            sign = (r.Y < 0) ? -1 : 1;
            r.Y = Mathf.Max(Mathf.Abs(r.Y), HalfDimensions.Y) * sign;
            return r;
        }

        /// <summary>
        /// check if this AABB intersects with another.
        /// </summary>
        /// <param name="other">the other aab to check against</param>
        /// <param name="EdgeTouchIsIntersection">if true, we count touching edges as intersections</param>
        /// <returns>true if an intersection is found</returns>
        public bool IntersectsWith(Bounds other, bool EdgeTouchIsIntersection)
        {

            return (EdgeTouchIsIntersection) ?
                (
                TopLeft.X       <=  other.BottomRight.X &&
                BottomRight.X   >=  other.TopLeft.X     &&
                TopLeft.Y       <=  other.BottomRight.Y &&
                BottomRight.Y   >=  other.TopLeft.Y
                ) 
                    :
                (
                TopLeft.X       <  other.BottomRight.X  &&
                BottomRight.X   >  other.TopLeft.X      &&
                TopLeft.Y       <  other.BottomRight.Y  &&
                BottomRight.Y   >  other.TopLeft.Y
                );
        }

        /// <summary>
        /// Check if a point is contianed in this Bounds
        /// </summary>
        /// <param name="Point">the point to check</param>
        /// <returns>true if the point is inside this bounds</returns>
        public bool Contains(Vector2 Point)
        {
            return (
                Point.X >= TopLeft.X        && 
                Point.X <= BottomRight.X    &&
                Point.Y <= BottomRight.Y    &&
                Point.Y >= TopLeft.Y
                );
        }
        #endregion
    }
}
