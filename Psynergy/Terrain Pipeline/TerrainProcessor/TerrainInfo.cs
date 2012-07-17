using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

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
        public TerrainInfo(float[,] heights, Vector3[,] normals, float terrainScale)
        {
            if (heights == null)
                throw new ArgumentNullException("heights");

            if (normals == null)
                throw new ArgumentNullException("normals");

            m_TerrainScale = terrainScale;
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
            float m_TerrainScale = input.ReadSingle();
            int m_Width = input.ReadInt32();
            int m_Height = input.ReadInt32();
            float[,] m_Heights = new float[m_Width, m_Height];
            Vector3[,] m_Normals = new Vector3[m_Width, m_Height];

            for (int x = 0; x < m_Width; x++)
            {
                for (int z = 0; z < m_Height; z++)
                {
                    m_Heights[x, z] = input.ReadSingle();
                }
            }
            for (int x = 0; x < m_Width; x++)
            {
                for (int z = 0; z < m_Height; z++)
                {
                    m_Normals[x, z] = input.ReadVector3();
                }
            }

            return new TerrainInfo(m_Heights, m_Normals, m_TerrainScale);
        }
    }
}
