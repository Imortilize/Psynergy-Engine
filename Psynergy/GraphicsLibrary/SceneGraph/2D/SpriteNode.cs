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
    public class SpriteNode : RenderNode, IRegister<SpriteNode>
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterString("texturefile", "TextureFile");

            base.ClassProperties(factory);
        }
        #endregion

        protected String m_RootFolderName = "";

        protected List<Texture2D> m_2DTextures = new List<Texture2D>();
        protected List<String> m_2DTextureNames = new List<String>();

        protected Texture2D m_CurrentTexture = null;

        // Used for switching which set of textures
        protected int m_TextureIndex = 0;

        protected GraphicsDeviceManager m_Graphics = null;
        protected SpriteBatch m_SpriteBatch = null;

        protected const int COLOURMAX = 255;
        protected Color m_StartColor = new Color(COLOURMAX, COLOURMAX, COLOURMAX, COLOURMAX);
        protected Color m_ActualColor = new Color(COLOURMAX, COLOURMAX, COLOURMAX, COLOURMAX);

        protected float m_Height = 0.0f;
        protected float m_Width = 0.0f;

        protected float m_RenderDepth = 0.0f;

        // Whether this texture was created in wide screen space or not
        protected bool m_WideScreen = false;

        public SpriteNode() : base("")
        {
        }

        public SpriteNode(String name)
            : base(name)
        {
        }

        public SpriteNode(String name, String assetName, Vector3 position) : base(name)
        {
            m_2DTextureNames.Add(assetName);
            m_Position = position;
        }

        public override void Initialise()
        {
            // Get the graphics related objects
            m_Graphics = RenderManager.Instance.GraphicsDeviceManager;

            Debug.Assert(m_Graphics != null, "Graphics device not loaded on a sprite node");

            base.Initialise();
        }

        public override void Load()
        {
            // Get the sprite batch related object
            m_SpriteBatch = RenderManager.Instance.SpriteBatch;

            Debug.Assert(m_SpriteBatch != null, "Sprite batch not loaded on a sprite node!");

            foreach (String textureName in m_2DTextureNames)
            {
                Texture2D texture = RenderManager.Instance.LoadTexture2D(m_RootFolderName + textureName);

                // Load texture into texture buffer
                m_2DTextures.Add(texture);
            }

            if (m_2DTextures.Count > 0)
            {
                // Set the default texture to the sprite
                SetTexture(m_2DTextures[0]);
            }
        }

        public override void Reset()
        {
        }

        public override void Update( GameTime deltaTime )
        {
        }

        public override void Render( GameTime deltaTime )
        {
            if (m_Graphics != null)
            {
                if ( m_Graphics.GraphicsDevice != null )
                    m_Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            }

            m_SpriteBatch.Draw(m_CurrentTexture, GetPos2D(), null, m_ActualColor, 0.0f, Vector2.Zero, GetScale2D(), SpriteEffects.None, MathHelper.Clamp(m_RenderDepth, 0.0f, 1.0f));

            base.Render( deltaTime );
        }

        public Texture2D GetDefaultTexture()
        {
            Texture2D toRet = null;

            if (m_2DTextures.Count > 0)
                toRet = m_2DTextures[0];

            return toRet;
        }

        public Texture2D GetTexture(int index)
        {
            Texture2D toRet = null;

            if (m_2DTextures.Count > index)
                toRet = m_2DTextures[index];

            return toRet;
        }

        public Texture2D GetTexture()
        {
            return m_CurrentTexture;
        }

        public void SetTexture( Texture2D texture )
        {
            Debug.Assert(texture != null, "Texture " + texture.Name + " is null!");

            if (texture != null)
            {
                m_CurrentTexture = texture;

                m_Height = m_CurrentTexture.Height;
                m_Width = m_CurrentTexture.Width;
            }
        }

        public void SetColour(Color color)
        {
            m_StartColor = color;
            m_ActualColor = m_StartColor;
        }

        public void SetColour(String color)
        {
            String[] splitColor = SplitString(color);
            float[] splitColorValues = new float[splitColor.Length];

            // Assert if formatted wrong
            Debug.Assert(splitColor.Length >= 3, "Colors should be greater than or equal 3 numbers long!");
            Debug.Assert(splitColor.Length <= 4, "Colors should be less than or equal 4 numbers long!");

            // Save the position accordingly
            for (int i = 0; i < splitColor.Length; i++)
            {
                splitColorValues[i] = System.Convert.ToSingle(splitColor[i]);

                if (splitColorValues[i] < 0)
                    splitColorValues[i] = 0;
                else if (splitColorValues[i] > 1)
                    splitColorValues[i] = 1;

                splitColor[i] = splitColorValues[i].ToString();
            }


            m_StartColor.R = (byte)(splitColorValues[0] * COLOURMAX);
            m_StartColor.G = (byte)(splitColorValues[1] * COLOURMAX);
            m_StartColor.B = (byte)(splitColorValues[2] * COLOURMAX);

            // Set the opacity
            if (splitColorValues.Length == 4)
                SetOpacity(splitColorValues[3]);
            else
                SetOpacity(1.0f);
        }

        public override void SetOpacity(float opacity)
        {
            if (opacity < 0)
                opacity = 0;
            else if (opacity > 1)
                opacity = 1;

            m_Opacity = opacity;

            // Save the new Colour
            byte red = (byte)(m_StartColor.R * m_Opacity);
            byte green = (byte)(m_StartColor.G * m_Opacity);
            byte blue = (byte)(m_StartColor.B * m_Opacity);
            byte alpha = (byte)(m_StartColor.R * m_Opacity);

            m_ActualColor = new Color(red, green, blue, alpha);
        }

        public void HeighTwo(float height)
        {
            m_Height = height;
        }

        public float Height { get { return (m_Height * m_Scale.Y); } set { m_Height = value; } }
        public float Width { get { return (m_Width * m_Scale.X); } set { m_Width = value; } }
        public String TextureFile 
        {
            get
            {
                String toRet = "";

                if (m_2DTextureNames.Count > m_TextureIndex)
                    toRet = m_2DTextureNames[m_TextureIndex];

                return toRet;
            } 
            set 
            { 
                m_2DTextureNames.Add(value); 
            } 
        }
        public float RenderDepth { get { return m_RenderDepth; } set { m_RenderDepth = value; } }
    }
}
