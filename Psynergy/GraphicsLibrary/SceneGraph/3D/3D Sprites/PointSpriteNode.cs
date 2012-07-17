using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/* Camera Library */
using Psynergy.Camera;

namespace Psynergy.Graphics
{
    public class PointSpriteNode : SpriteNode
    {
        private Effect m_PointSpriteEffect = null;
        private bool m_EnsureOcclusion = true;

        public PointSpriteNode() : base()
        {
        }

        public PointSpriteNode(String name, String assetName, Vector3 pos) : base(name, assetName, pos)
        {         
        }

        public override void Initialise()
        {
            base.Initialise();

            RenderGroupName = "default";
        }

        public override void Load()
        {
            base.Load();

            // Load health sprite
            m_PointSpriteEffect = RenderManager.Instance.LoadEffect("pointsprite");
            m_PointSpriteEffect.Parameters["xWorld"].SetValue(Matrix.Identity);

            if (GetDefaultTexture() != null)
                m_PointSpriteEffect.Parameters["Texture"].SetValue(GetDefaultTexture());

            m_PointSpriteEffect.Parameters["xPointSpriteSizeWidth"].SetValue(1.0f);
            m_PointSpriteEffect.Parameters["xPointSpriteSizeHeight"].SetValue(1.0f);
        }

        public override void Render(GameTime deltaTime)
        {
            // Render point sprite
            /*BaseCamera camera = RenderManager.Instance.GetActiveCamera();

            if ((camera != null) && ((camera.GetType() == typeof(Camera3D)) || camera.GetType().IsSubclassOf(typeof(Camera3D))))
            {
                Camera3D camera3D = (camera as Camera3D);

                // Just adding the one point here
                VertexPositionTexture[] point = new VertexPositionTexture[6];
                int i = 0;

                point[i++] = new VertexPositionTexture(Position, new Vector2(1, 1));
                point[i++] = new VertexPositionTexture(Position, new Vector2(0, 0));
                point[i++] = new VertexPositionTexture(Position, new Vector2(1, 0));

                point[i++] = new VertexPositionTexture(Position, new Vector2(1, 1));
                point[i++] = new VertexPositionTexture(Position, new Vector2(0, 1));
                point[i++] = new VertexPositionTexture(Position, new Vector2(0, 0));

                // Update texture
                if (GetDefaultTexture() != null)
                    m_PointSpriteEffect.Parameters["Texture"].SetValue(GetDefaultTexture());

                m_PointSpriteEffect.Parameters["xPointSpriteSizeWidth"].SetValue(5.4f);
                m_PointSpriteEffect.Parameters["xPointSpriteSizeHeight"].SetValue(4.73f);
                m_PointSpriteEffect.Parameters["xView"].SetValue(camera3D.View);
                m_PointSpriteEffect.Parameters["xProjection"].SetValue(camera3D.Projection);
                m_PointSpriteEffect.Parameters["xCamPos"].SetValue(camera3D.Position);
                m_PointSpriteEffect.Parameters["xCamUp"].SetValue(camera3D.Up);

                // Set to alpha blend
                RenderManager.Instance.SetBlendState( BlendState.AlphaBlend );

                if (m_EnsureOcclusion)
                {
                    DrawOpaquePixels(point);
                    DrawTransparentPixels(point);
                }
                else
                {
                    RenderManager.Instance.SetDepthStencilState( DepthStencilState.DepthRead );

                    // No alpha test
                    m_PointSpriteEffect.Parameters["xAlphaTest"].SetValue(false);

                    // Draw the billboards
                    DrawPointSprite(point);
                }

                m_PointSpriteEffect.Parameters["xAlphaTest"].SetValue(true);
                m_PointSpriteEffect.Parameters["xAlphaTestGreater"].SetValue(true);

                // Reset the alpha blend and depth read
                RenderManager.Instance.SetBlendState( BlendState.Opaque );
                RenderManager.Instance.SetDepthStencilState( DepthStencilState.Default );
            }*/
        }

        private void DrawOpaquePixels( VertexPositionTexture[] point )
        {
            RenderManager.Instance.SetDepthStencilState( DepthStencilState.Default );

            m_PointSpriteEffect.Parameters["xAlphaTest"].SetValue(true);
            m_PointSpriteEffect.Parameters["xAlphaTestGreater"].SetValue(true);

            // Draw billboards
            DrawPointSprite(point);
        }

        private void DrawTransparentPixels( VertexPositionTexture[] point )
        {
            RenderManager.Instance.SetDepthStencilState( DepthStencilState.DepthRead );

            m_PointSpriteEffect.Parameters["xAlphaTest"].SetValue(true);
            m_PointSpriteEffect.Parameters["xAlphaTestGreater"].SetValue(false);

            // Draw billboards
            DrawPointSprite(point);
        }

        private void DrawPointSprite( VertexPositionTexture[] point )
        {
            m_PointSpriteEffect.CurrentTechnique.Passes[0].Apply();

            // Draw the billboards
            RenderManager.Instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, point, 0, 2);
        }
    }
}
