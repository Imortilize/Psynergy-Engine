using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Psynergy
{
    public static class RandomHelper
    {
        public static Random m_RandomGenerator = new Random();

        public static Vector3 GeneratePositionXZ(int distance)
        {
            float posX = (m_RandomGenerator.Next(distance * 201) - distance * 100) * 0.01f;
            float posZ = (m_RandomGenerator.Next(distance * 201) - distance * 100) * 0.01f;

            return new Vector3(posX, 0, posZ);
        }

        public static Vector3 GenerationPositionXZFromView(Vector3 pos, Vector3 axisVec, float radius, int distance)
        {
            int halfRadius = (int)(radius * 0.5f);
            int randomRadiusPoint = m_RandomGenerator.Next(-halfRadius, halfRadius);

            //Vector3 axis = (axisVec * radius);
            Vector3 newPos = Vector3.Transform(axisVec, Matrix.CreateRotationY(MathHelper.ToRadians(randomRadiusPoint)));
            Vector3 dir = (newPos - pos);
            
            // Normalise the newPosition to get the direction of the new position
            dir.Normalize();

            // Generate a random distance for this new position
            float randomDistance = m_RandomGenerator.Next(0, distance);

            // Return the direction vector multiplied by the new position distance
            return (dir * randomDistance);
        }
    }
}
