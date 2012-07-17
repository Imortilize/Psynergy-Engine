using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Graphics
{
    public class Helpers
    {
        public static void TransformBoundingBox(ref BoundingBox src, ref Matrix transform, out BoundingBox dst)
        {
            Vector3 center, extents, rotatedExtents = Vector3.Zero;

            center = (src.Min + src.Max) * 0.5f;
            extents = src.Max - center;

            rotatedExtents.X = Math.Abs(extents.X * transform.Right.X) + Math.Abs(extents.Y * transform.Up.X) + Math.Abs(extents.Z * transform.Forward.X);
            rotatedExtents.Y = Math.Abs(extents.X * transform.Right.Y) + Math.Abs(extents.Y * transform.Up.Y) + Math.Abs(extents.Z * transform.Forward.Y);
            rotatedExtents.Z = Math.Abs(extents.X * transform.Right.Z) + Math.Abs(extents.Y * transform.Up.Z) + Math.Abs(extents.Z * transform.Forward.Z);

            center = Vector3.Transform(center, transform);

            dst.Min = center - rotatedExtents;
            dst.Max = center + rotatedExtents;
        }
    }
}
