using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Graphics
{
    public class SplineAsset
    {
        private String m_Name = "";
        private List<Vector3> m_ControlPoints = new List<Vector3>();                // List of points to interpolate between
        private List<Vector3> m_InterpolatedPoints = new List<Vector3>();

        private bool    m_Loop = false;                                         // Whether to loop on the spline or not
        private int     m_ControlPointIndex = -1;                               // What control point the model should be interpolating to
        private float   m_PointLeniancy = 1f;                                 // Leniany to detect when the point has been reached
        private bool    m_EndReached = false;
        private bool    m_PointReached = true;                                  // True to start so that the first point can be used
        private Vector3 m_LastPosition = Vector3.Zero;                          // Last position returned         

        // Interpolate functions
        private float   m_CurrentInterpolateValue = 0.0f;

        public SplineAsset(String name)
        {
            m_Name = name;
        }

        public SplineAsset(String name, bool loop)
        {
            m_Name = name;
            m_Loop = loop;
        }

        public SplineAsset(String name, List<Vector3> controlPoints, bool loop )
        {
            m_Name = name;
            m_ControlPoints = controlPoints;
            m_Loop = loop;
        }

        public void Reset()
        {
            m_LastPosition = Vector3.Zero;
            m_ControlPointIndex = 0;                // Start at -1 so it increments to get the first point to begin with
            m_CurrentInterpolateValue = 0.0f;       // Interpolate value is reset
            m_EndReached = false;
            m_PointReached = true;
        }

        public Vector3 GetNextControlPointPosition( Vector3 position )
        {
            m_LastPosition = Vector3.Zero;

            // Check that control point exist
            if (m_ControlPoints.Count > 0)
            {
                if (!m_EndReached)
                {
                    if (IsPointReached(position))
                    {
                        if (m_ControlPointIndex < (m_ControlPoints.Count - 1))
                            m_ControlPointIndex++;
                        else if (m_Loop)
                            m_ControlPointIndex = 0;
                        else
                        {
                            // Get the final control point
                            m_LastPosition = m_ControlPoints[(m_ControlPoints.Count - 1)];

                            // State the end has been reached
                            m_EndReached = true;
                        }
                    }

                    // If the end wasn't reached
                    if (!m_EndReached)
                    {
                        m_LastPosition = m_ControlPoints[m_ControlPointIndex];
                        m_PointReached = false;
                    }
                }
            }

            // If the last value and is already reached then it won't be used.
            return m_LastPosition;
        }

        /* A smoother, interpolated position */
        public Vector3 GetNextInterpolatedPosition(GameTime deltaTime, Vector3 position, float intervals)
        {
            return GetNextInterpolatedPosition(deltaTime, position, intervals, null);
        }

        public Vector3 GetNextInterpolatedPosition(GameTime deltaTime, Vector3 position, float intervals, TerrainNode terrain)
        {
            Vector3 toRet = m_LastPosition;

            if (m_ControlPoints.Count > 0)
            {
                int max = (m_ControlPoints.Count - 1);

                // If the current interpolate value is greater than or equal to the value of "1" then the end of the path between these
                // two control points has been reached so increase the control point index.
                if (m_CurrentInterpolateValue >= 1)
                {
                    m_CurrentInterpolateValue = 0;

                    if (m_ControlPointIndex < (m_ControlPoints.Count - 2))
                        m_ControlPointIndex++;
                    else
                    {
                        if (m_Loop)
                            m_ControlPointIndex = 0;
                        else
                        {
                            m_EndReached = true;
                            toRet = m_ControlPoints[max];
                        }
                    }
                }

                // Make sure the end hasn't been reached yet
                if (!m_EndReached)
                {
                    toRet = CatMullRom3D(m_ControlPoints[Wrap((m_ControlPointIndex - 1), max)],
                                          m_ControlPoints[Wrap(m_ControlPointIndex, max)],
                                          m_ControlPoints[Wrap((m_ControlPointIndex + 1), max)],
                                          m_ControlPoints[Wrap((m_ControlPointIndex + 2), max)],
                                          m_CurrentInterpolateValue);

                    // Increase the interpolate value by the speed * deltaTime
                    if (intervals <= 0)
                        intervals = 0;

                    m_CurrentInterpolateValue += (1 / intervals) * (float)deltaTime.ElapsedGameTime.TotalSeconds;
                }
            }

            if (terrain != null)
                toRet.Y = terrain.GetHeight(toRet);

            // Set the last position to be returned ( if the end has been reached then it should set itself to what it already is 
            // and return the same value ( hence not move anywhere else
            m_LastPosition = toRet;

            return toRet;
        }

        public void AddControlPoint( Vector3 newPoint, TerrainNode terrain )
        {
            // If a terrain has been passed in, check the height of the terrain
            // at the position the new point is at
            if (terrain != null)
                newPoint.Y = terrain.GetHeight(newPoint); // +newPoint.Y;

            if ( m_ControlPoints.Count > 0 )
            {
                // Check whether to have the end point the same as the start or whether to just directly add a point
                // Depends on looping or not
                if (m_Loop)
                    m_ControlPoints.Insert( (m_ControlPoints.Count - 1), newPoint );
                else
                    m_ControlPoints.Add(newPoint);
            }
            else
            {
                m_ControlPoints.Add(newPoint);

                // If looping add the point again as the end point
                if (m_Loop) 
                    m_ControlPoints.Add(newPoint);
            }
        }

        public void AddControlPoints(List<Vector3> newPoints, TerrainNode terrain)
        {
            for (int i = 0; i < newPoints.Count; i++ )
            {
                Vector3 point = newPoints[i];

                // If a terrain has been passed in, check the height of the terrain
                // at the position the new point is at
                if (terrain != null)
                    point.Y = terrain.GetHeight(point); // +newPoint.Y;

                if (m_ControlPoints.Count > 0)
                {
                    // Check whether to have the end point the same as the start or whether to just directly add a point
                    // Depends on looping or not
                    if (m_Loop)
                        m_ControlPoints.Insert((m_ControlPoints.Count - 1), point);
                    else
                        m_ControlPoints.Add(point);
                }
                else
                {
                    m_ControlPoints.Add(point);

                    // If looping add the point again as the end point
                    if (m_Loop)
                        m_ControlPoints.Add(point);
                }
            }
        }

        public void InterpolatePoints(int divisions)
        {
            // Run interpolate points without a terrain node attached
            InterpolatePoints(divisions, null, 0.0f);
        }

        public void InterpolatePoints( int divisions, TerrainNode terrain, float terrainOffset )
        {
            // If the points havn't been interpolated already and points exist to interpolate between
            if (m_ControlPoints.Count > 0)
            {
                // Create a new list of positions
                List<Vector3> interpolatedPositions = new List<Vector3>();
                int maxPoints = (m_ControlPoints.Count - 1);

                // Between each control point....
                for (int i = 0; i < maxPoints; i++)
                {
                    // Add the control point to the new list
                    interpolatedPositions.Add(m_ControlPoints[i]);

                    // ADd the specifed number of interpolated points
                    for (int j = 0; j < divisions; j++)
                    {
                        // Determine how far to interpolate
                        float amount = ((float)(j + 1) / (float)(divisions + 2));

                        // Find the position based on catmull-rom interpolation
                        Vector3 interpolation = CatMullRom3D(m_ControlPoints[Wrap((i - 1), maxPoints)],
                                                             m_ControlPoints[Wrap(i, maxPoints)],
                                                             m_ControlPoints[Wrap((i + 1), maxPoints)],
                                                             m_ControlPoints[Wrap((i + 2), maxPoints)],
                                                             amount);

                        // Check if a terrain node was passed in or not
                        if (terrain != null)
                        {
                            // If so then check this interpolated points height against
                            // the terrain heights and adjust accordingly
                            interpolation.Y = terrain.GetHeight(interpolation) + terrainOffset;
                        }

                        // Add the new point to the list
                        interpolatedPositions.Add(interpolation);
                    }
                }

                // Add the last point
                interpolatedPositions.Add(m_ControlPoints[maxPoints]);

                // Save the interpolated positions over the original positions
                m_InterpolatedPoints = interpolatedPositions;
            }
        }

        // Point being targetting for
        public bool IsPointReached(Vector3 position)
        {
            bool toRet = false;

            // Check that control points exist
            if (m_ControlPoints.Count > 0)
            {
                float distance = Vector3.Distance(position, m_LastPosition);

                if (distance < m_PointLeniancy)
                    toRet = m_PointReached = true;
                else
                {
                    toRet = m_PointReached;
                    m_PointReached = false;
                }
            }

            return toRet;
        }

        // Whether the end of the spline has been reached
        public bool IsEndReached( Vector3 position )
        {
            if (!m_EndReached)
            {
                // Check that control points exist
                if (m_ControlPoints.Count > 0)
                {
                    if (m_ControlPointIndex > -1)
                    {
                        float distance = Vector3.Distance(position, m_ControlPoints[m_ControlPointIndex]);

                        if (distance < m_PointLeniancy)
                        {
                            if ((m_ControlPointIndex >= (m_ControlPoints.Count - 1)) && !m_Loop)
                                m_EndReached = true;
                        }
                    }
                }
                else
                    m_EndReached = true;
            }

            return m_EndReached;
        }

        public bool PointsExist()
        {
            return (m_ControlPoints.Count > 0);
        }

        public void RenderPoints()
        {
                foreach (Vector3 point in m_InterpolatedPoints)
                    DebugRender.Instance.AddDebugPoint(point);
         
                foreach (Vector3 point in m_ControlPoints)
                    DebugRender.Instance.AddDebugPoint(point);
        }

        public void RenderPath()
        {
            if (m_InterpolatedPoints.Count > 0)
            {
                // Add a new line group
                DebugRender.Instance.AddNewDebugLineGroup( m_InterpolatedPoints[0], Color.Yellow );

                for (int i = 0; i < m_InterpolatedPoints.Count; i++)
                    DebugRender.Instance.AddDebugLine(m_InterpolatedPoints[i], Color.Green);

                // If looped then add the beginning point to it
                if (m_Loop)
                    DebugRender.Instance.AddDebugLine(m_InterpolatedPoints[m_InterpolatedPoints.Count - 1], Color.Yellow);
            }
            
            if (m_ControlPoints.Count > 0)
            {
                // Add a new line group
                DebugRender.Instance.AddNewDebugLineGroup(m_ControlPoints[0], Color.Red);

                for (int i = 0; i < m_ControlPoints.Count; i++)
                    DebugRender.Instance.AddDebugLine(m_ControlPoints[i], Color.Red);

                // If looped then add the beginning point to it
                if ( m_Loop )
                    DebugRender.Instance.AddDebugLine(m_ControlPoints[m_ControlPoints.Count - 1], Color.Yellow);
            }
        }

        public Vector3 GetStartPoint()
        {
            Vector3 toRet = Vector3.Zero;

            if (m_InterpolatedPoints.Count > 0)
                toRet = m_InterpolatedPoints[0];
            else if (m_ControlPoints.Count > 0)
                toRet = m_ControlPoints[0];

            return toRet;
        }

        public Vector3 GetEndPoint()
        {
            Vector3 toRet = Vector3.Zero;

            if (m_InterpolatedPoints.Count > 0)
                toRet = m_InterpolatedPoints[(m_InterpolatedPoints.Count - 1)];
            else if (m_ControlPoints.Count > 0)
                toRet = m_ControlPoints[(m_ControlPoints.Count - 1)];

            return toRet;
        }

        private Vector3 CatMullRom3D(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float amount)
        {
            return new Vector3( MathHelper.CatmullRom(v1.X, v2.X, v3.X, v4.X, amount), 
                                MathHelper.CatmullRom(v1.Y, v2.Y, v3.Y, v4.Y, amount),
                                MathHelper.CatmullRom(v1.Z, v2.Z, v3.Z, v4.Z, amount) );
        }

        // Wraps the CatMull "value" argument around [0, max]
        private int Wrap(int value, int max)
        {
            while (value > max)
                value -= max;

            while (value < 0)
                value += max;

            return value;
        }

        public void ClearSpline()
        {
            m_ControlPoints.Clear();
            m_InterpolatedPoints.Clear();

            // Reset the spline
            Reset();
        }

        public int NumControlPoints()
        {
            return m_ControlPoints.Count;
        }

        #region Property Set / Gets
        public bool Loop { get { return m_Loop; } set { m_Loop = value; } }
        #endregion
    }
}
