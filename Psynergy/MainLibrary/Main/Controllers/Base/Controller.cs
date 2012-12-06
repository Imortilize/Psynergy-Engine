using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy
{
    public class Controller : AbstractController
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterFloat("movementspeed", "MovementSpeed");
            factory.RegisterVector3("maxvelocity", "MaxVelocity");
            factory.RegisterFloat("terrainheightoffset", "TerrainHeightOffset");

            base.ClassProperties(factory);
        }
        #endregion

        protected GameObject m_ObjReference;

        // Movement variables
        protected bool m_Movement = false;
        protected float m_MovementSpeed = 0.0f;

        // Velocity of the object being controlled
        private bool m_VelocityDirty = false;
        private Vector3 m_Velocity = Vector3.Zero;
        private Vector3 m_MaxVelocity = Vector3.Zero;

        public Controller() : base()
        {
            // Create the vision node
            //m_Vision = new VisionNode((m_ObjReference as Node3D));
        }

        public Controller(GameObject objReference) //: base(objReference)
        {
            Debug.Assert(objReference != null, "Controller object cannot be null!");

            m_ObjReference = objReference;
        }

        public override void Initialise()
        {
            base.Initialise();

            // Reset the controller
            Reset();
        }

        public override void Reset()
        {
            base.Reset();

            // Set movement to false
            m_Movement = false;
            m_Velocity = Vector3.Zero;
            m_VelocityDirty = false;
        }

        public override void Load()
        {
        }

        public override void Update(GameTime deltaTime)
        {
            Debug.Assert(m_ObjReference != null, "Object reference is null on a controller!");

            // Object reference should not be null if this controller is being run
            if (m_ObjReference != null)
            {
                if (m_Movement)
                {
                    // Used for moving a model to a desired location
                    UpdateMovement(deltaTime, m_ObjReference.transform.Position);
                }

                // Update model rotation values
                UpdateRotation(deltaTime);
            }
        }

        protected virtual void UpdateMovement(GameTime deltaTime, Vector3 position)
        {
            // Clamp the velocity
            m_Velocity = Vector3.Clamp(m_Velocity, -m_MaxVelocity, m_MaxVelocity);

            // If the velocity is unchanged then slow it down over time
            if (!m_VelocityDirty && (m_ObjReference.Weight > 0))
            {
                float slowDownFactor = (1.0f - (m_ObjReference.Weight / 100));

                // Object weight will affect how fast the object slows down
                m_Velocity *= slowDownFactor;
            }

            // Update the position using the velocity
            m_ObjReference.transform.Position += m_Velocity;

            // Velocity is no longer dirty
            m_VelocityDirty = false;
        }

        protected virtual void UpdateRotation(GameTime deltaTime)
        {
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);
        }

        public virtual float GetMovementSpeed()
        {
            return m_MovementSpeed;
        }

        public virtual void SetDesiredPosition(Vector3 desiredPos)
        {
            // Movement is true
            m_Movement = true;
        }

        public virtual void SetDesiredRotation(Vector3 from, Vector3 to)
        {
            // .. Stub Function
        }

        public virtual Vector3 SetPosition(Vector3 position)
        {
            if (m_ObjReference != null)
                m_ObjReference.transform.Position = position;

            // Return the new position
            return position;
        }

        public virtual bool SetNextPosition(GameTime deltaTime)
        {
            return false;
        }

        public virtual void StopMovement()
        {
            m_Velocity = Vector3.Zero;
            m_Movement = false;
        }

        #region Factory class registers
        public override void OnClassSet(GameObject invokeNode)
        {
            ObjectReference = invokeNode;
        }
        #endregion

        #region Property Set/Gets
        public GameObject ObjectReference { get { return m_ObjReference; } set { m_ObjReference = value; } }
        public bool Movement { get { return m_Movement; } set { m_Movement = value; } }
        public float MovementSpeed { get { return m_MovementSpeed; } set { m_MovementSpeed = value; } }

        // Object velocity
        public Vector3 Velocity { get { return m_Velocity; } set { m_Velocity = value; m_VelocityDirty = true; } }
        public Vector3 MaxVelocity { get { return m_MaxVelocity; } set { m_MaxVelocity = value; } }
        #endregion
    }
}
