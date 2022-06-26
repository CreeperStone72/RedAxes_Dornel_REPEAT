using System;
using ProceduralLevelGeneration.Room;
using UnityEngine;

namespace Utility
{
    public static class BoundsFactory
    {
        public static readonly Bounds Zero = new Bounds(Vector3.zero, Vector3.zero);

        public static Bounds GetBounds(Room room)
        {
            var bounds = new Bounds(Vector3.zero, Vector3.zero);
            bounds.SetMinMax(new Vector3(room.x, 0, room.y), new Vector3(room.x + room.w, 0, room.y + room.l));
            return bounds;
        }
        
        public static Bounds GetBounds(Vector3 min, Vector3 max)
        {
            var bounds = Zero;
            bounds.SetMinMax(min, max);
            return bounds;
        }
        
        public static float BoundsArea(Bounds b) { return b.size.x * b.size.z; }

        public static Bounds Inter(Bounds b1, Bounds b2)
        {
            if (!b1.Intersects(b2)) return Zero;

            var min = Vector3.Max(b1.min, b2.min);
            var max = Vector3.Min(b1.max, b2.max);
            
            return GetBounds(min, max);
        }
        
        public static bool Borders(Bounds b, Vector3 point)
        {
            const float tolerance = 0f;
            
            var x = Math.Abs(point.x - b.min.x) < tolerance || Math.Abs(point.x - b.max.x) < tolerance;
            var y = Math.Abs(point.y - b.min.y) < tolerance || Math.Abs(point.y - b.max.y) < tolerance;
            var z = Math.Abs(point.z - b.min.z) < tolerance || Math.Abs(point.z - b.max.z) < tolerance;

            return x || y || z;
        }
    }
}