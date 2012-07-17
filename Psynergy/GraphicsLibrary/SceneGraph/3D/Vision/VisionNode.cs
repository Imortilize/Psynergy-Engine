using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

namespace Psynergy.Graphics
{
    public class VisionNode : Node
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterFloat("height", "Height");
            factory.RegisterFloat("radius", "Radius");
            factory.RegisterFloat("distance", "Distance");

            base.ClassProperties(factory);
        }
        #endregion

        private float m_Height = 0.0f;
        private float m_Radius = 0.0f;
        private float m_Distance = 200.0f;

        public VisionNode()
        {
        }

        public VisionNode(float visionHeight, float visionRadius, float visionDistance)
        {
            m_Height = visionHeight;
            m_Radius = visionRadius;
            m_Distance = visionDistance;
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);
        }

        public void Render(GameTime deltaTime, Node3D objectRef)
        {
            base.Render(deltaTime);

            if (objectRef != null)
            {
                if (m_Radius > 0)
                {
                    // Get this objects scale
                    Vector3 scale = objectRef.Scale;

                    // Turn the scale vector into a normalized single unit scale which will 
                    // in turn normalise the size of the sight ( takes into account objects that have been scaled up / down ).
                    scale = new Vector3((1 / scale.X), (1 / scale.Y), (1 / scale.Z));

                    // Take this objects forward vector and multiply it by its vision radius to get
                    // the vision in front of them.
                    float halfRadius = (m_Radius * 0.5f);
                    Vector3 axis = (objectRef.WorldMatrix.Forward * m_Distance * scale);
                    Vector3 minVisionPoint = Vector3.Transform(axis, Matrix.CreateRotationY(MathHelper.ToRadians(-halfRadius)));
                    Vector3 maxVisionPoint = Vector3.Transform(axis, Matrix.CreateRotationY(MathHelper.ToRadians(halfRadius)));

                    // Add a new line group
                    DebugRender.Instance.AddDebugVision((objectRef.Position + new Vector3(0, m_Height, 0)), axis, minVisionPoint, maxVisionPoint);
                }
            }
        }

        public bool IsInView(Vector3 position, Node3D objectRef)
        {
            bool toRet = false;

            if (objectRef != null)
            {
                Vector3 objPos = objectRef.Position;
                Vector3 offset = (position - objPos);
                float distance = offset.Length();

                // Has to be within the distance to be visible
                if (distance <= m_Distance)
                {
                    float halfRadius = (m_Radius * 0.5f);
                    Vector3 forwardVector = objectRef.WorldMatrix.Forward;
                    Vector3 scale = objectRef.Scale;

                    Vector3 axis = (objPos + (forwardVector * m_Distance * scale));
                    Vector3 axisPos = (axis - objPos);

                    Vector3 minVisionPoint = Vector3.Transform(axis, Matrix.CreateRotationY(MathHelper.ToRadians(-halfRadius)));
                    minVisionPoint = (minVisionPoint - objPos);
  
                    // Normalize vectors
                    axisPos.Normalize();
                    offset.Normalize();
                    minVisionPoint.Normalize();

                    // Calculate the vision boundary
                    float visionBoundary = Vector3.Dot(axisPos, minVisionPoint);
                    float actualBoundary = Vector3.Dot(offset, minVisionPoint);

                    // If within the vision radius ( WINNER )
                    if (actualBoundary >= visionBoundary)
                        toRet = true;
                    // 
                }
            }

            return toRet;
        }

        #region Property Set/Gets
        public float Height { get { return m_Height; } set { m_Height = value; } }
        public float Radius { get { return m_Radius; } set { m_Radius = value; } }
        public float Distance { get { return m_Distance; } set { m_Distance = value; } }
        #endregion
    }
}
