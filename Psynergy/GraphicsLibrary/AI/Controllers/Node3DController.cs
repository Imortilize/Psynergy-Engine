using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

/* Main Library */
using Psynergy;

// Event Library
using Psynergy.Events;

// AI Library
using Psynergy.AI;

namespace Psynergy.Graphics
{
    public class Node3DController : Controller, IRegister<Node3DController>
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterClass(typeof(VisionNode), "vision", "Vision");

            base.ClassProperties(factory);
        }
        #endregion

        #region Movement Variables
        protected float m_Velocity = 0.0f;
        protected float m_MaxVelocity = 0.0f;
        protected float m_RotationVelocity = 4.0f;
        protected Vector3 m_PositionOnSet = Vector3.Zero;
        protected Vector3 m_DesiredPosition = Vector3.Zero;

        protected float m_NormalizedPoint = 0.0f;

        protected Quaternion m_RotationOnSet = Quaternion.Identity;
        protected Quaternion m_DesiredRotation = Quaternion.Identity;
        #endregion

        #region Vision
        protected bool m_RenderVision = true;
        protected VisionNode m_Vision = null;
        #endregion

        public Node3DController() : base()
        {
        }

        public Node3DController(Node3D node) : base(node)
        {
        }

        public override void Initialise()
        {
            base.Initialise();

            if (m_Vision != null)
            {
                // Initialise vision
                m_Vision.Initialise();
            }
        }

        public override void Reset()
        {
            base.Reset();

            Node3D node = (m_ObjReference as Node3D);

            m_PositionOnSet = m_DesiredPosition;
            m_NormalizedPoint = 0.0f;

            if (node != null)
            {
                // Reset movement variables
                m_DesiredPosition = node.transform.Position;
                m_DesiredRotation = node.transform.Rotation;
            }
            else
            {
                // Reset movement variables
                m_DesiredPosition = Vector3.Zero;
                m_DesiredRotation = Quaternion.Identity;
            }

            m_RotationOnSet = m_DesiredRotation;
            m_Velocity = 0.0f;

            // Reset the vision
            if (m_Vision != null)
                m_Vision.Reset();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            // Update vision
            UpdateVision(deltaTime);
        }

        protected override void UpdateMovement(GameTime deltaTime, Vector3 position)
        {
            base.UpdateMovement(deltaTime, position);

            // Object reference should be a Node3D or inherited Node3D object
            Node3D node = (m_ObjReference as Node3D);
            if (node != null)
            {
                if (m_Movement)
                {
                    Vector3 directionVec = (m_DesiredPosition - node.transform.Position);                    // Direction from current position to desired position
                    float distance = directionVec.Length();                                     // Distance from desired position
                    float originalDistance = (m_DesiredPosition - m_PositionOnSet).Length();    // Original distance when position was set
                    float currentDistance = (node.transform.Position - m_PositionOnSet).Length();            // Distance from the original point

                    if (currentDistance >= originalDistance)
                    {
                        node.transform.Position = m_DesiredPosition;

                        // Set the desired rotation to be what it currently is
                        SetDesiredRotation(node.transform.Position, node.transform.Position);
                    }
                    else
                    {
                        // If the node hasn't reached its destination yet
                        if (distance > 1.0f)
                        {
                            // Increase velocity 
                            m_Velocity += ((GetMovementSpeed() * (float)deltaTime.ElapsedGameTime.TotalSeconds));
                        }

                        // If there is any speed involved ( otherwise not moving )
                        if (m_Velocity > 0)
                        {
                            // m_Velocity -= (FRICTION * (float)deltaTime.ElapsedGameTime.TotalSeconds);
                            m_Velocity = MathHelper.Clamp(m_Velocity, 0.0f, m_MaxVelocity);

                            // Normalize the direction vector
                            directionVec.Normalize();

                            //float originalHeight = node.PosY;

                            // What the new position will be after this update
                            Vector3 newPos = node.transform.Position + (directionVec * (m_Velocity * (float)deltaTime.ElapsedGameTime.TotalSeconds));

                            // Set the desired rotation
                            SetDesiredRotation(node.transform.Position, newPos);

                            // Use the directionVec * velocity to move towards point accordingly.
                            node.transform.Position = newPos;
                        }
                    }

                    // Check whether to get a next position or not
                    if ((originalDistance - currentDistance) <= 1)
                        SetNextPosition(deltaTime);
                }
            }
        }

        public override bool SetNextPosition(GameTime deltaTime)
        {
            bool toRet = false;

            if (m_ObjReference != null)
            {
                if (m_ObjReference.InheritsFrom<Node3D>())
                {
                    Node3D node = (m_ObjReference as Node3D);
                    toRet = node.SetNextPosition(deltaTime);
                }
            }

            return toRet;
        }

        protected override void UpdateRotation(GameTime deltaTime)
        {
            base.UpdateRotation(deltaTime);

            if (m_ObjReference != null)
            {
                Node3D node = (m_ObjReference as Node3D);

                node.transform.Rotation = Quaternion.Slerp(node.transform.Rotation, m_DesiredRotation, (m_RotationVelocity * (float)deltaTime.ElapsedGameTime.TotalSeconds));
            }
        }

        protected virtual void UpdateVision(GameTime deltaTime)
        {
            if (m_Vision != null)
                m_Vision.Update(deltaTime);
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);

            // Check to whether to render this nodes vision or not
            /*if (m_RenderVision)
            {
                if (m_Vision != null)
                    m_Vision.Render(deltaTime, (m_ObjReference as Node3D));
            }*/
        }

        public override Vector3 SetPosition(Vector3 position)
        {
            m_DesiredPosition = position;

            if ( m_ObjReference != null )
                m_ObjReference.transform.Position = position;
    
            // Return the new position
            return position;
        }

        public override void SetDesiredPosition(Vector3 desiredPos)
        {
            //Position = m_DesiredPosition;
            m_DesiredPosition = desiredPos;

            // Set the position this object is at when having its desired position set 
            // This is so we can smoothly move along the point
            m_PositionOnSet = m_ObjReference.transform.Position;

            // Set that there is no movement
            m_Movement = true;

            // Set the rotation to follow it in
            SetDesiredRotation(m_ObjReference.transform.Position, desiredPos);
        }

        public override void StopMovement()
        {
            m_DesiredPosition = m_ObjReference.transform.Position;
            m_Movement = false;
            //m_Velocity = 0.0f;
        }

        public override void SetDesiredRotation(Vector3 from, Vector3 to)
        {
            if (from != to)
            {
                Vector3 angles = MathLib.Instance.AngleTo(from, to);

                m_DesiredRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, angles.Y) * Quaternion.CreateFromAxisAngle(Vector3.Right, angles.X);
            }
        }

        public void ModifyDesiredRotation(Matrix modification)
        {
            Matrix current = Matrix.CreateFromQuaternion(m_DesiredRotation);

            current = modification * current;

            // Desired rotation
            m_DesiredRotation = Quaternion.CreateFromRotationMatrix(current);
        }

        protected virtual void OnTerrainLoaded()
        {
            if (m_ObjReference != null)
            {
                // Override the start position
                m_ObjReference.startTransform.Position = SetPosition(m_ObjReference.transform.Position);
            }
        }

        #region event handlers
        #endregion

        #region Property Set/Gets
        public float Velocity { get { return m_Velocity; } set { m_Velocity = value; } }
        public float MaxVelocity { get { return m_MaxVelocity; } set { m_MaxVelocity = value; } }
        public float RotationVelocity { get { return m_RotationVelocity; } set { m_RotationVelocity = value; } }

        public VisionNode Vision { get { return m_Vision; } set { m_Vision = value; } }
        public bool RenderVision { get { return m_RenderVision; } set { m_RenderVision = value; } }
        #endregion
    }
}
