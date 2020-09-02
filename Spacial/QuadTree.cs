using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Leo.ECS.Spacial
{
    public class QuadTree<T>
    {
        #region Variables
        public struct BoundsLink<K>
        {
            public K link;
            public Bounds bounds;
            public BoundsLink(K link, Bounds bounds)
            {
                this.link = link;
                this.bounds = bounds;
            }
        }

        private const int MaxReferencesPerTree = 3;
        public Bounds Bounds;
        private QuadTree<T> NW, NE, SW, SE;
        private bool IsDivided = false;
        private HashSet<BoundsLink<T>> Members = new HashSet<BoundsLink<T>>();
        public List<BoundsLink<T>> AllMembers = new List<BoundsLink<T>>();
        #endregion

        #region Constructors
        public QuadTree(Bounds bounds)
        {
            this.Bounds = bounds;
        }
        public QuadTree(float halfWidth, float halfHeight)
        {
            Bounds = new Bounds(Vector2.Zero, halfWidth, halfHeight);
        }
        #endregion

        #region Quadtree functionality
        public void Reset()
        {
            if (IsDivided)
            {
                NW.Reset();
                NE.Reset();
                SW.Reset();
                SE.Reset();
            }
            Members.Clear();
            AllMembers.Clear();
            NW = NE = SW = SE = null;
            IsDivided = false;
        }

        public List<T> QueryBounds(Bounds bounds, bool EdgeTouchIsIntersection)
        {
            List<T> results = new List<T>();
            if (!Bounds.IntersectsWith(bounds, EdgeTouchIsIntersection))
                return results; // return empty list
            // else check our members
            foreach (BoundsLink<T> member in Members)
            {
                if (member.bounds.IntersectsWith(bounds, EdgeTouchIsIntersection))
                    results.Add(member.link);
            }
            // check our leaves as well
            if (IsDivided)
            {
                results.AddRange(NW.QueryBounds(bounds, EdgeTouchIsIntersection));
                results.AddRange(NE.QueryBounds(bounds, EdgeTouchIsIntersection));
                results.AddRange(SW.QueryBounds(bounds, EdgeTouchIsIntersection));
                results.AddRange(SE.QueryBounds(bounds, EdgeTouchIsIntersection));
            }
            // return the compounded list
            return results;
        }

        private void Subdivide()
        {
            Vector2 newHalfDims = Bounds.HalfDimensions / 2f;
            Vector2 newCenter = Bounds.Center - newHalfDims;
            SW = new QuadTree<T>(new Bounds(newCenter, newHalfDims.X, newHalfDims.Y));
            newCenter.Y += Bounds.HalfDimensions.Y;
            NW = new QuadTree<T>(new Bounds(newCenter, newHalfDims.X, newHalfDims.Y));
            newCenter.X += Bounds.HalfDimensions.X;
            NE = new QuadTree<T>(new Bounds(newCenter, newHalfDims.X, newHalfDims.Y));
            newCenter.Y -= Bounds.HalfDimensions.Y;
            SE = new QuadTree<T>(new Bounds(newCenter, newHalfDims.X, newHalfDims.Y));
            IsDivided = true;
        }

        private bool Insert(BoundsLink<T> member)
        {
            // make sure we intersect with the bounds of this object
            if (!Bounds.IntersectsWith(member.bounds, true))
                return false;
            if (Members.Count < MaxReferencesPerTree)
            {
                Members.Add(member);
                return true;
            }

            // else we are full, but need to store this object
            if (!IsDivided)
                Subdivide();

            bool Inserted = false;
            if (SW.Insert(member)) Inserted = true;
            if (NW.Insert(member)) Inserted = true;
            if (NE.Insert(member)) Inserted = true;
            if (SE.Insert(member)) Inserted = true;
            return Inserted;
        }

        /// <summary>
        /// Insert an item into this QuadTree with the specified Bounds
        /// </summary>
        /// <param name="item">the item to insert</param>
        /// <param name="bounds">the bounds of the item</param>
        /// <returns>true if the item could be inserted, false otherwise</returns>
        public bool Insert(T item, Bounds bounds)
        {
            BoundsLink<T> member = new BoundsLink<T>(item, bounds);
            bool ret = Insert(member);
            if (ret)
                AllMembers.Add(member);

            return ret;
        }
        #endregion

        #region DrawInGizmos
        public void DrawInGizmos()
        {
            Gizmos.Color = Color.White;
            Gizmos.DrawRectangle(Bounds.Center, Bounds.HalfDimensions.X, Bounds.HalfDimensions.Y, 10);
            if (IsDivided)
            {
                NW.DrawInGizmos();
                NE.DrawInGizmos();
                SW.DrawInGizmos();
                SE.DrawInGizmos();
            }
        }
        #endregion
    }
}
