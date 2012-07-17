using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

using Microsoft.Xna.Framework;

/* Main Library */
using Psynergy;

/* Graphics Library */
using Psynergy.Graphics;

namespace XnaGame
{
    public class BoardTrail : GameObject
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterVector3("point", "Point");
            factory.RegisterInt("route", "Route");
            factory.RegisterInt("drawroutecolour", "DrawRouteColor");
            factory.RegisterString("connection", "Connection");

            base.ClassProperties(factory);
        }
        #endregion

        private BoardSquare.Route m_Route = BoardSquare.Route.eDefault;
        private BoardSquare.Route m_DrawRouteColour = BoardSquare.Route.eDefault;
        private List<Vector3> m_TrailPoints = new List<Vector3>();
        private List<String> m_Connections = new List<String>();

        public BoardTrail()
        {
        }

        public void AddPoint(Vector3 newPoint)
        {
            m_TrailPoints.Add(newPoint);
        }

        public List<Vector3> GetPoints()
        {
            return m_TrailPoints;
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);

            if (m_TrailPoints.Count > 0)
            {
                Color color = Color.Blue;

                if (m_DrawRouteColour == BoardSquare.Route.eAlternative1)
                    color = Color.Red;
                else if (m_DrawRouteColour == BoardSquare.Route.eAlternative2)
                    color = Color.Red;
                else if (m_DrawRouteColour == BoardSquare.Route.eAlternative3)
                    color = Color.Red;

                Vector3 offset = new Vector3(0, 0.05f, 0.0f);

                DebugLineGroup lineGroup = new DebugLineGroup(new List<DebugLinePoint>());
                lineGroup.AddLinePoint(m_TrailPoints[0] + offset, color);

                // Render points between the start and end point
                if (m_TrailPoints.Count > 2)
                {
                    for (int i = 0; i < m_TrailPoints.Count; i++)
                    {
                        if ((i > 0) && (i < (m_TrailPoints.Count - 1)))
                            lineGroup.AddLinePoint(m_TrailPoints[i] + offset, color);
                    }
                }

                // Add final point
                lineGroup.AddLinePoint(m_TrailPoints[(m_TrailPoints.Count - 1)] + offset, color);

                // Render line group
                DebugRender.Instance.RenderLineGroup(lineGroup);

                // Clear line point
                lineGroup.linePoints.Clear();
            }
        }
       
        #region Properties
        public BoardSquare.Route Route 
        { 
            get 
            { 
                return m_Route; 
            } 
            set 
            { 
                m_Route = value; 
            } 
        }

        public BoardSquare.Route DrawRouteColor
        {
            get
            {
                return m_DrawRouteColour;
            }
            set
            {
                m_DrawRouteColour = value;
            }
        }

        public Vector3 Point 
        { 
            get 
            {
                Vector3 toRet = Vector3.Zero;

                if (m_TrailPoints.Count > 0)
                    toRet = m_TrailPoints[0];

                return toRet; 
            } 
            set 
            { 
                // Add a trail point
                AddPoint(value);
            } 
        }

        public String Connection
        {
            get
            {
                String toRet = "";

                if (m_Connections.Count > 0)
                    toRet = m_Connections[0];

                return toRet;
            }
            set
            {
                if (!m_Connections.Contains(value))
                    m_Connections.Add(value);
            }
        }

        public List<String> Connections { get { return m_Connections; } set { m_Connections = value; } }
        #endregion
    }
}
