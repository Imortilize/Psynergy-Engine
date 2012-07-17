using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* JigLibX libraries */
using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;

/* Physics library */
using Psynergy.Physics;

namespace Psynergy.Graphics
{
    class CustomDiceJibLibX: CustomDice, JibLibXActor
    {
        private Body m_Body = null;
        private CollisionSkin m_Skin = null;
        private Vector3 m_Mass = Vector3.Zero;

        public CustomDiceJibLibX() : base()
        {
        }

        public CustomDiceJibLibX(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            base.Initialise();

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
                    Box box = new Box(Vector3.Zero, Matrix.Identity, Scale);
                    m_Skin.AddPrimitive(box, (int)MaterialTable.MaterialID.BouncyNormal);

                    // Set mass
                    m_Mass = SetMass(1.0f);

                    // Move the body to correct position initially
                    m_Body.MoveTo(Position, Matrix.Identity);

                    // Apply transform to skin
                    m_Skin.ApplyLocalTransform(new Transform(-m_Mass, Matrix.Identity));

                    // Enable body
                    EnableBody();
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

        public override void Load()
        {
            base.Load();
        }

        protected override void LoadSpecific()
        {
            base.LoadSpecific();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);
        }

        public void EnableBody()
        {
            if (m_Body != null)
                m_Body.EnableBody();
        }

        public void DisableBody()
        {
            if (m_Body != null)
                m_Body.DisableBody();
        }

        #region Property Set / Gets
        public Body Body { get { return m_Body; } }
        public CollisionSkin Skin { get { return m_Skin; } }
        #endregion
    }
}
