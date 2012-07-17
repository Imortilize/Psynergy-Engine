using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;

/* JigLibX libraries */
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Utils;

namespace Psynergy.Physics
{
    public class PhysicsController : JigLibX.Physics.Controller
    {
        protected Body m_Body;
        protected Vector3 m_ConstantForces = new Vector3();
        protected Vector3 m_Force = new Vector3();
        protected Vector3 m_Torque = new Vector3();
        protected Vector3 m_Impulse = new Vector3();
        protected Vector3 m_AddedMassForce = new Vector3();

        public PhysicsController()
        {
        }

        public virtual void Initialize(Body body)
        {
            EnableController();
            m_Body = body;

            if (m_Body != null)
            {
                // Update downwards force ( mass based )
                //m_ConstantForces = (Vector3.Down * m_Body.Mass * 100);
            }
        }

        public virtual void Reset()
        {
            m_ConstantForces = Vector3.Zero;
            m_Force = Vector3.Zero;
            m_Torque = Vector3.Zero;
            m_Impulse = Vector3.Zero;
        }

        public override void UpdateController(float dt)
        {
            Debug.Assert(m_Body != null, "Physics body should not be null!");

            if (m_Body == null)
                return;

            // Update constant forces ( extra mass falling and friction )
            UpdateConstantForces(dt);

            // Constant update world force 
            UpdateWorldForce(dt);

            // Constant update world torque
            UpdateWorldTorque(dt);

            // One shot world impulse
            UpdateWorldImpulse(dt);
        }

        private void UpdateConstantForces(float deltaTime)
        {
            if ((m_ConstantForces != null) && (m_ConstantForces != Vector3.Zero))
            {
                // Update downwards force ( mass based )
                //m_ConstantForces = (Vector3.Down * m_Body.Mass * 100);

                // Update friction ( mass based )
               // m_ConstantForces += (m_Body.Orientation.Backward * m_Body.Mass * 100);

                // Apply this force in a world way
                m_Body.AddWorldForce(m_ConstantForces);

                // Check body is active
                if (!m_Body.IsActive)
                    m_Body.SetActive();
            }
        }

        private void UpdateWorldForce( float deltaTime )
        {
            if ((m_Force != null) && (m_Force != Vector3.Zero))
            {
                // Update downwards force ( mass based )
                //m_Force = (Vector3.Down * m_Body.Mass * 100);

                // Update friction ( mass based )
                //m_Force += (m_Body.Orientation.Backward * m_Body.Mass * 100);

                // Apply this force in a world way
                m_Body.AddWorldForce(m_Force);

                // Check body is active
                if (!m_Body.IsActive)
                    m_Body.SetActive();
            }
        }

        private void UpdateWorldTorque(float deltaTime)
        {
            if ((m_Torque != null) && (m_Torque != Vector3.Zero))
            {
                m_Body.AddWorldTorque(m_Torque);

                if (!m_Body.IsActive)
                    m_Body.SetActive();
            }
        }

        private void UpdateWorldImpulse(float deltaTime)
        {
            if ((m_Impulse != null) && (m_Impulse != Vector3.Zero))
            {
                // Apply a force impulse
                m_Body.ApplyWorldImpulse(m_Impulse);

                // Check body is active
                if (!m_Body.IsActive)
                    m_Body.SetActive();

                // Reset impulse
                m_Impulse = Vector3.Zero;
            }
        }

        public void ApplyForce(Vector3 force)
        {
            m_Force += force;
        }

        public void ApplyTorque(Vector3 torque)
        {
            m_Torque += torque;
        }

        public void ApplyImpulse(Vector3 impulse)
        {
            m_Impulse = impulse;
        }

        public void SetForce(Vector3 force)
        {
            m_Force = force;
        }

        public void SetTorque(Vector3 torque)
        {
            m_Torque = torque;
        }

        public void StopForces()
        {
            m_Force = Vector3.Zero;
        }

        public void StopTorque()
        {
            m_Torque = Vector3.Zero;
        }

        public void Stop()
        {
            StopForces();
            StopTorque();
        }
    }
}
