using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

using Psynergy.Camera;

namespace Psynergy.TerrainPipeline
{
    /// <summary>
    /// HeightMapInfo is a collection of data about the heightmap. It includes
    /// information about how high the terrain is, and how far apart each vertex is.
    /// It also has several functions to get information about the heightmap, including
    /// its height at different points, and whether a point is on the heightmap.
    /// It is the runtime equivalent of HeightMapInfoContent.
    /// </summary>
    public class TerrainInfo
    {
        #region Private fields

        // TerrainScale is the distance between each entry in the Height property.
        // For example, if TerrainScale is 30, Height[0,0] and Height[1,0] are 30
        // units apart.        
        private float m_TerrainScale;

        // Terrain vertices
        private Vector3[] m_Vertices;

        // Terrain normals
        private Vector3[,] m_Normals;

        // This 2D array of floats tells us the height that each position in the 
        // heightmap is.
        private float[,] m_Heights;

        // the position of the heightmap's -x, -z corner, in worldspace.
        private Vector3 m_HeightmapPosition;

        // the total width of the heightmap, including terrainscale.
        private float m_HeightmapWidth;

        // the total height of the height map, including terrainscale.
        private float m_HeightmapHeight;
        #endregion


        // the constructor will initialize all of the member variables.
        public TerrainInfo(Vector3[] vertices, float[,] heights, Vector3[,] normals, float terrainScale)
        {
            if (heights == null)
                throw new ArgumentNullException("heights");

            if (normals == null)
                throw new ArgumentNullException("normals");

            m_TerrainScale = terrainScale;
            m_Vertices = vertices;
            m_Heights = heights;
            m_Normals = normals;

            m_HeightmapWidth = (heights.GetLength(0) - 1) * terrainScale;
            m_HeightmapHeight = (heights.GetLength(1) - 1) * terrainScale;

            m_HeightmapPosition.X = -(heights.GetLength(0) - 1) / 2 * terrainScale;
            m_HeightmapPosition.Z = -(heights.GetLength(1) - 1) / 2 * terrainScale;
        }


        // This function takes in a position, and tells whether or not the position is 
        // on the heightmap.
        public bool IsOnHeightmap(Vector3 position)
        {
            // first we'll figure out where on the heightmap "position" is...
            Vector3 positionOnHeightmap = position - m_HeightmapPosition;

            // ... and then check to see if that value goes outside the bounds of the
            // heightmap.
            return (positionOnHeightmap.X > 0 &&
                positionOnHeightmap.X < m_HeightmapWidth &&
                positionOnHeightmap.Z > 0 &&
                positionOnHeightmap.Z < m_HeightmapHeight);
        }

        // This function takes in a position, and returns the heightmap's height at that
        // point. Be careful - this function will throw an IndexOutOfRangeException if
        // position isn't on the heightmap!
        // This function is explained in more detail in the accompanying doc.
        public float GetHeight(Vector3 position)
        {
            float toRet = position.Y;

            if (IsOnHeightmap(position))
            {
                // the first thing we need to do is figure out where on the heightmap
                // "position" is. This'll make the math much simpler later.
                Vector3 positionOnHeightmap = position - m_HeightmapPosition;

                // we'll use integer division to figure out where in the "heights" array
                // positionOnHeightmap is. Remember that integer division always rounds
                // down, so that the result of these divisions is the indices of the "upper
                // left" of the 4 corners of that cell.
                int left, top;
                left = (int)positionOnHeightmap.X / (int)m_TerrainScale;
                top = (int)positionOnHeightmap.Z / (int)m_TerrainScale;

                // next, we'll use modulus to find out how far away we are from the upper
                // left corner of the cell. Mod will give us a value from 0 to terrainScale,
                // which we then divide by terrainScale to normalize 0 to 1.
                float xNormalized = (positionOnHeightmap.X % m_TerrainScale) / m_TerrainScale;
                float zNormalized = (positionOnHeightmap.Z % m_TerrainScale) / m_TerrainScale;

                // Now that we've calculated the indices of the corners of our cell, and
                // where we are in that cell, we'll use bilinear interpolation to calculuate
                // our height. This process is best explained with a diagram, so please see
                // the accompanying doc for more information.
                // First, calculate the heights on the bottom and top edge of our cell by
                // interpolating from the left and right sides.
                float topHeight = MathHelper.Lerp(
                    m_Heights[left, top],
                    m_Heights[left + 1, top],
                    xNormalized);

                float bottomHeight = MathHelper.Lerp(
                    m_Heights[left, top + 1],
                    m_Heights[left + 1, top + 1],
                    xNormalized);

                // next, interpolate between those two values to calculate the height at our
                // position.
                toRet = MathHelper.Lerp(topHeight, bottomHeight, zNormalized);
            }

            return toRet;
        }

