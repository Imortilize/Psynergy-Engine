using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* JigLibX libraries */
using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;

/* Main Library */
using Psynergy;

/* Graphics library */
using Psynergy.Graphics;

/* Physics library */
using Psynergy.Physics;

namespace XnaGame
{
    class TableModelJibLibX : TableModel, JibLibXActor
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            base.ClassProperties(factory);
        }
        #endregion

        private Body m_Body = null;
        private CollisionSkin m_Skin = null;
        private Vector3 m_Mass = Vector3.Zero;

        public TableModelJibLibX() : base("")
        {
        }

        public TableModelJibLibX(String name) : base(name)
        {
        }

        protected override void LoadSpecific()
        {
            base.LoadSpecific();

            // Create new bodey and collision skin
            m_Body = new Body();
            m_Skin = new CollisionSkin(m_Body);

            if (m_Body != null)
            {
                // Set skin to the body
                m_Body.CollisionSkin = m_Skin;

                // Check the skin was successfully created and add this 
                // custom dice as a primitive to the collision skin
                if (m_Skin != null)
                {
                    Vector3 sideLengths = (m_BoundingBox.Max - m_BoundingBox.Min);

                    if (sideLengths.Y <= 0)
                        sideLengths.Y = 1;

                    Box box = new Box(Vector3.Zero, Matrix.Identity, sideLengths);
                    m_Skin.AddPrimitive(box, (int)MaterialTable.MaterialID.BouncyNormal);

                    // Set mass
                    m_Mass = SetMass(1.0f);

                    // Move the body to correct position initially
                    m_Body.MoveTo((Position + new Vector3(0, (sideLengths.Y * 0.5f), 0)), Matrix.Identity);

                    // Apply transform to skin
                    m_Skin.ApplyLocalTransform(new Transform(-m_Mass, Matrix.Identity));

                    // Enable it 
                    Enable();

                    // Set to immovable
                    m_Body.Immovable = true;
                }
            }
        }

        private Vector3 SetMass(float mass)
        {
            PrimitiveProperties primitiveProperties = new PrimitiveProperties(
                PrimitiveProperties.MassDistributionEnum.Solid,
                PrimitiveProperties.MassTypeEnum.Mass, mass);

            float junk = 0.0f;
            Vector3 com = Vector3.Zero;
            Matrix it = Matrix.Identity;
            Matrix itCom = Matrix.Identity;

            if (m_Skin != null)
                m_Skin.GetMassProperties(primitiveProperties, out junk, out com, out it, out itCom);

            if (m_Body != null)
            {
                m_Body.BodyInertia = itCom;
                m_Body.Mass = junk;
            }

            return com;
        }

        public override void Enable()
        {
            base.Enable();

            if (m_Body != null)
                m_Body.EnableBody();

            // Add to Scene ( if one exists )
            AddToScene(SceneManager.Instance.CurrentScene);
        }

        public override void Disable()
        {
            base.Disable();

            if (m_Body != null)
                m_Body.DisableBody();

            // Remove from Scene
            RemoveFromScene();
        }

        #region Property Set / Gets
        public Body Body { get { return m_Body; } }
        public CollisionSkin Skin { get { return m_Skin; } }
        #endregion
    }
}
