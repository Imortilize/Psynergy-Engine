using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy
{
    public class MathLib : Singleton<MathLib>
    {
        public MathLib()
        {
        }

        public Vector3 QuaternionToEuler(Quaternion q)
        {
            Vector3 euler;

            float sqx = q.X * q.X;
            float sqy = q.Y * q.Y;
            float sqz = q.Z * q.Z;
            float sqw = q.W * q.W;

            float unit = sqx + sqy + sqz + sqw;
            float test = (q.X * q.W - q.Y * q.Z);

            // Handle singularity
            if (test > 0.4999999f * unit)
            {
                euler.X = MathHelper.PiOver2;
                euler.Y = 2.0f * (float)System.Math.Atan2(q.Y, q.W);
                euler.Z = 0;
            }
            else if (test < -0.4999999f * unit)
            {
                euler.X = -MathHelper.PiOver2;
                euler.Y = 2.0f * (float)System.Math.Atan2(q.Y, q.W);
                euler.Z = 0;
            }
            else
            {
                float ey_Y = 2 * (q.X * q.Z + q.Y * q.W);
                float ey_X = 1 - 2 * (sqy + sqx);
                float ez_Y = 2 * (q.X * q.Y + q.Z * q.W);
                float ez_X = 1 - 2 * (sqx + sqz);
                euler.X = (float)System.Math.Asin(2 * test);
                euler.Y = (float)System.Math.Atan2(ey_Y, ey_X);
                euler.Z = (float)System.Math.Atan2(ez_Y, ez_X);
            }

            // Convert to degrees
            euler.X = MathHelper.ToDegrees(euler.X);
            euler.Y = MathHelper.ToDegrees(euler.Y);
            euler.Z = MathHelper.ToDegrees(euler.Z);

            return euler;
        }

        // Returns Euler angles that point from one point to another
        public Vector3 AngleTo(Vector3 from, Vector3 location)
        {
            Vector3 angle = new Vector3();
            Vector3 v3 = Vector3.Normalize(location - from);

            angle.X = (float)Math.Asin(v3.Y);
            angle.Y = (float)Math.Atan2((double)-v3.X, (double)-v3.Z);
            //angle.Z = (float)(-1.0f * Math.Sin(-v3.X));

            return angle;
        }
    }
}
