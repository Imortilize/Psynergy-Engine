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
    public class Controller : AbstractController<Node>
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterFloat("movementspeed", "MovementSpeed");
            factory.RegisterFloat("maxvelocity", "MaxVelocity");
            factory.RegisterFloat("terrainheightoffset", "TerrainHeightOffset");

            base.ClassProperties(factory);
        }
        #endregion

        protected Node m_ObjReference;

        // Movement variables
        protected bool m_Movement = false;
        protected float m_MovementSpeed = 0.0f;

        public Controller() : base()
        {
            // Create the vision node
            //m_Vision = new VisionNode((m_ObjReference as Node3D));
        }

        public Controller(Node objReference) //: base(objReference)
        {
            Debug.Assert(objReference != null, "Controller object cannot be null!");

            m_ObjReference = objReference;

            // Create the vision node
           // m_Vision = new VisionNode((m_ObjReference as Node3D));
        }

        public override void Initialise()
        {
            base.Initialise();

            // Register node events
            RegisterEvents();

            // Reset the controller
            Reset();
        }

        protected virtual void RegisterEvents()
        {
        }

        public override void Reset()
        {
            base.Reset();

            m_Movement = false;
        }

        public override void Load()
        {
        }

        public override void Update(GameTime deltaTime)
        {
            // Used for moving a model to a desired location
            UpdateMovement(deltaTime);

            // Update model rotation values
            UpdateRotation(deltaTime);
        }

        protected virtual void UpdateMovement(GameTime deltaTime)
        {
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

        #region Factory class registers
        public override void OnClassSet(GameObject invokeNode)
        {
            ObjectReference = (invokeNode as Node);
        }
        #endregion

        #region Property Set/Gets
        public Node ObjectReference { get { return m_ObjReference; } set { m_ObjReference = value; } }
        public bool Movement { get { return m_Movement; } set { m_Movement = value; } }
        public float MovementSpeed { get { return m_MovementSpeed; } set { m_MovementSpeed = value; } }
        #endregion
    }
}
