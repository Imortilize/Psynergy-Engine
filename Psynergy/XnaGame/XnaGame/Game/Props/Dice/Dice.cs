using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

/* Graphics Library */
using Psynergy.Graphics;

namespace XnaGame
{
    public class Dice : ModelNode
    {
        public enum ActiveState
        {
            eState_Enabled = 0,
            eState_Disabled = 1
        }

        protected ActiveState m_ActiveState = ActiveState.eState_Enabled;

        public Dice() : base("")
        {
        }

        public Dice(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            // Cube specific variables
            //TextureName = "Textures/Others/default";
            RenderGroupName = "shadow";

            // Set modelName
            ModelName = "Models/FBX/Dice/dice";

            // Set dice scale
            transform.Scale *= 0.1f;

            base.Initialise();
        }

        public override void Reset()
        {
            base.Reset();

            // Disable to start
            Disable();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);
            
            // Update activestate
            UpdateActiveState(deltaTime);
        }

        private void UpdateActiveState(GameTime deltaTime)
        {
            switch (m_ActiveState)
            {
                case ActiveState.eState_Enabled:
                    {
                        UpdateEnabled(deltaTime);
                    }
                    break;
                case ActiveState.eState_Disabled:
                    {
                        UpdatedDisabled(deltaTime);
                    }
                    break;
            }
        }

        protected virtual void UpdateEnabled(GameTime deltaTime)
        {
        }

        protected virtual void UpdatedDisabled(GameTime deltaTime)
        {
        }

        public virtual void Enable()
        {
            // Set active state to enabled
            m_ActiveState = ActiveState.eState_Enabled;

            // Set opacity to full
            SetOpacity(1.0f);

            // Set to render
            ActiveRender = true;
        }

        public virtual void Disable()
        {
            // Set active state to disabled
            m_ActiveState = ActiveState.eState_Disabled;
        }

        public int FindResult()
        {
            int toRet = 0;

            Vector3 compareVec = Vector3.Up;

            // Top face //
            if (CheckFace(compareVec, transform.WorldMatrix.Up))
                return 5;

            // left face //
            if (CheckFace(compareVec, transform.WorldMatrix.Left))
                return 4;

            // Back face //
            if (CheckFace(compareVec, transform.WorldMatrix.Backward))
                return 1;

            // Right face //
            if (CheckFace(compareVec, transform.WorldMatrix.Right))
                return 3;

            // Front face //
            if (CheckFace(compareVec, transform.WorldMatrix.Forward))
                return 6;

            // Bottom face //
            if (CheckFace(compareVec, transform.WorldMatrix.Down))
                return 2;

            return toRet;
        }

        private bool CheckFace(Vector3 compareVec, Vector3 faceVec)
        {
            bool toRet = false;

            // surface
            faceVec.Normalize();

            // Do dice face calculations ( dot product with down vector )
            float dot = Vector3.Dot(faceVec, compareVec);

            // If the dot product is greater then the set value then it is this face ( give 0.05f% inaccuracy leniancy )
            if (dot >= 0.95f)
                toRet = true;

            return toRet;
        }
    }
}
