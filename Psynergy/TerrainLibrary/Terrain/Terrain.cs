using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SkinnedModel;

using Psynergy;
using Psynergy.Graphics;
using Psynergy.TerrainPipeline;
using Psynergy.Camera;

namespace Psynergy.Graphics.Terrain
{
    public class Terrain : ModelNode, IRegister<Terrain>
    {
        private TerrainInfo m_TerrainInfo = null;

        public Terrain() : base("")
        {
        }

        public Terrain(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            // Cube specific variables
            //TextureName = "Textures/Others/default";
            //RenderGroupName = "shadow";

            // Set modelName
            ModelName = "Textures/HeightMaps/terrain";
            //Scale *= 0.1f;

            base.Initialise();
        }

        public override void  Load()
        {
 	         base.Load();

            // Fire a loaded event saying this terrain has been loaded
            TerrainLoadedEvent terrainLoadedEvent = new TerrainLoadedEvent(this);
            terrainLoadedEvent.Fire();
        }

        protected override void LoadSpecific()
        {
            base.LoadSpecific();

            // State that this should ignore camera culling 
            // ( stops sky spheres dissapearing due to camera not binding with it )
            if (m_Mesh != null)
                m_Mesh.IgnoreCameraCulling = true;
        }

        protected override Mesh CreateMesh()
        {
            Mesh mesh = base.CreateMesh();

            // Create mesh data
            MeshMetaData meta = new MeshMetaData();
            meta.BoundingBox = m_BoundingBox;

            TerrainMeshMetaData terrainMeta = new TerrainMeshMetaData();
            m_TerrainInfo = terrainMeta.TerrainInfo = (m_Model.Tag as TerrainInfo);
            terrainMeta.EnableLighting = true;

            // Hacked in for now to see what it looks like
            terrainMeta.UseNormalMap = true;

            terrainMeta.BoundingBox = m_BoundingBox;

            // Add as sub mesh
            meta.AddSubMeshMetadata(terrainMeta);

            // Save meta data
            m_Model.Tag = meta;

            // Get correct render effect
            foreach ( ModelMesh modelMesh in m_Model.Meshes )
            {
                foreach (ModelMeshPart modelMeshPart in modelMesh.MeshParts)
                {
                    modelMeshPart.Effect = RenderManager.Instance.GetEffect("RenderGBuffer");

                    if (modelMeshPart.Effect != null)
                    {
                        if ( m_Textures.Count > 0 )
                            modelMeshPart.Effect.Parameters["Texture"].SetValue(m_Textures[0]);

                        modelMeshPart.Effect.Parameters["NormalMap"].SetValue(RenderManager.Instance.LoadTexture2D("Textures/GrassNormalMap"));
                    }
                }
            }

            // Create a metameshdata tag
            return mesh;
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);     
        }

        #region Terrain Picking
        /* Used to pick a position on the terrain */
        public Vector3? Pick()
        {
            Vector3? intersectionPoint = null;

            if (m_TerrainInfo != null)
            {
                // Get the active camera
                BaseCamera camera = CameraManager.Instance.ActiveCamera;

                // Camera must not be null
                Debug.Assert(camera != null, "Camera can not be null when picking on terrain!");

                if (camera != null)
                {
                    Ray ray = camera.CastRay(Matrix.Identity);

                    // Get the point that the ray intersects the terrain if it does
                    intersectionPoint = IntersectsAtPoint(ray);

                    // Scale accordingly
                    intersectionPoint *= Scale;

                    if (intersectionPoint != null)
                        Console.WriteLine("INTERSECTION POINT: " + intersectionPoint.ToString());
                }
            }

            // Return the picked position
            return intersectionPoint;
        }

        /// <summary>
        /// Returns the point that a ray intersects a model at
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public Vector3? IntersectsAtPoint(Ray ray)
        {
            Vector3? intersectionPoint = null;

            // First check that the ray intersects the model bounds
            if (Intersects(ray) != null)
            {
                // Cast the ray and check against the terrain
                intersectionPoint = m_TerrainInfo.Pick(ray, Position, m_WorldMatrix);
            }

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
        #endregion

        #region Property Set / Gets
        public TerrainInfo TerrainInfo { get { return m_TerrainInfo; } }

        public override Vector3 Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;

                if (m_TerrainInfo != null)
                    m_TerrainInfo.Position = new Vector3(m_TerrainInfo.Position.X, Position.Y, m_TerrainInfo.Position.Z);
            }
        }

        // Average scale of the terrain
        public float AverageScale
        {
            get
            {
                return ((Scale.X + Scale.Y + Scale.Z) / 3);
            }
        }
        #endregion
    }
}
