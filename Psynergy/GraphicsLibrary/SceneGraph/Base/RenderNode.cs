using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

/* Camera Library */
using Psynergy.Camera;

namespace Psynergy.Graphics
{
    public class RenderNode : GameObject, IDeferrable
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterString("texture", "TextureName");

            base.ClassProperties(factory);
        }

        #endregion
        protected GraphicsDevice m_GraphicsDevice = null;

        // Rendergroups
        protected String m_RenderGroupName = "default";
        protected RenderGroup m_RenderGroup = null;

        // Effects
        protected Effect m_CurrentEffect = null;
        protected String m_CurrentEffectTechnique = "";

        // For when effects are cached
        protected Effect m_CachedEffect = null;
        protected String m_CachedEffectTechnique = "";

        #region Texturing
        protected bool m_Textured = false;
        protected List<String> m_TextureNames = new List<String>();
        protected List<Texture> m_Textures = new List<Texture>();
        #endregion

        #region Scene Control
        // Scene added to ( if one exists )
        protected Scene m_SceneAddedTo = null;
        #endregion

        public RenderNode() : base("")
        {
        }

        public RenderNode(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Load()
        {
            base.Load();

            // Get the graphics device from the render manager for easier local usage on 
            // visual nodes.
            Debug.Assert( RenderManager.Instance.GraphicsDevice != null, "[Warning] - Render manager graphics device must not null!" );

            if (RenderManager.Instance.GraphicsDevice != null)
                m_GraphicsDevice = RenderManager.Instance.GraphicsDevice;
        }

        protected virtual void LoadTextures()
        {            
            // Go through all the texture names and try to load them accordingly
            if ((m_TextureNames != null) && (m_TextureNames.Count > 0))
            {
                for (int i = 0; i < m_TextureNames.Count; i++)
                {
                    if (m_TextureNames[i] != null)
                    {
                        // Get this texure and save it to the matching indexed elements
                        m_Textures.Add(RenderManager.Instance.LoadTexture2D(m_TextureNames[i]));
                    }
                }
            }
        }

        // Stores references to all the model's current effects
        public virtual void CacheEffects()
        {
            // Cache the model effect
            m_CachedEffect = m_CurrentEffect;

            // Cache the models current effect technique
            m_CachedEffectTechnique = m_CurrentEffectTechnique;
        }

        // Restores the effects referenced by the model's cache
        public virtual void RestoreEffects()
        {
            // Restore the models previous effect
            m_CurrentEffect = m_CachedEffect;
            m_CachedEffect = null;

            // Restore the models previous effect technique
            m_CurrentEffectTechnique = m_CachedEffectTechnique;
            m_CachedEffectTechnique = "";
        }

        public void SetCurrentEffectTechniqueName(String techniqueName)
        {
            if ((techniqueName != null) && (techniqueName != ""))
                m_CurrentEffectTechnique = techniqueName;
        }

        public void SetEffectTechnique(String techniqueName)
        {
            if (m_CurrentEffect != null)
            {
                if (techniqueName == "")
                    techniqueName = "NoTexture";

                if ( m_CurrentEffect.Techniques[techniqueName] != null )
                    m_CurrentEffect.CurrentTechnique = m_CurrentEffect.Techniques[techniqueName];
            }
        }

        /* Sets the specified effect parameter to the given effect, if it has that parameter */
        protected void SetEffectParameter(Effect effect, string paramName, object val)
        {
            // Debug.Assert(effect.Parameters[paramName] != null, "[Warning] - parameter '" + paramName + "' was not found on effect '" + effect.Name + "'.");

            if (effect != null)
            {
                if (effect.Parameters[paramName] == null)
                    return;

                if (val is int)
                    effect.Parameters[paramName].SetValue((int)val);
                if (val is float)
                    effect.Parameters[paramName].SetValue((float)val);
                if (val is Vector3)
                    effect.Parameters[paramName].SetValue((Vector3)val);
                else if (val is bool)
                    effect.Parameters[paramName].SetValue((bool)val);
                else if (val is Matrix)
                    effect.Parameters[paramName].SetValue((Matrix)val);
                else if (val is Matrix[])
                    effect.Parameters[paramName].SetValue((Matrix[])val);
                else if (val is Texture)
                    effect.Parameters[paramName].SetValue((Texture)val);
            }
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);
        }

        // When a child is removed, check to see if it should be removed from the renderer as well
        protected override void OnChildRemoved(GameObject child, bool result)
        {
            base.OnChildRemoved(child, result);

            // No longer has a parent or siblings
            if (result)
            {
                // If it is a render node, attempt to remove it from the render node list
                if (child.InheritsFrom<RenderNode>())
                {
                    RenderNode renderNode = (child as RenderNode);

                    // For now, as it isn't fully deffered yet
                    DeferredRenderer renderer = (RenderManager.Instance.ActiveRenderer as DeferredRenderer);
                    renderer.RemoveRenderNode(renderNode);
                }
            }
        }

        #region Scene Control
        public virtual void AddToScene(Scene scene)
        {
            if (scene != null)
            {
                // Add this way for now
                scene.Add(this);

                // Save which scene it has been added to
                m_SceneAddedTo = scene;
            }
        }

        public virtual void RemoveFromScene()
        {
            if (m_SceneAddedTo != null)
            {
                // Add this way for now
                m_SceneAddedTo.Remove(this);
                m_SceneAddedTo = null;
            }
        }
        #endregion

        #region New Deferred Renderer
        protected String m_DeferredEffectTechnique = "RenderToGBuffer";

        public void SetDeferredTechnique(String technique)
        {
            m_DeferredEffectTechnique = technique;
        }
        #endregion

        #region Set / Gets
        public String RenderGroupName 
        { 
            get 
            { 
                return m_RenderGroupName; 
            } 
            set 
            { 
                m_RenderGroupName = value;

                if (value != "")
                    RenderGroup = RenderManager.Instance.FindRenderGroup(value);
            } 
        }

        public RenderGroup RenderGroup { get { return m_RenderGroup; } set { m_RenderGroup = value; } }
        public String TextureName { get { return ""; } set { m_TextureNames.Add(value); } }
        public Effect CurrentEffect { get { return m_CurrentEffect; } set { m_CurrentEffect = value; } }
        #endregion
    }
}
