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
using JigLibX.Collision;

/* Physics library */
using Psynergy.Physics;

/* Sound library */
using Psynergy.Sound;

namespace XnaGame
{
    public class DiceController : PhysicsController
    {
        private float m_SpinTime = 0.5f;
        private float m_CurrentSpinTime = 0.0f;

        public DiceController() : base()
        {
        }

        public override void Initialize(Body body)
        {
            base.Initialize(body);
        }

        public override void Reset()
        {
            base.Reset();

            if (m_Body != null)
            {
                if (m_Body.CollisionSkin != null)
                    m_Body.CollisionSkin.Collisions.Clear();

                m_Body.ClearForces();
                m_Body.ClearVelChanged();
                m_Body.Velocity = Vector3.Zero;
                m_Body.AngularVelocity = Vector3.Zero;
            }

            m_CurrentSpinTime = m_SpinTime;
        }

        public override void UpdateController(float dt)
        {
            base.UpdateController(dt);

            if ( (m_Body != null) && (m_Body.IsBodyEnabled))
            {
                // First check whether to spin more or not
                if ((m_CurrentSpinTime -= dt) > 0.0f)
                   m_Body.ApplyBodyAngImpulse(new Vector3(0.5f, 0.5f, 0.5f) * 10);
 
                Vector3 oldVelocity = m_Body.OldVelocity;
                Vector3 newVelocity = m_Body.Velocity;
                Vector3 resultVel = (newVelocity - oldVelocity);
                float speed = resultVel.Length();

                if (speed > 2.0f)
                {
                    CollisionSkin skin = m_Body.CollisionSkin;

                    if (skin != null)
                    {
                        if (skin.Collisions.Count > 0)
                        {
                            // Play dice sound
                            SoundManager.Instance.PlaySound("diceroll");
                        }
                    }
                }
            }
        }
    }
}
