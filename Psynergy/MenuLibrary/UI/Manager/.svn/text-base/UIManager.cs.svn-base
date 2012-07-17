using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Psynergy;
using Psynergy.Graphics;

namespace Psynergy.Menus
{
    public class UIManager : Singleton<UIManager>
    {
        public enum TextureAlignment
        {
            eTopLeft = 0,
            eTopRight = 1,
            eCenter = 2,
            eCenterLeft = 3,
            eCenterRight = 4,
            eCenterBottom = 5
        }

        // Sprite batch for menu drawing
        private SpriteBatch m_SpriteBatch = null;

        // list of textures to render
        private SortedList<String, UIObject> m_LoadedTextures = new SortedList<String, UIObject>();
        private SortedList<String, UIObject> m_Textures = new SortedList<String, UIObject>();
        List<String> m_UIObjectsToRemove = new List<String>();

        public UIManager()
        {
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Reset()
        {
            base.Reset();

            for (int i = 0; i < m_Textures.Count; i++)
            {
                UIObject uiObject = m_Textures.Values[i];

                if ( uiObject != null )
                    uiObject.Reset();
            }

            m_Textures.Clear();
            m_UIObjectsToRemove.Clear();
        }

        public override void Load()
        {
            base.Load();
        }

        public override void UnLoad()
        {
            base.UnLoad();
        }

        public void LoadTexture(String name, String assetName)
        {
            if (!m_LoadedTextures.ContainsKey(name))
            {
                UIObject newSprite = new UIObject(name, assetName, Vector3.Zero);
                newSprite.Initialise();
                newSprite.Load();

                // Add loaded texture
                m_LoadedTextures.Add(name, newSprite);
            }
        }

        public void LoadTexture(String name, String assetName, Vector3 position, UIObjectEffect effect, float depth)
        {
            if (!m_LoadedTextures.ContainsKey(name))
            {
                UIObject newSprite = new UIObject(name, assetName, position);
                newSprite.Initialise();
                newSprite.Load();

                newSprite.RenderDepth = depth;

                // Add loaded texture
                m_LoadedTextures.Add(name, newSprite);

                if (effect != null)
                    effect.SetUIObject(newSprite);
            }
        }

        public void LoadTexture(String name, String assetName, Vector3 position)
        {
            if (!m_LoadedTextures.ContainsKey(name))
            {
                UIObject newSprite = new UIObject(name, assetName, position);
                newSprite.Initialise();
                newSprite.Load();

                // Add loaded texture
                m_LoadedTextures.Add(name, newSprite);
            }
        }

        public void LoadSelectableTexture(String name, String assetName, String selectedAssetName, MenuAction action )
        {
            if (!m_LoadedTextures.ContainsKey(name))
            {
                UIObjectSelectable newSprite = new UIObjectSelectable(name, assetName, Vector3.Zero, selectedAssetName, action);
                newSprite.Initialise();
                newSprite.Load();

                // Add loaded texture
                m_LoadedTextures.Add(name, newSprite);
            }
        }

        public void LoadSelectableTexture(String name, String assetName, Vector3 position, String selectedAssetName, MenuAction action)
        {
            if (!m_LoadedTextures.ContainsKey(name))
            {
                UIObjectSelectable newSprite = new UIObjectSelectable(name, assetName, position, selectedAssetName, action);
                newSprite.Initialise();
                newSprite.Load();

                // Add loaded texture
                m_LoadedTextures.Add(name, newSprite);
            }
        }

        public void LoadSelectableTexture(String name, String assetName, String selectedAssetName, MenuAction action, UIObjectEffect effect)
        {
            LoadSelectableTexture(name, assetName, selectedAssetName, action, effect, 0.9f); 
        }

        public void LoadSelectableTexture(String name, String assetName, String selectedAssetName, MenuAction action, UIObjectEffect effect, float depth)
        {
            if (!m_LoadedTextures.ContainsKey(name))
            {
                UIObjectSelectable newSprite = new UIObjectSelectable(name, assetName, Vector3.Zero, selectedAssetName, action);
                newSprite.Initialise();
                newSprite.Load();

                // Set render depth
                newSprite.RenderDepth = depth;

                // Add loaded texture
                m_LoadedTextures.Add(name, newSprite);

                // Set effect
                if (effect != null)
                    effect.SetUIObject(newSprite);
            }
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            // Render any textures pushed to the User interface
            if (m_Textures.Count > 0)
            {
                m_UIObjectsToRemove.Clear();

                for ( int i = 0; i < m_Textures.Count; i++ )
                {
                    KeyValuePair<String, UIObject> sprite = m_Textures.ElementAt(i);

                    if ( sprite.Value != null )
                    {
                        UIObject UIObject = sprite.Value;

                        if (!UIObject.RemoveObject())
                            UIObject.Update(deltaTime);
                        else
                            m_UIObjectsToRemove.Add(sprite.Key);
                    }
                }

                // Check if there are any objects to remove this cycle
                if ( m_UIObjectsToRemove.Count > 0 )
                {
                    foreach (String objectToRemove in m_UIObjectsToRemove)
                        FlagToRemoveUIObject(objectToRemove);
                }
            }
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);

            // Render the current menu if there is one to render.
            if (m_SpriteBatch != null)
            {
                GraphicsDevice device = RenderManager.Instance.GraphicsDevice;
                Vector2 baseResolution = RenderManager.Instance.BaseResolution;
                float horScaling = (float)device.PresentationParameters.BackBufferWidth / baseResolution.X;
                float verScaling = (float)device.PresentationParameters.BackBufferHeight / baseResolution.Y;
                Vector3 screenScalingFactor = new Vector3(horScaling, verScaling, 1);

                // Scaler
                Matrix globalTransformation = Matrix.CreateScale(screenScalingFactor);

                // Begin a seperate sprite batch for menus
                m_SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, globalTransformation);

                // Render any textures pushed to the User interface
                if (m_Textures.Count > 0)
                {
                    foreach (UIObject sprite in m_Textures.Values)
                        sprite.Render(deltaTime);
                }

                // End the sprite batch
                m_SpriteBatch.End();
            }
        }

