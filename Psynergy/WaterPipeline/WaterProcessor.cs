using System.ComponentModel;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace Psynergy.WaterPipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.

    /// </summary>
    ///         

    [ContentProcessor(DisplayName = "Psynergy Water Processor")]
    public class WaterProcessor : ContentProcessor<Texture2DContent, ModelContent>
    {
        private float m_WaterScale = 1;
        [DisplayName("Water Scale")]
        [DefaultValue(1)]
        [Description("Scale of the the water geometry width and length.")]
        public float WaterScale
        {
            get { return m_WaterScale; }
            set { m_WaterScale = value; }
        }

        private float m_Width = 50f;
        [DisplayName("Water Width")]
        [DefaultValue(50f)]
        [Description("Width of water")]
        public float WaterWidth
        {
            get { return m_Width; }
            set { m_Width = value; }
        }

        private float m_Length = 50f;
        [DisplayName("Water Length")]
        [DefaultValue(50f)]
        [Description("Length of water")]
        public float WaterLength
        {
            get { return m_Length; }
            set { m_Length = value; }
        }

        private float m_TexCoordScale = 0.1f;
        [DisplayName("Texture Coordinate Scale")]
        [DefaultValue(0.1f)]
        [Description("Water texture tiling density.")]
        public float TexCoordScale
        {
            get { return m_TexCoordScale; }
            set { m_TexCoordScale = value; }
        }

        public override ModelContent Process(Texture2DContent input, ContentProcessorContext context)
        {
            MeshBuilder builder = MeshBuilder.StartMesh("water");

            // Convert the input texture to float format, for ease of processing.
            input.ConvertBitmapType(typeof(PixelBitmapContent<float>));

            PixelBitmapContent<float> waterMap;
            waterMap = (PixelBitmapContent<float>)input.Mipmaps[0];

            // Create the water vertices.
            for (int y = 0; y < m_Length; y++)
            {
                for (int x = 0; x < m_Width; x++)
                {
                    Vector3 position;

                    // position the vertices so that the heightfield is centered
                    // around x=0,z=0
                    position.X = m_WaterScale * (x - ((m_Width - 1) / 2.0f));
                    position.Z = m_WaterScale * (y - ((m_Length - 1) / 2.0f));

                    // Flat 
                    position.Y = 0;

                    // Create position
                    builder.CreatePosition(position);
                }
            }

            // material stuff...
            // Create a vertex channel for holding texture coordinates.
            int texCoordId = builder.CreateVertexChannel<Vector2>(
                                            VertexChannelNames.TextureCoordinate(0));

            // Create the individual triangles that make up our water.
            for (int y = 0; y < m_Length - 1; y++)
            {
                for (int x = 0; x < m_Width - 1; x++)
                {
                    AddVertex(builder, texCoordId, (int)m_Width, x, y);
                    AddVertex(builder, texCoordId, (int)m_Width, (x + 1), y);
                    AddVertex(builder, texCoordId, (int)m_Width, (x + 1), (y + 1));

                    AddVertex(builder, texCoordId, (int)m_Length, x, y);
                    AddVertex(builder, texCoordId, (int)m_Length, (x + 1), (y + 1));
                    AddVertex(builder, texCoordId, (int)m_Length, x, (y + 1));
                }
            }

            // Chain to the ModelProcessor so it can convert the mesh we just generated.
            MeshContent waterMesh = builder.FinishMesh();

            // Convert the meshcontent into modelcontent
            ModelContent model = context.Convert<MeshContent, ModelContent>(waterMesh, "WaterModelProcessor");

            // generate information about the height map, and attach it to the finished
            // model's tag.
            model.Tag = new WaterContent(WaterScale);

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

            // Add triangle vertex
            builder.AddTriangleVertex(x + y * w);
        }
    }
}