using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* Graphics library */
using Psynergy.Graphics;

/* Input Library */
using Psynergy.Input;

/* Camera Library */
using Psynergy.Camera;

/* Menu Library */
using Psynergy.Menus;

namespace Psynergy.Menus
{
    public class UIObjectSelectable : UIObject
    {
        public UIObjectSelectable() : base()
        {
        }

        public UIObjectSelectable(String name, String assetName, Vector3 position, String selectedTextureName, MenuAction action) : base(name, assetName, position)
        {
            // Set selected texture name
            m_SelectedTextureName = selectedTextureName;

            // Set action and set default key press
            m_Action = action;

            if (m_Action != null)
                m_Action.SetKeyPress("select");
        }

        protected String m_SelectedTextureName = null;
        protected Texture2D m_SelectedTexture = null;

        protected bool m_Selected = false;

        // Bounding area
        protected BoundingBox m_BoundingArea = new BoundingBox();

        // Associated action
        protected MenuAction m_Action = null;

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Reset()
        {
            base.Reset();

            // Make sure it is deselected
            Deselect();
        }

        public override void Load()
        {
            base.Load();

            if ((m_SelectedTextureName != null) && (m_SelectedTextureName != ""))
            {
                m_SelectedTexture = RenderManager.Instance.LoadTexture2D(m_RootFolderName + m_SelectedTextureName);

                // Set up collision box
                SetUpBoundingBox();
            }
        }

        public override void UnLoad()
        {
            base.UnLoad();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            if (IsIdle())
            {
                InputManager input = InputManager.Instance;

                if (input.IsMouseConnected())
                {
                    Vector2 currentMousePos = input.GetCurrentMousePos();
                    BaseCamera camera = CameraManager.Instance.ActiveCamera;

                    // Check if the mouse is over this option or not
                    if (IsMouseOverOption(currentMousePos) && (camera != null) && (!camera.Tween))
                    {
                        Select();

                        // run action
                        RunAction();
                    }
                    else
                        Deselect();
                }
                else
                    Deselect();
            }
            else
                Deselect();
        }

        public void Select()
        {
            m_Selected = true;

            if (m_SelectedTexture != null)
                SetTexture(m_SelectedTexture);
            else
                Console.WriteLine("Selected texture for menu option " + Name + " is null!");
        }

        public void Deselect()
        {
            m_Selected = false;

            if (GetDefaultTexture() != null)
                SetTexture(GetDefaultTexture());
        }

        private void RunAction()
        {
            if (m_Action != null)
            {
                if ( m_Action.CheckAction() )
                    m_Action.RunAction();
            }
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);
        }

        public bool IsMouseOverOption(Vector2 position)
        {
            bool toRet = false;

            GraphicsDevice device = RenderManager.Instance.GraphicsDevice;
            Vector2 baseResolution = RenderManager.Instance.BaseResolution;
            float horScaling = (float)device.PresentationParameters.BackBufferWidth / baseResolution.X;
            float verScaling = (float)device.PresentationParameters.BackBufferHeight / baseResolution.Y;
            Vector3 screenScalingFactor = new Vector3(horScaling, verScaling, 1);

            // Scaler
            Matrix globalTransformation = Matrix.CreateScale(screenScalingFactor);

            BoundingBox actualBoundingBox = new BoundingBox();
            actualBoundingBox.Min = Vector3.Transform(m_BoundingArea.Min, globalTransformation);
            actualBoundingBox.Min = Vector3.Transform(m_BoundingArea.Max, globalTransformation);

            if (m_BoundingArea.Contains(new Vector3(position.X, position.Y, 0)) != ContainmentType.Disjoint)
                toRet = true;

            return toRet;
        }

        public override void OnShow()
        {
            base.OnShow();

            // Set up collision box
            SetUpBoundingBox();
        }

        private void SetUpBoundingBox()
        {
            if (m_SelectedTexture != null)
            {
                m_BoundingArea.Min = new Vector3(PosX, PosY, 0);
                m_BoundingArea.Max = new Vector3((PosX + m_SelectedTexture.Width), (PosY + m_SelectedTexture.Height), 0);
            }
        }

        #region Property Set / Gets
        public String SelectedTextureFile { get { return m_SelectedTextureName; } set { m_SelectedTextureName = value; } }
        public bool Selected { get { return m_Selected; } set { m_Selected = value; } }
        #endregion
    }
}
