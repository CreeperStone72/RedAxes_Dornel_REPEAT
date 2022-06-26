using System;
using UnityEngine;
using Utility;

namespace ProceduralLevelGeneration.Room
{
    /// <summary>
    /// Data representation of a room as a five-tuple
    /// SOURCE : J. A. Brown, B. Lutfullin and P. Oreshin,
    /// "Procedural content generation of level layouts for Hotline Miami",
    /// 2017 9th Computer Science and Electronic Engineering (CEEC), 2017,
    /// pp. 106-111, doi: 10.1109/CEEC.2017.8101608.
    /// LAST ACCESSED : 24/06
    /// </summary>
    [Serializable]
    public class Room
    {
        /// <summary>Starting x-coordinate of a room</summary>
        public int x;
        
        /// <summary>Starting y-coordinate of a room</summary>
        public int y;

        /// <summary>Length of the room</summary>
        public int l;
        
        /// <summary>Width of the room</summary>
        public int w;
        
        public Room(int x, int y, int l, int w)
        {
            this.x = x;
            this.y = y;
            this.l = l;
            this.w = w;
        }

        public int Width => w;

        public float Area => w * l;

        public bool IsNarrow => w == 1 || l == 1;

        public bool IsTiny => w == 1 && l == 1;
        
        public Bounds Bounds => BoundsFactory.GetBounds(this);

        public bool Intersects(Room other) { return Bounds.Intersects(other.Bounds); }

        /// <summary>
        /// Calculates the intersection of two rooms
        /// Original code by GeeksforGeeks, altered by me to return the bounds of the intersection
        /// SOURCE : https://www.geeksforgeeks.org/total-area-two-overlapping-rectangles/
        /// LAST ACCESSED : 26/06
        /// </summary>
        /// <param name="other">The other room</param>
        /// <returns>Empty Bounds if the rooms don't overlap, the bounds of the overlap otherwise</returns>
        public Bounds GetIntersection(Room other) { return !Intersects(other) ? BoundsFactory.Zero : BoundsFactory.Inter(Bounds, other.Bounds); }

        public float GetIntersectionArea(Room other) { return BoundsFactory.BoundsArea(GetIntersection(other)); }

        public float GetUnionArea(Room other) { return Area + other.Area - GetIntersectionArea(other); }
        
        public Mesh ToMesh() { return MeshFactory.BuildMesh(x, y, w, l); }

        public delegate Room Mutate();

        public override string ToString() { return $"({x} - {y} - {l} - {w})"; }
    }
}