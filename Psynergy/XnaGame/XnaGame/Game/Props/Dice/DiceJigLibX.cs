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

/* Main library */
using Psynergy;

/* Physics library */
using Psynergy.Physics;

/* Graphics Library */
using Psynergy.Graphics;

namespace XnaGame
{
    public class DiceJigLibX : Dice, JibLibXActor
    {
        private Body m_Body = null;
        private CollisionSkin m_Skin = null;
        private Vector3 m_Mass = Vector3.Zero;

        // Physics controller ( for forces )
        PhysicsController m_PhysicsController = new DiceController();

        public DiceJigLibX() : base()
        {
        }

        public DiceJigLibX(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            base.Initialise();

            // Create new bodey and collision skin
            m_Body = new Body();
            m_Skin = new CollisionSkin(m_Body);

            // Set physics controller
            m_PhysicsController.Initialize(m_Body);
        }

        public override void Reset()
        {
            base.Reset();

            if (m_PhysicsController != null)
                m_PhysicsController.Reset();
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

        protected override void LoadSpecific()
        {
            base.LoadSpecific();

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
                        sideLengths.Y = 0.1f;

                    Box box = new Box(Vector3.Zero, Matrix.Identity, sideLengths);
                    m_Skin.AddPrimitive(box, (int)MaterialTable.MaterialID.BouncyRough);

                    // Set mass
                    m_Mass = SetMass(7.0f);

                    // Move the body to correct position initially
                    m_Body.MoveTo(transform.Position, Matrix.Identity);

                    // Apply transform to skin
                    m_Skin.ApplyLocalTransform(new JigLibX.Math.Transform(-m_Mass, Matrix.Identity));

                    // make sure it isn't in the scene and is disabled
                    Disable();
                }
            }
        }

        public override void Update(GameTime deltaTime)
        {
            // Update physics controller if it exists
            if (m_PhysicsController != null)
                m_PhysicsController.UpdateController((float)deltaTime.ElapsedGameTime.TotalSeconds);

            // Update model position and orientation using the specified
            // body and skin
            if ((m_Body != null) && ( m_Skin != null ))
            {
                Vector3 newPos = m_Body.Position;
                Quaternion newRot = transform.Rotation;

                Primitive primitive = m_Skin.GetPrimitiveLocal(0);

                if (primitive != null)
                    newRot = Quaternion.CreateFromRotationMatrix(primitive.Transform.Orientation * m_Body.Orientation);

                // Set position and rotation accordingly
                transform.Position = newPos;
                transform.Rotation = newRot;
            }

            base.Update(deltaTime);  
        }

        protected override void UpdatedDisabled(GameTime deltaTime)
        {
            base.UpdatedDisabled(deltaTime);
        }

        public override void Enable()
        {
            base.Enable();

            if (m_Body != null)
            {
                m_Body.EnableBody();

                // Set intial body position
                m_Body.Position = transform.Position;
            }

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

            // Set not to render
            ActiveRender = false;
        }

        public bool IsStationary()
        {
            bool toRet = false;

            if (m_Body != null)
            {
                if (m_Body.CollisionSkin.Collisions.Count > 0)
                {
                    Vector3 posDiff = (m_Body.OldPosition - m_Body.Position);
                    float diff = posDiff.Length();

                    if (diff <= 0.0015f)
                        toRet = true;
                }
            }

            return toRet;
        }

        public void SetForce(Vector3 force)
        {
            if (m_PhysicsController != null)
                m_PhysicsController.SetForce(force);
        }

        public void ApplyImpulse( Vector3 impulse)
        {
            if (m_PhysicsController != null)
                m_PhysicsController.ApplyImpulse(impulse);
        }

        #region Property Set / Gets
        public Body Body { get { return m_Body; } }
        public CollisionSkin Skin { get { return m_Skin; } }
        #endregion
    }
}
