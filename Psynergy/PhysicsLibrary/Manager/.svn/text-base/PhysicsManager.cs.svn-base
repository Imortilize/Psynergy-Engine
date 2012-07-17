using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

// JigLibX physics library components

namespace Psynergy.Physics
{
    public enum PhysicsEngineType
    {
        eNone = 0,
        eJibLibX = 1
    };

    public class PhysicsManager : Singleton<PhysicsManager>
    {
        // Private implementations of physics engines
        private PhysicsEngineType m_Engine = PhysicsEngineType.eNone;
        private PhysicsEngine m_PhysicsEngine = null;

        public PhysicsManager() 
        {
        }

        /* Used to select which physics engine to use */
        public void SelectEngine(PhysicsEngineType engine)
        {
            m_Engine = engine;
        }

        public override void Initialise()
        {
            base.Initialise();

            // Use the engine enum to determine which physics 
            // engine to use if any
            if (m_Engine == PhysicsEngineType.eJibLibX)
                m_PhysicsEngine = new JigLibXPhysicsEngine();

            // If a physics engine was created, initialise it
            if (m_PhysicsEngine != null)
                m_PhysicsEngine.Initialise();
        }

        public override void Load()
        {
            base.Load();
        }

        public override void UnLoad()
        {
            base.UnLoad();
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            // If a physics engine exists, update it
            if (m_PhysicsEngine != null)
                m_PhysicsEngine.Update(deltaTime);
        }

        public void SetGravity(Vector3 gravity)
        {
            if (m_PhysicsEngine != null)
                m_PhysicsEngine.SetGravity(gravity);
        }
    }
}
