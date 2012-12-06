using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

namespace Psynergy.Graphics
{
    public class Scene : GameObject, IRegister<Scene>
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterVector3("ambient", "AmbientLighting");

            base.ClassProperties(factory);
        }
        #endregion

        private SceneFragment m_Resource = null;
        private Hierarchy m_Hierarchy = null;

        #region Lighting
        private Vector3 m_AmbientLighting = Vector3.Zero;
        #endregion

        public Scene() : base( "" )
        {
            m_Hierarchy = new Hierarchy();
        }

        public Scene( String name ): base(name)
        {
            m_Hierarchy = new Hierarchy(name + ":Hierarchy");
        }

        public Scene(String name, String resource) : base(name)
        {
            m_Hierarchy = new Hierarchy(name + ":Hierarchy");

            if ( resource != null )
                m_Resource = new SceneFragment(resource, m_Hierarchy);
        }

        public override void Initialise()
        {
            // Load resources
            if (m_Resource != null)
                m_Resource.Load();

            // Initialise the hierarchy
            if ( m_Hierarchy != null )
                m_Hierarchy.Initialise();

            // Initialise children
            base.Initialise();
        }

        public override void Load()
        {
            // For now, as it isn't fully deffered yet
            PsynergyRenderer renderer = RenderManager.Instance.ActiveRenderer;
            if (renderer != null)
                renderer.AmbientLighting = m_AmbientLighting;

            // Load resources
            if (m_Hierarchy != null)
                m_Hierarchy.Load();

            // List of deferrables
            List<IDeferrable> deferrables = new List<IDeferrable>();

            // Get any models from the hierarchy and stash them away in a buffer 
            deferrables.AddRange(Hierarchy.FindAll<Light>());
            deferrables.AddRange(Hierarchy.FindAll<RenderNode>());

            if (renderer != null)
                renderer.Defer(deferrables);
        }

        public override void Reset()
        {
            base.Reset();

            // reset the hierarchy
            if (m_Hierarchy != null)
                m_Hierarchy.Reset();
        }

        public override void UnLoad()
        {
            // Load resources
            if (m_Hierarchy != null)
                m_Hierarchy.UnLoad();
        }

        public override void Update(GameTime deltaTime)
        {
            // Load resources
            if (m_Hierarchy != null)
                m_Hierarchy.Update(deltaTime);
        }

        public override void Render(GameTime deltaTime)
        {
            // Load resources
            if (m_Hierarchy != null)
                m_Hierarchy.Render(deltaTime);
        }

        #region Scene Change Management

        #endregion

        #region Render Node Management
        public void Add(RenderNode renderNode)
        {
            if (renderNode != null)
            {
                if (renderNode.RenderGroup != null)
                {
                    // Defer this render node to the renderer
                    PsynergyRenderer renderer = RenderManager.Instance.ActiveRenderer;
                    if (renderer != null)
                        renderer.Defer(renderNode);
                }
            }
        }

        public void Remove(RenderNode renderNode)
        {
            if (renderNode != null)
            {
                // Defer this render node to the renderer
                PsynergyRenderer renderer = RenderManager.Instance.ActiveRenderer;
                if (renderer != null)
                    renderer.RemoveRenderNode(renderNode);

                if (renderNode.Parent != null)
                    renderNode.Parent.RemoveChild(renderNode);
            }
        }
        #endregion

        #region Properties
        public Hierarchy Hierarchy { get { return m_Hierarchy; } }

        // Lighting
        public Vector3 AmbientLighting
        {
            get { return m_AmbientLighting; }
            set
            {
                m_AmbientLighting = Vector3.Clamp(value, Vector3.Zero, Vector3.One);
            }
        }
        #endregion
    }
}
