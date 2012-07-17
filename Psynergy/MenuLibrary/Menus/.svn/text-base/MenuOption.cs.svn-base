using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Psynergy;
using Psynergy.Graphics;
using Psynergy.Input;

namespace Psynergy.Menus
{
    public class MenuOption : SpriteNode
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterString("selectedtexturefile", "SelectedTextureFile");

            base.ClassProperties(factory);
        }

        #endregion
        protected MenuAction m_Action = null;
        
        // New Texture Lists for switching between textures on menu options
        protected List<Texture2D> m_SelectedTextures = new List<Texture2D>();
        protected List<String> m_SelectedTextureNames = new List<String>();

        protected bool m_Selected = false;

        // Bounding area
        protected BoundingBox m_BoundingArea = new BoundingBox();

        public MenuOption() : base()
        {
            m_RootFolderName = "Textures/Menus/";
        }

        public MenuOption(String name, String assetName, Vector3 position) : base(name, assetName, position)
        {
            m_RootFolderName = "Textures/Menus/";
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Load()
        {
            base.Load();

            foreach (String textureName in m_SelectedTextureNames)
            {
                Texture2D texture = RenderManager.Instance.LoadTexture2D(m_RootFolderName + textureName);

                // Load texture into texture buffer
                m_SelectedTextures.Add(texture);
            }

            if (m_SelectedTextures.Count > m_TextureIndex)
            {
                m_BoundingArea.Min = new Vector3(PosX, PosY, 0);
                m_BoundingArea.Max = new Vector3((PosX + m_SelectedTextures[m_TextureIndex].Width), (PosY + m_SelectedTextures[m_TextureIndex].Height), 0);
            }
        }

        public void SetMenuAction(MenuAction menuAction)
        {
            m_Action = menuAction;
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            if (m_Action != null)
            {
                // Check if the action is used or not
                // If so then run it, otherwise keep polling
                if (m_Action.CheckAction())
                {
                    m_Action.RunAction();

                    // If it is a switchable texture menu option then switch the texture index
                    // ( For now just cycle through them
                    if ((m_2DTextures.Count > (m_TextureIndex + 1)) && ((m_SelectedTextures.Count > (m_TextureIndex + 1))))
                        m_TextureIndex++;
                    else
                        m_TextureIndex = 0;

                    m_BoundingArea.Min = new Vector3(PosX, PosY, 0);
                    m_BoundingArea.Max = new Vector3((PosX + m_SelectedTextures[m_TextureIndex].Width), (PosY + m_SelectedTextures[m_TextureIndex].Height), 0);

                    InputManager input = InputManager.Instance;

                    // Check if we are over the option or not
                    if ( IsMouseOverOption(input.GetCurrentMousePos()))
                        SetTexture(m_SelectedTextures[m_TextureIndex]);
                    else
                        SetTexture(m_2DTextures[m_TextureIndex]);
                }
            }
        }

        public void Select()
        {
            m_Selected = true;

            if (m_SelectedTextures[m_TextureIndex] != null)
                SetTexture(m_SelectedTextures[m_TextureIndex]);
            else
                Console.WriteLine("Selected texture for menu option " + Name + " is null!");
        }

        public void Deselect()
        {
            m_Selected = false;

            Debug.Assert(GetDefaultTexture() != null, "No default texture has been set!");

            if (m_2DTextures[m_TextureIndex] != null)
                SetTexture(m_2DTextures[m_TextureIndex]);
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);
        }

        public bool IsMouseOverOption( Vector2 position )
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
            actualBoundingBox.Max = Vector3.Transform(m_BoundingArea.Max, globalTransformation);

            if (actualBoundingBox.Contains(new Vector3(position.X, position.Y, 0)) != ContainmentType.Disjoint)
                toRet = true;

            return toRet;
        }

        #region Property Set / Gets
        public String SelectedTextureFile 
        { 
            get 
            {
                String toRet = "";

                if (m_SelectedTextureNames.Count > m_TextureIndex)
                    toRet = m_SelectedTextureNames[m_TextureIndex];

                return toRet; 
            } 
            set 
            {
                m_SelectedTextureNames.Add(value); 
            } 
        }

        public bool Selected { get { return m_Selected; } set { m_Selected = value; } }
        #endregion
    }
}
