using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Psynergy
{
    public class Transform
    {
        #region Fields
        private GameObject m_GameObject = null;
        private Vector3 m_Position = Vector3.Zero;
        private Quaternion m_Rotation = Quaternion.Identity;
        private Quaternion m_OrbitalRotation = Quaternion.Identity;
        private Vector3 m_Scale = Vector3.One;

        // Matrices
        private Matrix m_LocalMatrix = Matrix.Identity;
        private Matrix m_WorldMatrix = Matrix.Identity;

        // Whether to use parent scale or not
        private bool m_UseParentScale = false;

        // Pivot (TODO: Attach a pivot for this object to transform around or a list of pivots maybe?)
        #endregion

        #region Constructor
        public Transform(GameObject gameObject)
        {
            m_GameObject = gameObject;
        }
        #endregion

        #region Functions
        public void Rebuild()
        {                      
            if (m_GameObject != null)
            {
                // Rebuild world matrix...
                // Grab parent if it exists
                GameObject parent = m_GameObject.Parent;

                // Position 
                Vector3 positionToUse = m_Position;

                if (parent != null)
                    positionToUse -= parent.transform.Position;

                // Build the world matrix and the local world matrix 
                // The world matrix uses the pivot node and is what children nodes use to determine world rotations according to parents
                // The local matrix is what this node specifically uses and uses its own rotations
                m_LocalMatrix = (Matrix.CreateScale(m_Scale) * Matrix.CreateFromQuaternion(m_Rotation) * Matrix.CreateTranslation(positionToUse) * Matrix.CreateFromQuaternion(m_OrbitalRotation));
                if (parent != null)
                {
                    Vector3 parentScale = Vector3.One;

                    if (m_UseParentScale)
                        parentScale = parent.transform.Scale;

                    // Multiply the world matrix by it's parent's world matrix which brings it to its parents world space
                    m_WorldMatrix = (Matrix.CreateScale(parentScale * m_Scale) * Matrix.CreateWorld((parent.transform.Position + m_LocalMatrix.Translation), m_LocalMatrix.Forward, m_LocalMatrix.Up));
                }
                else
                {
                    m_WorldMatrix = m_LocalMatrix;
                }

                // Tell the system the transform has been updated
                m_GameObject.OnTransformUpdated();
            }
        }
        #endregion

        #region Properties
        public Vector3 Position
        {
            get { return m_Position; }
            set
            {
                m_Position = value;

                // Rebuild transform
                //Rebuild();
            }
        }

        public Quaternion Rotation
        {
            get { return m_Rotation; }
            set
            {
                m_Rotation = value;

                // Rebuild transform
                //Rebuild();
            }
        }

        public Vector3 RotationInDegrees
        {
            set
            {
                m_Rotation = Quaternion.CreateFromYawPitchRoll(value.Y, value.X, value.Z);
            }
        }

        public Quaternion OrbitalRotation
        {
            get { return m_OrbitalRotation; }
            set
            {
                m_OrbitalRotation = value;

                // Rebuild transform
               // Rebuild();
            }
        }

        public Vector3 OrbitalRotationInDegrees
        {
            set
            {
                m_OrbitalRotation = Quaternion.CreateFromYawPitchRoll(value.Y, value.X, value.Z);
            }
        }

        public Vector3 Scale
        {
            get { return m_Scale; }
            set
            {
                m_Scale = value;

                // Rebuild transform
                //Rebuild();
            }
        }

        public Matrix LocalMatrix { get { return m_LocalMatrix; } set { m_LocalMatrix = value; } }
        public Matrix WorldMatrix { get { return m_WorldMatrix; } set { m_WorldMatrix = value; Position = value.Translation; } }
        
        // Use parent scale or not
        public bool UseParentScale { get { return m_UseParentScale; } set { m_UseParentScale = value; } }
        #endregion
    }
}
