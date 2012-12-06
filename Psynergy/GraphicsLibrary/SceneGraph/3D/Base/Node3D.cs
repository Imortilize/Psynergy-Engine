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
            factory.RegisterVector3("rotspeed", "RotationSpeeds");
            
            base.ClassProperties(factory);
        }
        #endregion

        #region custom containers
        // Vertex and index storage
        protected VertexPositionColor[] m_Vertices;
        protected VertexPositionNormalTexture[] m_TexturedVertices;
        protected int[] m_Indices;

        protected VertexBuffer m_VertexBuffer;
        protected IndexBuffer m_IndexBuffer;
        #endregion

        public Node3D() : base("")
        {
        }

        public Node3D(String name) : base(name)
        {

        }

        public override void Initialise()
        {
            // Create the pivotnode for children to use
            String name = Name;

            if (Name == "")
                name = "Default";

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
            if (Focused)
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

            // Positions desired to be at
            if (Parent != null)
                transform.Position += Parent.transform.Position;

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

        // Here temporarily atm
        public virtual bool SetNextPosition(GameTime deltaTime)
        {
            bool toRet = false;

            if (Controller != null)
                toRet = Controller.SetNextPosition(deltaTime);

            return toRet;
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

        public virtual IFocusable3D GetFocus()
        {
            return this;
        }

        #region Properties
        public Matrix GetParentWorldMatrix()
        {
            Matrix toRet = Matrix.Identity;

            if (Parent != null)
                toRet = Parent.transform.WorldMatrix;

            return toRet;
        }

        public Matrix GetParentLocalWorldMatrix()
        {
            Matrix toRet = Matrix.Identity;

            if (Parent != null)
                toRet = Parent.transform.LocalMatrix;

            return toRet;
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
        #endregion
    }
}
