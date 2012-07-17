using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework;

namespace Psynergy.TerrainPipeline
{
    /// <summary>
    /// HeightMapInfoContent contains information about a size and heights of a
    /// heightmap. When the game is being built, it is constructed by the
    /// TerrainProcessor, and attached to the finished terrain's Tag. When the game is
    /// run, it will be read in as a HeightMapInfo.
    /// </summary>
    public class TerrainContent
    {
        /// <summary>
        /// This propery is a 2D array of floats, and tells us the height that each
        /// position in the heightmap is.
        /// </summary>
        public float[,] Height
        {
            get { return m_Height; }
        }

        float[,] m_Height;

        /// <summary>
        /// This property is a 2D array of Vector3s, and tells us the normal that each
        /// position in the heightmap is.
        /// </summary>
        public Vector3[,] Normals
        {
            get { return m_Normals; }
            set { m_Normals = value; }
        }
        private Vector3[,] m_Normals;

        /// <summary>
        /// TerrainScale is the distance between each entry in the Height property.
        /// For example, if TerrainScale is 30, Height[0,0] and Height[1,0] are 30
        /// units apart.
        /// </summary>
        public float TerrainScale
        {
            get { return m_TerrainScale; }
        }

        private float m_TerrainScale;

        public TerrainContent(MeshContent terrainMesh, float terrainScale, int terrainWidth, int terrainLength)
        {
            // validate the parameters
            if (terrainMesh == null)
                throw new ArgumentNullException("terrainMesh");

            if (terrainWidth <= 0)
                throw new ArgumentOutOfRangeException("terrainWidth");

            if (terrainLength <= 0)
                throw new ArgumentOutOfRangeException("terrainLength");

            // Set the terrain scale
            m_TerrainScale = terrainScale;

            // Create new arrays of the requested size
            m_Height = new float[terrainWidth, terrainLength];
            m_Normals = new Vector3[terrainWidth, terrainLength];

            // To fill those arrays we will look at the position and normal data contained 
            // in the terrain mesh
            GeometryContent geometry = terrainMesh.Geometry[0];

            // Go through each vertex...
            for (int i = 0; i < geometry.Vertices.VertexCount; i++)
            {
                // Look up the position and normal...
                Vector3 position = geometry.Vertices.Positions[i];
                Vector3 normal = (Vector3)geometry.Vertices.Channels[VertexChannelNames.Normal()][i];

                // From the positions X and Z value, we can tell what X and Y 
                // coordinate of the arrays to put the height and normal into.
                int arrayX = (int)((position.X / terrainScale) + (terrainWidth - 1) * 0.5f);
                int arrayY = (int)((position.Z / terrainScale) + (terrainLength - 1) * 0.5f);

                // Store values
                m_Height[arrayX, arrayY] = position.Y;
                m_Normals[arrayX, arrayY] = normal;
            }
        }
    }


    /// <summary>
    /// A TypeWriter for HeightMapInfo, which tells the content pipeline how to save the
    /// data in HeightMapInfo. This class should match HeightMapInfoReader: whatever the
    /// writer writes, the reader should read.
    /// </summary>
    [ContentTypeWriter]
    public class HeightMapInfoWriter : ContentTypeWriter<TerrainContent>
    {
        protected override void Write(ContentWriter output, TerrainContent value)
        {
            output.Write(value.TerrainScale);

            output.Write(value.Height.GetLength(0));
            output.Write(value.Height.GetLength(1));

            // Write the mesh heights
            foreach (float height in value.Height)
            {
                output.Write(height);
            }

            // Write the mesh normals
            foreach (Vector3 normal in value.Normals)
            {
                output.Write(normal);
            }
        }

        /// <summary>
        /// Tells the content pipeline what CLR type the
        /// data will be loaded into at runtime.
        /// </summary>
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "Psynergy.TerrainPipeline.TerrainInfo, " +
                "Psynergy.TerrainPipeline, Version=1.0.0.0, Culture=neutral";
        }


        /// <summary>
        /// Tells the content pipeline what worker type
        /// will be used to load the data.
        /// </summary>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Psynergy.TerrainPipeline.TerrainInfoReader, " +
                "Psynergy.TerrainPipeline, Version=1.0.0.0, Culture=neutral";
        }
    }
}