        public Vector3 GetNormal(Vector3 position)
        {
            Vector3 toRet = Vector3.Up;

            if (IsOnHeightmap(position))
            {
                // the first thing we need to do is figure out where on the heightmap
                // "position" is. This'll make the math much simpler later.
                Vector3 positionOnHeightmap = position - m_HeightmapPosition;

                // we'll use integer division to figure out where in the "heights" array
                // positionOnHeightmap is. Remember that integer division always rounds
                // down, so that the result of these divisions is the indices of the "upper
                // left" of the 4 corners of that cell.
                int left, top;
                left = (int)positionOnHeightmap.X / (int)m_TerrainScale;
                top = (int)positionOnHeightmap.Z / (int)m_TerrainScale;

                // next, we'll use modulus to find out how far away we are from the upper
                // left corner of the cell. Mod will give us a value from 0 to terrainScale,
                // which we then divide by terrainScale to normalize 0 to 1.
                float xNormalized = (positionOnHeightmap.X % m_TerrainScale) / m_TerrainScale;
                float zNormalized = (positionOnHeightmap.Z % m_TerrainScale) / m_TerrainScale;

                // We'll repeat the same process to calculate the normal.
                Vector3 topNormal = Vector3.Lerp(
                    m_Normals[left, top],
                    m_Normals[left + 1, top],
                    xNormalized);

                Vector3 bottomNormal = Vector3.Lerp(
                    m_Normals[left, top + 1],
                    m_Normals[left + 1, top + 1],
                    xNormalized);

                toRet = Vector3.Lerp(topNormal, bottomNormal, zNormalized);
                toRet.Normalize();
            }

            return toRet;
        }

        // This function takes in a position, and returns the heightmap's height at that
        // point. Be careful - this function will throw an IndexOutOfRangeException if
        // position isn't on the heightmap!
        // This function is explained in more detail in the accompanying doc.
        public void GetHeightAndNormal(Vector3 position, out float height, out Vector3 normal)
        {
            height = position.Y;
            normal = Vector3.Up;

            if (IsOnHeightmap(position))
            {
                // the first thing we need to do is figure out where on the heightmap
                // "position" is. This'll make the math much simpler later.
                Vector3 positionOnHeightmap = position - m_HeightmapPosition;

                // we'll use integer division to figure out where in the "heights" array
                // positionOnHeightmap is. Remember that integer division always rounds
                // down, so that the result of these divisions is the indices of the "upper
                // left" of the 4 corners of that cell.
                int left, top;
                left = (int)positionOnHeightmap.X / (int)m_TerrainScale;
                top = (int)positionOnHeightmap.Z / (int)m_TerrainScale;

                // next, we'll use modulus to find out how far away we are from the upper
                // left corner of the cell. Mod will give us a value from 0 to terrainScale,
                // which we then divide by terrainScale to normalize 0 to 1.
                float xNormalized = (positionOnHeightmap.X % m_TerrainScale) / m_TerrainScale;
                float zNormalized = (positionOnHeightmap.Z % m_TerrainScale) / m_TerrainScale;

                // Now that we've calculated the indices of the corners of our cell, and
                // where we are in that cell, we'll use bilinear interpolation to calculuate
                // our height. This process is best explained with a diagram, so please see
                // the accompanying doc for more information.
                // First, calculate the heights on the bottom and top edge of our cell by
                // interpolating from the left and right sides.
                float topHeight = MathHelper.Lerp(
                    m_Heights[left, top],
                    m_Heights[left + 1, top],
                    xNormalized);

                float bottomHeight = MathHelper.Lerp(
                    m_Heights[left, top + 1],
                    m_Heights[left + 1, top + 1],
                    xNormalized);

                // next, interpolate between those two values to calculate the height at our
                // position.
                height = MathHelper.Lerp(topHeight, bottomHeight, zNormalized);

                // We'll repeat the same process to calculate the normal.
                Vector3 topNormal = Vector3.Lerp(
                    m_Normals[left, top],
                    m_Normals[left + 1, top],
                    xNormalized);

                Vector3 bottomNormal = Vector3.Lerp(
                    m_Normals[left, top + 1],
                    m_Normals[left + 1, top + 1],
                    xNormalized);

                normal = Vector3.Lerp(topNormal, bottomNormal, zNormalized);
                normal.Normalize();
            }
        }

