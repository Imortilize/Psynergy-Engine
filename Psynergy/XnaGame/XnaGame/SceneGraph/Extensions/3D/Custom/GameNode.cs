using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

/* Main Library */
using Psynergy;

/* Graphics Library */
using Psynergy.Graphics;

namespace XnaGame
{
    public class GameNode : ModelNode
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            base.ClassProperties(factory);
        }
        #endregion

        // Test on spline
        public SplineAsset m_Spline = null;

        public GameNode()
        {
        }

        public GameNode(String name) : base(name)
        {
        }

        public override void Reset()
        {
            base.Reset();

            // Reset the spline
            if (m_Spline != null)
                m_Spline.Reset();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            UpdateHealth(deltaTime);
        }

        private void UpdateHealth(GameTime deltaTime)
        {
        }

        public override bool SetNextPosition(GameTime deltaTime)
        {
            bool toRet = false;

            if (HasController && (m_Spline != null))
            {
                // Make sure the end of the spline hasn't been reached
                if (m_Spline.PointsExist() && !m_Spline.IsEndReached(transform.Position))
                {
                    // A spline exists with points and the end hasn't been reached yet.
                    toRet = true;

                    // Points exist and it isn't the end of the spline, 
                    // Check if the point has been reached
                    if (m_Spline.IsPointReached(transform.Position))
                    {
                        Vector3 newPos = transform.Position;

                        // Get the next spline position
                        newPos = m_Spline.GetNextControlPointPosition(transform.Position);

                        // Test interpolation
                        //newPos = m_Spline.GetNextInterpolatedPosition(deltaTime, newPos, 1, Controller.TerrainReference);

                        if (!m_Spline.IsEndReached(transform.Position))
                        {
                            // Set the next desired position to this point
                            Controller.SetDesiredPosition(newPos);
                        }
                        else
                            Controller.StopMovement();
                    }
                }
                else
                    Controller.StopMovement();
            }

            return toRet;
        }

        protected void RenderSpline(GameTime deltaTime)
        {
            if (m_Spline != null)
            {
                m_Spline.RenderPoints();
                m_Spline.RenderPath();
            }
        }

        #region Event Handlers
     
        #endregion

        #region Property Set / Gets
        public SplineAsset Spline { get { return m_Spline; } }
        #endregion
    }
}