        public UIObject AddUIObject(String name, Vector3 position)
        {
            return AddUIObject(name, position, TextureAlignment.eTopLeft);
        }

        public UIObject AddUIObject(String name, Vector3 position, TextureAlignment alignment)
        {
            UIObject sprite = null;

            if (m_LoadedTextures.Count > 0)
            {
                int index = m_LoadedTextures.IndexOfKey(name);

                if ((index >= 0) && (index < m_LoadedTextures.Count))
                {
                    sprite = m_LoadedTextures.Values[index];

                    Vector3 newPos = position;

                    if (alignment == TextureAlignment.eTopRight)
                        newPos -= new Vector3(sprite.Width, 0.0f, 0.0f);
                    else if ( alignment == TextureAlignment.eCenter )
                        newPos -= new Vector3((sprite.Width * 0.5f), (sprite.Height * 0.5f), 0.0f );
                    else if ( alignment == TextureAlignment.eCenterLeft )
                        newPos -= new Vector3(0.0f, (sprite.Height * 0.5f), 0.0f);
                    else if (alignment == TextureAlignment.eCenterRight)
                        newPos -= new Vector3(sprite.Width, (sprite.Height * 0.5f), 0.0f);
                    else if (alignment == TextureAlignment.eCenterBottom)
                        newPos -= new Vector3((sprite.Width * 0.5f), sprite.Height, 0.0f);

                    // Reposition accordingly
                    sprite.SetPos(newPos);

                    if (!m_Textures.ContainsValue(sprite))
                    {
                        // Add to the render textures
                        m_Textures.Add(name, sprite);
                    }

                    // Run any post show code ( such as collision box movement )
                    sprite.OnShow();
                }
            }

            return sprite;
        }

        public UIObject AddUIObject(UIObject UIObject, Vector3 position)
        {
            UIObject toRet = UIObject;

            if (toRet != null)
            {
                Vector3 newPos = position;

                // Reposition accordingly
                toRet.SetPos(newPos);

                if (!m_Textures.ContainsValue(toRet))
                {
                    // Add to the render textures
                    m_Textures.Add(toRet.Name, toRet);
                }

                // Run any post show code ( such as collision box movement )
                toRet.OnShow();
            }

            return toRet;
        }

        public UIObject AddUIObject(UIObject UIObject, Vector3 position, TextureAlignment alignment)
        {
            UIObject toRet = UIObject;

            if (toRet != null)
            {
                Vector3 newPos = position;

                if (alignment == TextureAlignment.eTopRight)
                    newPos -= new Vector3(toRet.Width, 0.0f, 0.0f);
                else if (alignment == TextureAlignment.eCenter)
                    newPos -= new Vector3((toRet.Width * 0.5f), (toRet.Height * 0.5f), 0.0f);
                else if (alignment == TextureAlignment.eCenterLeft)
                    newPos -= new Vector3(0.0f, (toRet.Height * 0.5f), 0.0f);
                else if (alignment == TextureAlignment.eCenterRight)
                    newPos -= new Vector3(toRet.Width, (toRet.Height * 0.5f), 0.0f);

                // Reposition accordingly
                toRet.SetPos(newPos);

                if (!m_Textures.ContainsValue(toRet))
                {
                    // Add to the render textures
                    m_Textures.Add(toRet.Name, toRet);
                }

                // Run any post show code ( such as collision box movement )
                toRet.OnShow();
            }

            return toRet;
        }

