using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

/* Main Library */
using Psynergy;

/* Camera Library */
using Psynergy.Camera;

namespace Psynergy.Graphics
{
    public class Node3D : RenderNode, IFocusable3D
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterQuat("rot", "Rotation");
            factory.RegisterVector3("rotdegrees", "RotationDegrees");
            factory.RegisterVector3("rotspeed", "RotationSpeeds");
            factory.RegisterQuat("orbitalrot", "OrbitalRotation");
            factory.RegisterVector3("orbitalrotdegrees", "OrbitalRotationDegrees");
            factory.RegisterVector3("orbitalrotspeed", "OrbitalRotationSpeeds");
            factory.RegisterBool("setfocus", "Focused");            
       
            base.ClassProperties(factory);
        }
        #endregion

        #region Movement variables
        protected Matrix m_WorldMatrix = Matrix.Identity;
        protected Matrix m_LocalWorldMatrix = Matrix.Identity;
        protected Quaternion m_Rotation = Quaternion.Identity;
        protected Quaternion m_StartRotation = Quaternion.Identity;
  
        protected Quaternion m_OrbitalRotation = Quaternion.Identity;
        protected Quaternion m_OrbitalStartRotation = Quaternion.Identity;

        protected float m_OrbitalRoll = 0.0f;
        protected float m_OrbitalRollSpeed = 0.0f;
        protected float m_OrbitalStartRoll = 0.0f;
        protected float m_OrbitalStartRollSpeed = 0.0f;

        protected float m_OrbitalYaw = 0.0f;
        protected float m_OrbitalYawSpeed = 0.0f;
        protected float m_OrbitalStartYaw = 0.0f;
        protected float m_OrbitalStartYawSpeed = 0.0f;

        protected float m_OrbitalPitch = 0.0f;
        protected float m_OrbitalPitchSpeed = 0.0f;
        protected float m_OrbitalStartPitch = 0.0f;
        protected float m_OrbitalStartPitchSpeed = 0.0f;

        #endregion

        // Pivot node
        private PivotNode m_PivotNode = null;

        #region custom containers
        // Vertex and index storage
        protected VertexPositionColor[] m_Vertices;
        protected VertexPositionNormalTexture[] m_TexturedVertices;
        protected int[] m_Indices;

        protected VertexBuffer m_VertexBuffer;
        protected IndexBuffer m_IndexBuffer;
        #endregion

        #region Lighting
        #endregion

        #region world matrix variables
        protected bool m_UseParentScale = true;
        #endregion

        public Node3D() : base("")
        {
        }

        public Node3D(String name) : base(name)
        {

        }

        public override void Initialise()
        {
            // Set the start rotation
            m_StartRotation = Rotation;

            // Set the start orbital rotation
            m_OrbitalStartRotation = OrbitalRotation;

            // Set the start orbital rotation builder values
            m_OrbitalStartRoll = OrbitalRoll;
            m_OrbitalStartRollSpeed = OrbitalRollSpeed;
            m_OrbitalStartYaw = OrbitalYaw;
            m_OrbitalStartYawSpeed = OrbitalYawSpeed;
            m_OrbitalStartPitch = OrbitalPitch;
            m_OrbitalStartPitchSpeed = OrbitalPitchSpeed;

            // Create the pivotnode for children to use
            String name = Name;

            if (Name == "")
                name = "Default";

            m_PivotNode = new PivotNode(name + "-PivotNode");

            // Set the pivot node start position and rotation
            m_PivotNode.Position = m_Position;
            m_PivotNode.Rotation = Rotation;

            // Initialise the base class
            base.Initialise();
        }

        public override void Load()
        {
            base.Load();

            // Load 3d specific items
            LoadSpecific();
        }

        protected virtual void LoadSpecific()
        {
            // If it is to be focused by the camera, then focus it.
            if (m_CameraFocus)
            {
                BaseCamera activeCamera = RenderManager.Instance.GetActiveCamera();

                if (activeCamera != null)
                {
                    CameraFocusEvent cameraFocusEvent = new CameraFocusEvent(this);
                    cameraFocusEvent.Fire();
                }
            }
        }

        public override void Reset()
        {
            base.Reset();

            Rotation = m_StartRotation;

            OrbitalRotation = m_OrbitalStartRotation;
            OrbitalRoll = m_OrbitalStartRoll;
            OrbitalRollSpeed = m_OrbitalStartRollSpeed;
            OrbitalYaw = m_OrbitalStartYaw;
            OrbitalYawSpeed = m_OrbitalStartYawSpeed;
            OrbitalPitch = m_OrbitalStartPitch;
            OrbitalPitchSpeed = m_OrbitalStartPitchSpeed;

            // Positions desired to be at
            if (Parent != null)
                m_Position += Parent.Position;

            // Initialise world matrix translation
            m_WorldMatrix.Translation = m_Position;

            // Check if the camera focus should be set to this object or not
            if (Focused)
            {
                BaseCamera activeCamera = RenderManager.Instance.GetActiveCamera();

                if (activeCamera != null)
                {
                    if (activeCamera.RequiresFocus)
                        activeCamera.SetInstantFocus(this);
                }
            }
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            // Build world matrix appropiately
            BuildWorldMatrix(deltaTime);
        }

        public virtual bool SetNextPosition(GameTime deltaTime)
        {
            bool toRet = false;

            if ( m_Controller != null )
                toRet = Controller.SetNextPosition(deltaTime);

            return toRet;
        }

        private void BuildWorldMatrix(GameTime deltaTime)
        {
            // orbital rotation
            BuildOrbitalRotation();

            // Position 
            Vector3 positionToUse = m_Position;

            if (Parent != null)
                positionToUse -= Parent.Position;

            // Build the world matrix and the local world matrix 
            // The world matrix uses the pivot node and is what children nodes use to determine world rotations according to parents
            // The local matrix is what this node specifically uses and uses its own rotations
            m_LocalWorldMatrix = (Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(positionToUse) * Matrix.CreateFromQuaternion(OrbitalRotation));
            if (Parent != null)
            {
                Vector3 parentScale = Vector3.One;

                if (m_UseParentScale)
                    parentScale = Parent.Scale;

                // Multiply the world matrix by it's parent's world matrix which brings it to its parents world space
                m_WorldMatrix = (Matrix.CreateScale(parentScale * Scale) * Matrix.CreateWorld((Parent.Position + m_LocalWorldMatrix.Translation), m_LocalWorldMatrix.Forward, m_LocalWorldMatrix.Up));

            }
            else
            {
                m_WorldMatrix = m_LocalWorldMatrix;
            }
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);
        }

        public int GetVertexCount()
        {
            if (!m_Textured)
                return m_Vertices.Length;
            else
                return m_TexturedVertices.Length;
        }

        public int GetPrimitiveCount()
        {
            if (m_Indices.Length > 0)
                return (m_Indices.Length / 3);
            else
                return 0;
        }

        private void BuildOrbitalRotation()
        {
            m_OrbitalRotation = Quaternion.CreateFromYawPitchRoll(m_OrbitalYaw, m_OrbitalPitch, m_OrbitalRoll);
        }

        public void SetPosition(Vector3 position)
        {
            if ( Controller != null )
                Controller.SetPosition(position);
        }

        public void SetDesiredPosition(Vector3 desiredPos)
        {
            if (Controller != null)
                Controller.SetDesiredPosition(desiredPos);
        }

        public void StopMovement()
        {
            if (Controller != null)
                Controller.StopMovement();
        }

        public void SetDesiredRotation(Vector3 from, Vector3 to)
        {
            if (Controller != null)
                Controller.SetDesiredRotation(from, to);
        }

     /*   public void ModifyDesiredRotation(Matrix modification)
        {
            if (Controller != null)
                Controller.ModifyDesiredRotation(modification);
        }*/

        public void SetTerrain(TerrainNode terrainNode, float terrainOffset )
        {
            Debug.Assert(terrainNode != null, "Terrain node cannot be assigned if null");

            if (terrainNode != null)
            {
                Debug.Assert(m_Controller != null, "Controller is null so terrain cannot be added");

                if (m_Controller != null)
                {
                    Node3DController controller = (m_Controller as Node3DController);
                    controller.TerrainReference = terrainNode;
                    controller.TerrainHeightOffset = terrainOffset;
                }
            }
        }

        public virtual IFocusable3D GetFocus()
        {
            return this;
        }

        #region Properties
        public PivotNode GetParentPivotNode()
        {
            PivotNode toRet = null;

            if (m_ParentNode != null)
            {
                if (m_ParentNode.InheritsFrom(typeof(Node3D)))
                    toRet = (m_ParentNode as Node3D).Pivot;
            }

            return toRet;
        }

        public Matrix GetParentWorldMatrix()
        {
            Matrix toRet = Matrix.Identity;

            if (m_ParentNode != null)
            {
                if (m_ParentNode.InheritsFrom(typeof(Node3D)))
                   toRet = (m_ParentNode as Node3D).WorldMatrix;
            }

            return toRet;
        }

        public Matrix GetParentLocalWorldMatrix()
        {
            Matrix toRet = Matrix.Identity;

            if (m_ParentNode != null)
            {
                if (m_ParentNode.InheritsFrom(typeof(Node3D)))
                    toRet = (m_ParentNode as Node3D).LocalWorldMatrix;
            }

            return toRet;
        }

        // World and Local matrices
        public Matrix WorldMatrix { get { return m_WorldMatrix; } set { m_WorldMatrix = value; } }
        public Matrix LocalWorldMatrix { get { return m_LocalWorldMatrix; } set { m_LocalWorldMatrix = value; } }

        // Pivot Node which is where any children nodes will be transformed around
        public PivotNode Pivot { get { return m_PivotNode; } set { m_PivotNode = value; } }

        // Node rotations
        public Quaternion Rotation { get { return m_Rotation; } set { m_Rotation = value; } }
        public Quaternion StartRotation { get { return m_StartRotation; } set { m_StartRotation = value; } }
        public Vector3 RotationDegrees
        {
            set
            {
                // Build the rotation from these values
                m_Rotation = Quaternion.CreateFromYawPitchRoll(value.Y, value.X, value.Z);
            }
        }

        public Quaternion OrbitalRotation { get { return m_OrbitalRotation; } set { m_OrbitalRotation = value; } }
        public Vector3 OrbitalRotationDegrees
        {
            set
            {
                m_OrbitalPitch = value.X;
                m_OrbitalYaw = value.Y;
                m_OrbitalRoll = value.Z;

                // Build the orbital rotation from these values
                BuildOrbitalRotation();
            }
        }

        public float OrbitalRoll { get { return m_OrbitalRoll; } set { m_OrbitalRoll = value; } }
        public float OrbitalRollSpeed { get { return m_OrbitalRollSpeed; } set { m_OrbitalRollSpeed = value; } }

        public float OrbitalYaw { get { return m_OrbitalYaw; } set { m_OrbitalYaw = value; } }
        public float OrbitalYawSpeed { get { return m_OrbitalYawSpeed; } set { m_OrbitalYawSpeed = value; } }

        public float OrbitalPitch { get { return m_OrbitalPitch; } set { m_OrbitalPitch = value; } }
        public float OrbitalPitchSpeed { get { return m_OrbitalPitchSpeed; } set { m_OrbitalPitchSpeed = value; } }

        public Vector3 OrbitalRotationSpeeds
        {
            set
            {
                m_OrbitalPitchSpeed = value.X;
                m_OrbitalYawSpeed = value.Y;
                m_OrbitalRollSpeed = value.Z;
            }
        }

        public Texture GetTexture(int index)
        {
            Texture toRet = null;

            if ((m_Textures != null) && (m_Textures.Count > 0))
            {
                if (index < m_Textures.Count)
                    toRet = m_Textures[index];
            }

            return toRet;
        }

        public virtual void OnSelect()
        {
        }

        //public Node3DController Controller { get { return (m_Controller as Node3DController); } set { m_Controller = value; } }
        #endregion
    }
}
