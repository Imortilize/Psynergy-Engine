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
    public class Scene3D : Scene
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterVector3("ambient", "AmbientLighting");
            factory.RegisterBool("useprelighting", "UsePreLighting");

            base.ClassProperties(factory);
        }
        #endregion

        #region Lighting
        private bool m_UsePreLighting = false;
        private Vector3 m_AmbientLighting = Vector3.Zero;

        // Renderer buffers
        private List<RenderNode> m_RenderNodeBuffer = new List<RenderNode>();
        private List<ModelNode> m_ModelBuffer = new List<ModelNode>();
        private List<TerrainNode> m_TerrainBuffer = new List<TerrainNode>();
        private List<Light> m_LightBuffer = new List<Light>();
        #endregion

        #region variables
        private TerrainNode m_CurrentTerrain = null;
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
            List<Node> sceneTerrain = Hierarchy.FindAllTerrainNodes();
            List<Node> sceneRenderNodes = Hierarchy.FindAllRenderNodes();
            List<Node> sceneLights = Hierarchy.FindAllLights();

            // If any models were in the scene or created 
            if (sceneRenderNodes.Count > 0)
            {
                foreach (Node node in sceneRenderNodes)
                {
                    m_ModelBuffer.Add(node as ModelNode);
                    m_RenderNodeBuffer.Add(node as RenderNode);
                }
            }

            // Load terrain objects first
            if (sceneTerrain.Count > 0)
            {
                foreach (Node node in sceneTerrain)
                {
                    m_TerrainBuffer.Add(node as TerrainNode);
                    m_RenderNodeBuffer.Add(node as RenderNode);
                }

                m_CurrentTerrain = m_TerrainBuffer[0];
            }

            // If any lights were in the scene or created 
            if (sceneLights.Count > 0)
            {
                foreach (Node node in sceneLights)
                    m_LightBuffer.Add(node as Light);
            }

            // Check if a terrain was loaded
            if ( m_CurrentTerrain != null )
            {
                // Set the new terrian
                TerrainSetEvent terrainSetEvent = new TerrainSetEvent(m_CurrentTerrain);
                terrainSetEvent.Fire();
            }

            // For now, as it isn't fully deffered yet
            PsynergyRenderer renderer = RenderManager.Instance.ActiveRenderer;

            renderer.DeferLightGroup( m_LightBuffer );
            renderer.AmbientLighting = m_AmbientLighting;

            base.Load();
        }

        public override void Render(GameTime deltaTime)
        {
            // Render the scene conventionally now that any pre lighting has been commited
            base.Render(deltaTime);
        }

        public float GetTerrainHeightAtPosition(Vector3 position)
        {
            float toRet = 0.0f;

            // If the current scene terrain exists
            if (m_CurrentTerrain != null)
                toRet = m_CurrentTerrain.GetHeight(position);

            return toRet;
        }

        public float GetTerrainHeightAtPosition(Vector3 position, out float steepness)
        {
            float toRet = 0.0f;
            steepness = 0.0f;

            // If the current scene terrain exists
            if (m_CurrentTerrain != null)
                toRet = m_CurrentTerrain.GetHeight(position, out steepness);

            return toRet;
        }

        public void AddNewRenderNode(RenderNode renderNode)
        {
            if (renderNode != null)
            {
                if (renderNode.RenderGroup != null)
                {
                    if (!m_RenderNodeBuffer.Contains(renderNode))
                        m_RenderNodeBuffer.Add(renderNode);

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
                if (m_RenderNodeBuffer.Contains(renderNode))
                    m_RenderNodeBuffer.Remove(renderNode);

                // For now, as it isn't fully deffered yet
                DeferredRenderer renderer = (RenderManager.Instance.ActiveRenderer as DeferredRenderer);

                // Remove from pre-lighting renderer
                renderer.RemoveRenderNode(renderNode);

                if (renderNode.Parent != null)
                    renderNode.Parent.RemoveChild(renderNode);
            }
        }

        #region Properties
        public bool UsePreLighting { get { return m_UsePreLighting; } set { m_UsePreLighting = value; } }

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

        public TerrainNode CurrentTerrain { get { return m_CurrentTerrain; } }
        #endregion
    }
}
