using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/* SkinnedModel Structure */
using SkinnedModel;


using Psynergy;
using Psynergy.Camera;
using Psynergy.Input;

namespace Psynergy.Graphics
{
    public class ModelNode : Node3D, IRegister<ModelNode>
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterString("file", "ModelName");
            
            base.ClassProperties(factory);
        }
        #endregion

        // Custome model class variables
        protected Model m_Model = null;
        protected String m_ModelName = "";
        protected Matrix[] m_Transforms;
        protected BoundingSphere m_BoundingSphere;
        protected BoundingBox m_BoundingBox;

        protected bool m_DrawSphereMesh = true;

        #region Skinned values
        // Skinning data to be saved
        protected SkinningData m_SkinningData = null;
        protected Matrix[] m_BoneTransforms;

        // Animation variables
        private float m_AnimationPlaySpeed = 1.0f;

        #endregion

        #region Deferred Rendering Mesh Handling
        protected Mesh m_Mesh = null;
        #endregion

        public ModelNode() : base("")
        {
        }

        public ModelNode(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            base.Initialise();

            if (RenderGroup == null)
                RenderGroup = new RenderGroup("default");

            m_BoundingBox = new BoundingBox();
        }

        public override void Reset()
        {
            base.Reset();

            if ( AnimationPlayer != null )
            {
                // Reset animation player
                AnimationPlayer.Reset();
            }
        }

        protected override void LoadSpecific()
        {
            base.LoadSpecific();

            // Load model
            if ((m_ModelName != null) && (m_ModelName != ""))
            {
                m_Model = RenderManager.Instance.LoadModel(m_ModelName);

                if (m_Model != null)
                {
                    m_Transforms = new Matrix[m_Model.Bones.Count];
                    m_Model.CopyAbsoluteBoneTransformsTo(m_Transforms);

                    // Build the boundin box for the model
                    BuildBoundingBox();

                    // For now build bounding sphere too ( enumerate later )
                    BuildBoundingSphere();

                    // Load any textures
                    LoadTextures();

                    // Load texture that want to be generated after mesh tags are created
                    LoadPostTagTextures();

                    // Create Mesh
                    m_Mesh = CreateMesh();

                    // Mesh assignment
                    m_Mesh.Model = m_Model;
                    m_Mesh.Transform = transform.WorldMatrix;

                    // If it is a skinned model, load it as a skinned model properties
                    if (RenderGroup.IsSkinned)
                    {
                        SkinnedMesh skinnedMesh = (m_Mesh as SkinnedMesh);

                        // Create the animation player
                        AnimationPlayer = new SkinnedAnimationPlayer(skinnedMesh.SkinningData);
                    }
                }
            }

            // Defer the renderable to the renderer
            RenderManager.Instance.ActiveRenderer.DeferRenderable(this);
        }

        protected virtual Mesh CreateMesh()
        {
            Mesh toRet = null;

            if (!RenderGroup.IsSkinned)
            {
                // Create Mesh
                toRet = new Mesh();
            }
            else
            {
                // Create Mesh
                toRet = new SkinnedMesh();
            }

            return toRet;
        }

        protected void BuildBoundingSphere()
        {
            BoundingSphere sphere = new BoundingSphere(Vector3.Zero, 0);

            // TODO: Sort out bounding sphere creation later!
            if (m_Model != null)
            {
                // Merge all the models built in bounding spheres
                foreach (ModelMesh mesh in m_Model.Meshes)
                {
                    BoundingSphere transformed = mesh.BoundingSphere.Transform(m_Transforms[mesh.ParentBone.Index]);

                    sphere = BoundingSphere.CreateMerged(sphere, transformed);
                }
            }

            sphere = sphere.Transform(transform.WorldMatrix);
            m_BoundingSphere = sphere;
        }

        protected void BuildBoundingBox()
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            if (m_Model != null)
            {
                // For each mesh of the model
                foreach (ModelMesh mesh in Model.Meshes)
                {
                    foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    {
                        // Vertex buffer parameters
                        int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                        int vertexBufferSize = meshPart.NumVertices * vertexStride;

                        // Get vertex data as float
                        float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                        meshPart.VertexBuffer.GetData<float>(vertexData);

                        // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                        for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                        {
                            Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), m_Transforms[mesh.ParentBone.Index]);
                            transformedPosition = Vector3.Transform(transformedPosition, transform.WorldMatrix);

                            min = Vector3.Min(min, transformedPosition);
                            max = Vector3.Max(max, transformedPosition);
                        }
                    }
                }
            }

            // Create and return bounding box
            m_BoundingBox.Min = (min * transform.Scale);
            m_BoundingBox.Max = (max * transform.Scale);
        } 

        protected virtual void LoadPostTagTextures()
        {
        }

        public override void Update(GameTime deltaTime)
        {
            // Update base class where the main calculations occur.
            base.Update(deltaTime);

            // Update rotation before building the matrices
            Rotate(deltaTime);

            // Update movement before building the matrices
            Move(deltaTime);

            // Build the bounding box for the model
            BuildBoundingBox();

            // For now build bounding sphere too ( enumerate later )
            BuildBoundingSphere();

            if (RenderGroup != null)
            {
                // If skinned
                if (RenderGroup.IsSkinned)
                {
                    // Elapsed time with a variables which can be used to modify the animation play speed.
                    TimeSpan elapsedTime = TimeSpan.FromSeconds(deltaTime.ElapsedGameTime.TotalSeconds * m_AnimationPlaySpeed);

                    if (AnimationPlayer != null)
                    {
                        // Update the animation player
                        AnimationPlayer.Update(elapsedTime, transform.WorldMatrix);
                    }

                    SkinnedMesh skinnedMesh = (m_Mesh as SkinnedMesh);
                    skinnedMesh.BoneMatrices = AnimationPlayer.SkinTransforms;
                }
            }

            if ( m_Mesh != null )
            {
                m_Mesh.Active = ActiveRender;
                m_Mesh.Transform = transform.WorldMatrix;
            }
        }

        protected virtual void Rotate(GameTime deltaTime)
        {
        }

        protected virtual void Move(GameTime deltaTime)
        {
        }

        public virtual void ChangeTexture(String textureName)
        {
        }

        #region Animations
        public float PlayAnimation(String animName, bool loop)
        {
            float toRet = 0.0f;

            if (RenderGroup.IsSkinned)
            {
                if (AnimationPlayer != null)
                {
                    AnimationPlayer.StartClip(animName, loop);

                    // Get the animation duration if it exists
                    toRet = GetAnimationDuration();
                }
            }

            return toRet;
        }

        public float PlayAnimation(String animName, int startFrame, int endFrame, bool loop)
        {
            float toRet = 0.0f;

            if (RenderGroup.IsSkinned)
            {
                AnimationPlayer.StartClip(animName, startFrame, endFrame, loop);
            
                // Get the animation duration if it exists
                toRet = GetAnimationDuration();
            }

            return toRet;
        }

        public float PlayAnimation(String animName, TimeSpan startTime, TimeSpan endTime, bool loop)
        {
            float toRet = 0.0f;

            if (RenderGroup.IsSkinned)
            {
                AnimationPlayer.StartClip(animName, startTime, endTime, loop);
            
                // Get the animation duration if it exists
                toRet = GetAnimationDuration();
            }

            return toRet;
        }

        public void SetAnimationSpeed(float speed)
        {
            if (RenderGroup.IsSkinned)
            {
                // Make sure it doesn't go into minus numbers
                if (speed < 0)
                    speed = 0;

                m_AnimationPlaySpeed = speed;
            }
        }

        public float GetAnimationDuration()
        {
            float toRet = 0.0f;

            if ( AnimationPlayer.CurrentClip != null )
                toRet = (float)AnimationPlayer.CurrentClip.Duration.TotalSeconds;

            return toRet;
        }

        public bool IsAnimationFinished()
        {
            return AnimationPlayer.Done;
        }
        #endregion

        #region Scene Control
        public override void AddToScene(Scene scene)
        {
            base.AddToScene(scene);
        }

        public override void RemoveFromScene()
        {
            base.RemoveFromScene();
        }
        #endregion

        #region Intersections
        public float? Intersects(Ray ray)
        {
            float? intersection = null;

            // The input ray is in world space, but our model data is stored in object
            // space. We would normally have to transform all the model data by the
            // modelTransform matrix, moving it into world space before we test it
            // against the ray. That transform can be slow if there are a lot of
            // triangles in the model, however, so instead we do the opposite.
            // Transforming our ray by the inverse modelTransform moves it into object
            // space, where we can test it directly against our model data. Since there
            // is only one ray but typically many triangles, doing things this way
            // around can be much faster.

            // Inverse model transform
           // Matrix inverseTransform = Matrix.Invert(m_WorldMatrix);

            // Transform the ray into object space
           // ray.Position = Vector3.Transform(ray.Position, inverseTransform);
           // ray.Direction = Vector3.TransformNormal(ray.Direction, inverseTransform);

            if (m_BoundingSphere != null)
                intersection = m_BoundingSphere.Intersects(ray);

            // For now check against bounding boxes as well if sphere fails 
            // TODO: ( probably want bounding enumeration later )
            if (intersection == null)
            {
                if (m_BoundingBox != null)
                    intersection = m_BoundingBox.Intersects(ray);
            }

            return intersection;
        }
        #endregion

        #region Properties
        public Model Model { get { return m_Model; } }
        public String ModelName { get { return m_ModelName; } set { m_ModelName = value; } }

        public BoundingSphere BoundingSphere
        {
            get
            {
                return m_BoundingSphere;
            }
        }

        public SkinnedAnimationPlayer AnimationPlayer { get; private set; }

        #region Deferred Renderer Properties
        public Mesh Mesh
        {
            get { return m_Mesh; }
        }
        #endregion
        #endregion
    }
}
