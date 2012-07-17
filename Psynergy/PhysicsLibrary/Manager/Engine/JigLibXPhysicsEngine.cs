using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* JigLibX specific files */
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Collision;

namespace Psynergy.Physics
{
    public class JigLibXPhysicsEngine : PhysicsEngine
    {
        private PhysicsSystem m_PhysicsSystem = null;

        public JigLibXPhysicsEngine() : base()
        {
        }

        public override void Initialise()
        {
            m_PhysicsSystem = new PhysicsSystem();

            // Add Collision system
            m_PhysicsSystem.CollisionSystem = new CollisionSystemSAP();
        }

        public override void Update(GameTime deltaTime)
        {
            if ( m_PhysicsSystem != null )
            {
                float timeStep = (float)deltaTime.ElapsedGameTime.TotalSeconds;
                m_PhysicsSystem.Integrate(timeStep);
            }
        }

        public override void SetGravity(Vector3 gravity)
        {
            if ( m_PhysicsSystem != null )
                m_PhysicsSystem.Gravity = gravity;
        }
    }
}