        public UIObject AddTextObject(String name, String inText, Vector2 position, Color color, float scale)
        {
            UIObject text = new Text(name, inText, color, position, scale);

            if (text != null)
            {
                if (!m_Textures.ContainsValue(text))
                {
                    if (m_Textures.ContainsKey(text.Name))
                    {
                        int index = m_Textures.IndexOfKey(text.Name);
                        text = m_Textures.Values[index];
                    }
                    else
                    {
                        // Add to the render textures
                        m_Textures.Add(text.Name, text);
                    }
                }

                // Run any post show code ( such as collision box movement )
                text.OnShow();
            }

            return text;
        }

        public UIObject AddText3DObject(String name, String text, Vector3 position, Color color, float scale )
        {
            UIObject text3D = new Text3D(name, text, position, color, scale);

            if (text3D != null)
            {
                if (!m_Textures.ContainsValue(text3D))
                {
                    if (m_Textures.ContainsKey(text3D.Name))
                    {
                        int index = m_Textures.IndexOfKey(text3D.Name);
                        text3D = m_Textures.Values[index];
                    }
                    else
                    {
                        // Add to the render textures
                        m_Textures.Add(text3D.Name, text3D);
                    }
                }

                // Run any post show code ( such as collision box movement )
                text3D.OnShow();
            }

            return text3D;
        }

        public void AddTimedUIObject(String name, Vector3 position, float time)
        {
            AddTimedUIObject(name, position, time, TextureAlignment.eTopLeft);
        }

        public void AddTimedUIObject(String name, Vector3 position, float time, TextureAlignment alignment)
        {
            if (m_LoadedTextures.Count > 0)
            {
                int index = m_LoadedTextures.IndexOfKey(name);

                if ((index >= 0) && (index < m_LoadedTextures.Count))
                {
                    UIObject sprite = m_LoadedTextures.Values[index];

                    if (sprite != null)
                    {
                        // Positioning
                        Vector3 newPos = position;

                        if (alignment == TextureAlignment.eCenter)
                            newPos -= new Vector3((sprite.Width * 0.5f), (sprite.Height * 0.5f), 0.0f);

                        // Reposition accordingly
                        sprite.SetPos(newPos);

                        UIObjectTimed timedSprite = new UIObjectTimed(sprite, time);

                        if (!m_Textures.ContainsKey(name) && !m_Textures.ContainsValue(timedSprite))
                        {
                            // Add to the render textures
                            m_Textures.Add(name, timedSprite);
                        }
                    }
                }
            }
        }

        public void FlagToRemoveUIObject(String name)
        {
            if (m_Textures.Count > 0)
            {
                if (m_Textures.ContainsKey(name))
                {
                    int index = m_Textures.IndexOfKey(name);

                    // Run remove code
                    UIObject uiObject = m_Textures.Values[index];

                    if (uiObject != null)
                        uiObject.OnRemove();
                }
            }
        }

        public void FocusUIObject(String name)
        {
            if (m_Textures.Count > 0)
            {
                if (m_Textures.ContainsKey(name))
                {
                    int index = m_Textures.IndexOfKey(name);

                    // Run remove code
                    UIObject uiObject = m_Textures.Values[index];

                    if (uiObject != null)
                        uiObject.Focus();
                }
            }
        }

        public void UnFocusUIObject(String name)
        {
            if (m_Textures.Count > 0)
            {
                if (m_Textures.ContainsKey(name))
                {
                    int index = m_Textures.IndexOfKey(name);

                    // Run remove code
                    UIObject uiObject = m_Textures.Values[index];

                    if (uiObject != null)
                        uiObject.UnFocus();
                }
            }
        }


        public void RemoveUIObject(UIObject uiObject)
        {
            if (m_Textures.Count > 0)
            {
                if (m_Textures.ContainsValue(uiObject))
                {
                    int index = m_Textures.IndexOfValue(uiObject);

                    // Run remove code
                    String key = m_Textures.Keys[index];

                    // Now remove from the textures list
                    m_Textures.Remove(key);
                }
            }
        }

        public UIObject GetUIObject(String name)
        {
            UIObject toRet = null;

            if (name != "")
            {
                int index = m_LoadedTextures.IndexOfKey(name);

                if (index >= 0)
                    toRet = m_LoadedTextures.Values[index];
            }

            return toRet;
        }

        public bool IsShowing(UIObject uiObject)
        {
            return m_Textures.ContainsValue(uiObject);
        }

        public void ChangeTextObject(String name, String text)
        {
            Text textObject = null;

            if (name != "")
            {
                int index = m_Textures.IndexOfKey(name);

                if (index >= 0)
                {
                    textObject = (m_Textures.Values[index] as Text);

                    if (textObject != null)
                        textObject.SetText(text);
                }
            }
        }

        public SpriteBatch SpriteBatch { get { return m_SpriteBatch; } set { m_SpriteBatch = value; } }
    }
}