        /* Used to pick a position on the terrain */
        public Vector3? Pick(Ray ray, Vector3 position, Matrix world)
        {
            Vector3? intersectionPoint = null;

           // Matrix inverseTransform = Matrix.Invert(Matrix.CreateScale(30));

            // Transform the ray into object space
           /// ray.Position = Vector3.Transform(ray.Position, inverseTransform);
           // ray.Direction = Vector3.TransformNormal(ray.Direction, inverseTransform);

            // Keep track of the closest triangle we found so far,
            // so we can always return the closest one.
            float? closestIntersection = null;

            // Cycle through the list of vertices in the mesh
            for (int i = 0; i < m_Vertices.Length; i += 3)
            {
                Vector3 vertex1 = (m_Vertices[i] + position);
                Vector3 vertex2 = (m_Vertices[i + 1] + position);
                Vector3 vertex3 = (m_Vertices[i + 2] + position);

                float? newIntersection = RayIntersectsTriangle(ref ray, ref vertex1, ref vertex2, ref vertex3);

                // Does the ray intersect this triangle?
                if (newIntersection != null)
                {
                    // If so, is it closer than any other previous triangle?
                    if ((closestIntersection == null) || (newIntersection < closestIntersection))
                    {
                        // Store the distance to this triangle.
                        closestIntersection = newIntersection;

                        // Intersection point will be the ray direction * the intersection which is the
                        // distance to the intersection
                        intersectionPoint = (ray.Direction * closestIntersection);

                        break;
                    }
                }
            }

            // Return the intersection point whether it is null or not
            return intersectionPoint;
        }

        /// <summary>
        /// Checks whether a ray intersects a triangle. This uses the algorithm
        /// developed by Tomas Moller and Ben Trumbore, which was published in the
        /// Journal of Graphics Tools, volume 2, "Fast, Minimum Storage Ray-Triangle
        /// Intersection".
        /// 
        /// This method is implemented using the pass-by-reference versions of the
        /// XNA math functions. Using these overloads is generally not recommended,
        /// because they make the code less readable than the normal pass-by-value
        /// versions. This method can be called very frequently in a tight inner loop,
        /// however, so in this particular case the performance benefits from passing
        /// everything by reference outweigh the loss of readability.
        /// </summary>
        private float? RayIntersectsTriangle(ref Ray ray, ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3)
        {
            // Compute vectors along two edges of the triangle.
            Vector3 edge1, edge2;

            Vector3.Subtract(ref vertex2, ref vertex1, out edge1);
            Vector3.Subtract(ref vertex3, ref vertex1, out edge2);

            // Compute the determinant.
            Vector3 directionCrossEdge2;
            Vector3.Cross(ref ray.Direction, ref edge2, out directionCrossEdge2);

            float determinant;
            Vector3.Dot(ref edge1, ref directionCrossEdge2, out determinant);

            // If the ray is parallel to the triangle plane, there is no collision.
            if (determinant > -float.Epsilon && determinant < float.Epsilon)
                return null;

            float inverseDeterminant = 1.0f / determinant;

            // Calculate the U parameter of the intersection point.
            Vector3 distanceVector;
            Vector3.Subtract(ref ray.Position, ref vertex1, out distanceVector);

            float triangleU;
            Vector3.Dot(ref distanceVector, ref directionCrossEdge2, out triangleU);
            triangleU *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if ((triangleU < 0) || (triangleU > 1))
                return null;

            // Calculate the V parameter of the intersection point.
            Vector3 distanceCrossEdge1;
            Vector3.Cross(ref distanceVector, ref edge1, out distanceCrossEdge1);

            float triangleV;
            Vector3.Dot(ref ray.Direction, ref distanceCrossEdge1, out triangleV);
            triangleV *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if ((triangleV < 0) || ((triangleU + triangleV) > 1))
                return null;

            // Compute the distance along the ray to the triangle.
            float rayDistance;
            Vector3.Dot(ref edge2, ref distanceCrossEdge1, out rayDistance);
            rayDistance *= inverseDeterminant;

            // Is the triangle behind the ray origin?
            if (rayDistance < 0)
                return null;

            return rayDistance;
        }
        /**/

        #region Property Set / Get
        public Vector3 Position { get { return m_HeightmapPosition; } set { m_HeightmapPosition = value; } }
        #endregion
    }



    /// <summary>
    /// This class will load the TerrainInfo when the game starts. This class needs 
    /// to match the TerrainInfoWriter.
    /// </summary>
    public class TerrainInfoReader : ContentTypeReader<TerrainInfo>
    {
        protected override TerrainInfo Read(ContentReader input, TerrainInfo existingInstance)
        {
            float terrainScale = input.ReadSingle();
            int width = input.ReadInt32();
            int height = input.ReadInt32();
            float[,] heights = new float[width, height];
            Vector3[,] normals = new Vector3[width, height];

            // Get the terrain heights
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    heights[x, z] = input.ReadSingle();
                }
            }

            // Get the terrain normals
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    normals[x, z] = input.ReadVector3();
                }
            }

            // Read in the vertex list to use for terrain picking
            int vertCount = input.ReadInt32();
            Vector3[] vertices = new Vector3[vertCount];

            for (int i = 0; i < vertCount; i++)
                vertices[i] = input.ReadVector3();

            return new TerrainInfo(vertices, heights, normals, terrainScale);
        }
    }
}
