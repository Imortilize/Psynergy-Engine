using System.ComponentModel;
using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace Psynergy.TerrainPipeline
{
    /// <summary>
    /// Custom content processor for creating terrain meshes. Given an
    /// input heightfield texture, this processor uses the MeshBuilder
    /// class to programatically generate terrain geometry.
    /// </summary>
    [ContentProcessor(DisplayName = "Deferred Terrain Mesh Processor")]
    public class TerrainProcessor : ContentProcessor<Texture2DContent, ModelContent>
    {
        #region Fields
        #endregion

        #region Properties


        private float m_TerrainScale = 30f;
        [DisplayName("Terrain Scale")]
        [DefaultValue(30f)]
        [Description("Scale of the the terrain geometry width and length.")]
        public float TerrainScale
        {
            get { return m_TerrainScale; }
            set { m_TerrainScale = value; }
        }

        private float m_TerrainBumpiness = 640f;
        [DisplayName("Terrain Bumpiness")]
        [DefaultValue(640f)]
        [Description("Scale of the the terrain geometry height.")]
        public float TerrainBumpiness
        {
            get { return m_TerrainBumpiness; }
            set { m_TerrainBumpiness = value; }
        }

        private float m_TexCoordScale = 0.1f;
        [DisplayName("Texture Coordinate Scale")]
        [DefaultValue(0.1f)]
        [Description("Terrain texture tiling density.")]
        public float TexCoordScale
        {
            get { return m_TexCoordScale; }
            set { m_TexCoordScale = value; }
        }

        private string m_TerrainTextureFilename = "rocks.bmp";
        [DisplayName("Terrain Texture")]
        [DefaultValue("rocks.bmp")]
        [Description("The name of the terrain texture.")]
        public string TerrainTextureFilename
        {
            get { return m_TerrainTextureFilename; }
            set { m_TerrainTextureFilename = value; }
        }


        #endregion

        public override ModelContent Process(Texture2DContent input,
                                             ContentProcessorContext context)
        {
            MeshBuilder builder = MeshBuilder.StartMesh("terrain");

            // Convert the input texture to float format, for ease of processing.
            input.ConvertBitmapType(typeof(PixelBitmapContent<float>));

            PixelBitmapContent<float> heightfield;
            heightfield = (PixelBitmapContent<float>)input.Mipmaps[0];

            // Create the terrain vertices.
            for (int y = 0; y < heightfield.Height; y++)
            {
                for (int x = 0; x < heightfield.Width; x++)
                {
                    Vector3 position;

                    // position the vertices so that the heightfield is centered
                    // around x=0,z=0
                    position.X = m_TerrainScale * (x - ((heightfield.Width - 1) / 2.0f));
                    position.Z = m_TerrainScale * (y - ((heightfield.Height - 1) / 2.0f));

                    position.Y = (heightfield.GetPixel(x, y) - 1) * m_TerrainBumpiness;

                    builder.CreatePosition(position);
                }
            }

            // Create a material, and point it at our terrain texture.
            BasicMaterialContent material = new BasicMaterialContent();
            material.SpecularColor = new Vector3(.4f, .4f, .4f);

            string directory = Path.GetDirectoryName(input.Identity.SourceFilename);
            string texture = Path.Combine(directory, m_TerrainTextureFilename);

            material.Texture = new ExternalReference<TextureContent>(texture);

            builder.SetMaterial(material);

            // Create a vertex channel for holding texture coordinates.
            int texCoordId = builder.CreateVertexChannel<Vector2>(
                                            VertexChannelNames.TextureCoordinate(0));

            // Create the individual triangles that make up our terrain.
            for (int y = 0; y < heightfield.Height - 1; y++)
            {
                for (int x = 0; x < heightfield.Width - 1; x++)
                {
                    AddVertex(builder, texCoordId, heightfield.Width, x, y);
                    AddVertex(builder, texCoordId, heightfield.Width, x + 1, y);
                    AddVertex(builder, texCoordId, heightfield.Width, x + 1, y + 1);

                    AddVertex(builder, texCoordId, heightfield.Width, x, y);
                    AddVertex(builder, texCoordId, heightfield.Width, x + 1, y + 1);
                    AddVertex(builder, texCoordId, heightfield.Width, x, y + 1);
                }
            }

            // Chain to the ModelProcessor so it can convert the mesh we just generated.
            MeshContent terrainMesh = builder.FinishMesh();
            ModelContent model = context.Convert<MeshContent, ModelContent>(terrainMesh,"TerrainModelProcessor");

            // generate information about the height map, and attach it to the finished
            // model's tag.
            model.Tag = new TerrainContent(terrainMesh, m_TerrainScale, heightfield.Width, heightfield.Height);

            // Return the model
            return model;
        }


        /// <summary>
        /// Helper for adding a new triangle vertex to a MeshBuilder,
        /// along with an associated texture coordinate value.
        /// </summary>
        void AddVertex(MeshBuilder builder, int texCoordId, int w, int x, int y)
        {
            builder.SetVertexChannelData(texCoordId, new Vector2(x, y) * m_TexCoordScale);

            builder.AddTriangleVertex(x + y * w);
        }
    }
}
