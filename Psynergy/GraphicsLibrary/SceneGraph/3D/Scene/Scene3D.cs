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
    public class Scene3D : Scene, IRegister<Scene3D>
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterVector3("ambient", "AmbientLighting");

            base.ClassProperties(factory);
        }
        #endregion

        #region Lighting
        private Vector3 m_AmbientLighting = Vector3.Zero;

        // Renderer buffers
        private List<Light> m_LightBuffer = new List<Light>();
        #endregion

        public Scene3D() : base( "" )
        {
        }

        public Scene3D( String name ): base(name)
        {
        }

        public Scene3D(String name, String resource)
            : base(name, resource)
        {
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Load()
        {
            // Get any models from the hierarchy and stash them away in a buffer 
            List<Light> sceneLights = Hierarchy.FindAll<Light>();

            // If any lights were in the scene or created 
            if (sceneLights.Count > 0)
            {
                foreach (Light node in sceneLights)
                    m_LightBuffer.Add(node);
            }

            // For now, as it isn't fully deffered yet
            PsynergyRenderer renderer = RenderManager.Instance.ActiveRenderer;
            if (renderer != null)
            {
                renderer.DeferLightGroup(m_LightBuffer);
                renderer.AmbientLighting = m_AmbientLighting;
            }

            base.Load();
        }

        public override void Render(GameTime deltaTime)
        {
            // Render the scene conventionally now that any pre lighting has been commited
            base.Render(deltaTime);
        }

        public void AddNewRenderNode(RenderNode renderNode)
        {
            if (renderNode != null)
            {
                if (renderNode.RenderGroup != null)
                {
                    // For now, as it isn't fully deffered yet
                    DeferredRenderer renderer = (RenderManager.Instance.ActiveRenderer as DeferredRenderer);

                    // Remove from pre-lighting renderer
                    renderer.DeferRenderable(renderNode);
                }
            }
        }

        public void RemoveRenderNode(RenderNode renderNode)
        {
            if (renderNode != null)
            {
                // For now, as it isn't fully deffered yet
                DeferredRenderer renderer = (RenderManager.Instance.ActiveRenderer as DeferredRenderer);

                // Remove from pre-lighting renderer
                renderer.RemoveRenderNode(renderNode);

                if (renderNode.Parent != null)
                    renderNode.Parent.RemoveChild(renderNode);
            }
        }

        #region Properties
        public Vector3 AmbientLighting 
        { 
            get { return m_AmbientLighting; } 
            set 
            {
                if (value.X < 0.0f)
                    value.X = 0.0f;
                else if (value.X > 1.0f)
                    value.X = 1.0f;

                if (value.Y < 0.0f)
                    value.Y = 0.0f;
                else if (value.Y > 1.0f)
                    value.Y = 1.0f;

                if (value.Z < 0.0f)
                    value.Z = 0.0f;
                else if (value.Z > 1.0f)
                    value.Z = 1.0f;

                m_AmbientLighting = value; 
            } 
        }
        #endregion
    }
}
