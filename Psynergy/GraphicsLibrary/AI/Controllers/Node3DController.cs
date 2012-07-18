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
    public class Node3DController : Abstract3DController, IListener<TerrainSetEvent>, IListener<TerrainLoadedEvent>
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

        #region TerrainReferences
        protected float m_TerrainHeightOffset = 0.0f;
        protected TerrainNode m_TerrainReference = null;
        #endregion

        #region Vision
        protected bool m_RenderVision = true;
        protected VisionNode m_Vision = null;
        #endregion

        public Node3DController() : base()
        {
        }

        public Node3DController(Node node) : base(node)
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
                m_DesiredPosition = node.Position;
                m_DesiredRotation = node.Rotation;
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

        protected override void UpdateMovement(GameTime deltaTime)
        {
            base.UpdateMovement(deltaTime);

            if (m_ObjReference != null)
            {
                Node3D node = (m_ObjReference as Node3D);

                if (m_Movement)
                {
                    Vector3 directionVec = (m_DesiredPosition - node.Position);                    // Direction from current position to desired position
                    float distance = directionVec.Length();                                     // Distance from desired position
                    float originalDistance = (m_DesiredPosition - m_PositionOnSet).Length();    // Original distance when position was set
                    float currentDistance = (node.Position - m_PositionOnSet).Length();            // Distance from the original point

                    if (currentDistance >= originalDistance)
                    {
                        if (m_TerrainReference != null)
                            m_DesiredPosition.Y = (m_TerrainReference.GetHeight(m_DesiredPosition) + m_TerrainHeightOffset);

                        node.Position = m_DesiredPosition;

                        // Set the desired rotation to be what it currently is
                        SetDesiredRotation(node.Position, node.Position);
                    }
                    else
                    {
                        // If the node hasn't reached its destination yet
                        if (distance > 1.0f)
                        {
                            // Increase velocity 
                            m_Velocity += ((GetMovementSpeed() * (float)deltaTime.ElapsedGameTime.TotalSeconds));
                        }

                        // If there is any speed involved ( otherwise not moving
                        if (m_Velocity > 0)
                        {
                            // m_Velocity -= (FRICTION * (float)deltaTime.ElapsedGameTime.TotalSeconds);
                            m_Velocity = MathHelper.Clamp(m_Velocity, 0.0f, m_MaxVelocity);

                            // Normalize the direction vector
                            directionVec.Normalize();

                            //float originalHeight = node.PosY;

                            // What the new position will be after this update
                            Vector3 newPos = node.Position + (directionVec * (m_Velocity * (float)deltaTime.ElapsedGameTime.TotalSeconds));

                            // Calculate the height at which the soldier should be at on the terrain
                            // If the distance of the object from the original point is greater than the original calculated value then cap the position to 
                            // the stated desired position
                            float height = 0.0f;

                            if (m_TerrainReference != null)
                            {
                                height = m_TerrainReference.GetHeight(node.Position);

                                // Set the new position according to the terrain height
                                newPos.Y = (height + m_TerrainHeightOffset);
                            }
                            //else
                              //  newPos.Y = originalHeight;

                            // Set the desired rotation
                            SetDesiredRotation(node.Position, newPos);

                            // Use the directionVec * velocity to move towards point accordingly.
                            node.Position = newPos;
                        }
                    }

                    // Check whether to get a next position or not
                    if ((originalDistance - currentDistance) <= 1)
                        SetNextSplinePosition(deltaTime);
                }
            }
        }

        public virtual bool SetNextSplinePosition(GameTime deltaTime)
        {
            bool toRet = false;

            if (m_ObjReference != null)
            {
                if (m_ObjReference.GetType().IsSubclassOf(typeof(Node)))
                {
                    Node3D node = (m_ObjReference as Node3D);
                    toRet = node.SetNextSplinePosition(deltaTime);
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

                node.Rotation = Quaternion.Slerp(node.Rotation, m_DesiredRotation, (m_RotationVelocity * (float)deltaTime.ElapsedGameTime.TotalSeconds));

                // increase rotation
                node.OrbitalPitch += (node.OrbitalPitchSpeed * (float)deltaTime.ElapsedGameTime.TotalSeconds);
                node.OrbitalYaw += (node.OrbitalYawSpeed * (float)deltaTime.ElapsedGameTime.TotalSeconds);
                node.OrbitalRoll += (node.OrbitalRollSpeed * (float)deltaTime.ElapsedGameTime.TotalSeconds);
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
            if (m_TerrainReference != null)
                position.Y = m_TerrainReference.GetHeight(position);

            m_DesiredPosition = position;

            if ( m_ObjReference != null )
                m_ObjReference.Position = position;
    
            // Return the new position
            return position;
        }

        public override void SetDesiredPosition(Vector3 desiredPos)
        {
            if (m_TerrainReference != null)
                desiredPos.Y = m_TerrainReference.GetHeight(desiredPos);

            //Position = m_DesiredPosition;
            m_DesiredPosition = desiredPos;

            // Set the position this object is at when having its desired position set 
            // This is so we can smoothly move along the point
            m_PositionOnSet = m_ObjReference.Position;

            // Set that there is no movement
            m_Movement = true;

            // Set the rotation to follow it in
            SetDesiredRotation(m_ObjReference.Position, desiredPos);
        }

        public override void StopMovement()
        {
            m_DesiredPosition = m_ObjReference.Position;
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
                m_ObjReference.StartPosition = SetPosition(m_ObjReference.Position);
            }
        }

        #region event handlers
        public virtual void Handle(TerrainSetEvent message)
        {
            TerrainNode terrain = message.Terrain;

            if (terrain != null)
                m_TerrainReference = terrain;
        }

        public virtual void Handle(TerrainLoadedEvent message)
        {
            TerrainNode terrain = (message.Terrain as TerrainNode);
            Node3D node = (m_ObjReference as Node3D);

            if (m_TerrainReference != null)
            {
                // If this objects sphere is hit then select it
                if ((terrain == m_TerrainReference) && (terrain != node))
                {
                    // Run on terrain loaded.
                    OnTerrainLoaded();
                }
            }
        }
        #endregion

        #region Property Set/Gets
        public float Velocity { get { return m_Velocity; } set { m_Velocity = value; } }
        public float MaxVelocity { get { return m_MaxVelocity; } set { m_MaxVelocity = value; } }
        public float RotationVelocity { get { return m_RotationVelocity; } set { m_RotationVelocity = value; } }
        public TerrainNode TerrainReference { get { return m_TerrainReference; } set { m_TerrainReference = value; } }
        public float TerrainHeightOffset { get { return m_TerrainHeightOffset; } set { m_TerrainHeightOffset = value; } }

        public VisionNode Vision { get { return m_Vision; } set { m_Vision = value; } }
        public bool RenderVision { get { return m_RenderVision; } set { m_RenderVision = value; } }
        #endregion
    }
}
