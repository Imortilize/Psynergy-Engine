using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Graphics
{
    public class BaseRenderEffect
    {
        #region Variables
        public static float TotalTime = 0;

        private Effect m_Effect;
        private EffectParameter m_WorldParameter;
        private EffectParameter m_ViewParameter;
        private EffectParameter m_ProjectionParameter;
        private EffectParameter m_WorldViewParameter;
        private EffectParameter m_WorldViewProjectionParameter;
        private EffectParameter m_CameraPositionParameter;
        private EffectParameter m_LightViewProjParameter;
        private EffectParameter m_FarClipParameter;
        private EffectParameter m_LightBufferPixelSizeParameter;
        private EffectParameter m_BonesParameter;
        private EffectParameter m_LightBufferParameter;
        private EffectParameter m_LightSpecularBufferParameter;
        private EffectParameter m_ColorBufferParameter;
        private EffectParameter m_AmbientColorParameter;

        // Fog params
        private EffectParameter m_EnableFogParameter;
        private EffectParameter m_FogStartParameter;
        private EffectParameter m_FogEndParameter;
        private EffectParameter m_FogColorParameter;
        /* */

        // Tone Mapping params
        private EffectParameter m_EnableToneMappingParameter;

        // Normal Map params
        private EffectParameter m_EnableLightingParameter;
        /* */

        // Normal Map params
        private EffectParameter m_EnableNormalBufferParameter;
        private EffectParameter m_NormalBufferParameter;
        /* */

        // Reflection Map params
        private EffectParameter m_DepthBufferParameter;
        private EffectParameter m_ReflectionBufferParameter;
        private EffectParameter m_ClipPlaneParameter;
        private EffectParameter m_UseReflectionClipPlaneParameter;
        private EffectParameter m_InvertViewProjectionParameter;
        /* */

        // Refraction Map params
        private EffectParameter m_RefractionBufferParameter;
        private EffectParameter m_UseRefractionClipPlaneParameter;
        /* */

        // Offset Map params
        private EffectParameter m_OffsetMapParameter;
        private EffectParameter m_ScrollTimeParameter;
        /* */

        #endregion

        public BaseRenderEffect(Effect effect)
        {
            SetEffect(effect);
        }

        public void SetEffect(Effect effect)
        {
            m_Effect = effect;
            ExtractParameters();
        }

        private void ExtractParameters()
        {
            m_WorldParameter = m_Effect.Parameters["xWorld"];
            m_ViewParameter = m_Effect.Parameters["xView"];
            m_ProjectionParameter = m_Effect.Parameters["xProjection"];
            m_WorldViewParameter = m_Effect.Parameters["xWorldView"];
            m_WorldViewProjectionParameter = m_Effect.Parameters["xWorldViewProjection"];
            m_InvertViewProjectionParameter = m_Effect.Parameters["xInvertViewProjection"];
            m_CameraPositionParameter = m_Effect.Parameters["xCameraPosition"];
            m_LightViewProjParameter = m_Effect.Parameters["xLightViewProjection"];
            m_FarClipParameter = m_Effect.Parameters["xMaxDepth"];
            m_LightBufferPixelSizeParameter = m_Effect.Parameters["xHalfPixel"];

            m_BonesParameter = m_Effect.Parameters["xBones"];
            m_EnableNormalBufferParameter = m_Effect.Parameters["xUseNormalMap"];
            m_NormalBufferParameter = m_Effect.Parameters["NormalMap"];
            m_LightBufferParameter = m_Effect.Parameters["LightMap"];
            m_LightSpecularBufferParameter = m_Effect.Parameters["SpecularMap"];
            m_ColorBufferParameter = m_Effect.Parameters["Texture"];
            m_AmbientColorParameter = m_Effect.Parameters["xAmbientColor"];

            m_EnableFogParameter = m_Effect.Parameters["xEnableFog"];
            m_FogStartParameter = m_Effect.Parameters["xFogStart"];
            m_FogEndParameter = m_Effect.Parameters["xFogEnd"];
            m_FogColorParameter = m_Effect.Parameters["xFogColor"];

            m_EnableLightingParameter = m_Effect.Parameters["xEnableLighting"];
            m_EnableToneMappingParameter = m_Effect.Parameters["xToneMap"];

            // Reflection map
            m_DepthBufferParameter = m_Effect.Parameters["DepthMap"];
            m_ReflectionBufferParameter = m_Effect.Parameters["xReflectionMap"];
            m_ClipPlaneParameter = m_Effect.Parameters["xClipPlane"];
            m_UseReflectionClipPlaneParameter = m_Effect.Parameters["xUseReflectionClipPlane"];
        
            // Refraction map
            m_RefractionBufferParameter = m_Effect.Parameters["xRefractionMap"];
            m_UseRefractionClipPlaneParameter = m_Effect.Parameters["xUseRefractionClipPlane"];

            // Offset map
            m_OffsetMapParameter = m_Effect.Parameters["xOffsetMap"];
            m_ScrollTimeParameter = m_Effect.Parameters["xScrollTime"];
        }

        public void SetMatrices(Matrix world, Matrix view, Matrix projection)
        {
            Matrix worldView, worldViewProj;
            Matrix.Multiply(ref world, ref view, out worldView);
            Matrix.Multiply(ref worldView, ref projection, out worldViewProj);

            if (m_WorldParameter != null)
                m_WorldParameter.SetValue(world);

            if (m_ViewParameter != null)
                m_ViewParameter.SetValue(view);

            if (m_ProjectionParameter != null)
                m_ProjectionParameter.SetValue(projection);

            if (m_InvertViewProjectionParameter != null)
                m_InvertViewProjectionParameter.SetValue(Matrix.Invert(view * projection));

            if (m_WorldViewParameter != null)
                m_WorldViewParameter.SetValue(worldView);

            if (m_WorldViewProjectionParameter != null)
                m_WorldViewProjectionParameter.SetValue(worldViewProj);
        }

        public void SetLightViewProj(Matrix matrix)
        {
            if ( m_LightViewProjParameter != null )
                m_LightViewProjParameter.SetValue(matrix);
        }

        public void SetFarClip(float farClip)
        {
            if (m_FarClipParameter != null)
                m_FarClipParameter.SetValue(farClip);
        }

        public void SetLightBufferPixelSize(Vector2 pixelSize)
        {
            if (m_LightBufferPixelSizeParameter != null)
                m_LightBufferPixelSizeParameter.SetValue(pixelSize);
        }

        public void SetBones(Matrix[] bones)
        {
            m_BonesParameter.SetValue(bones);
        }

        public void SetNormalBuffer(Texture normalTarget)
        {
            if (m_NormalBufferParameter != null)
                m_NormalBufferParameter.SetValue(normalTarget);
        }

        public void SetLightBuffer(Texture lightAccumBuffer)
        {
            if (m_LightBufferParameter != null)
                m_LightBufferParameter.SetValue(lightAccumBuffer);
        }

        public void SetLightBufferx(Texture lightAccumBuffer, Texture lightSpecularAccumBuffer)
        {
            if (m_LightBufferParameter != null)
                m_LightBufferParameter.SetValue(lightAccumBuffer);

            if (m_LightSpecularBufferParameter != null)
                m_LightSpecularBufferParameter.SetValue(lightSpecularAccumBuffer);
        }

        public void SetWorld(Matrix globalTransform)
        {
            m_WorldParameter.SetValue(globalTransform);
        }

        public void SetWorldViewProjection(Matrix worldViewProj)
        {
            m_WorldViewProjectionParameter.SetValue(worldViewProj);
        }

        public void SetCameraPosition(Vector3 pos)
        {
            if ( m_CameraPositionParameter != null )
                m_CameraPositionParameter.SetValue(pos);
        }

        public void SetCurrentTechnique(int t)
        {
            m_Effect.CurrentTechnique = m_Effect.Techniques[t];
        }

        public void Apply()
        {
            m_Effect.CurrentTechnique.Passes[0].Apply();
        }

        public EffectParameter GetParameter(string parameter)
        {
            return m_Effect.Parameters[parameter];
        }

        public void SetColorBuffer(Texture2D color)
        {
            if (m_ColorBufferParameter != null)
                m_ColorBufferParameter.SetValue(color);
        }

        public void SetDepthBuffer(Texture2D color)
        {
            if (m_DepthBufferParameter != null)
                m_DepthBufferParameter.SetValue(color);
        }

        public void SetAmbientColor(Vector3 color)
        {
            if (m_AmbientColorParameter != null)
                m_AmbientColorParameter.SetValue(color);
        }

        #region Fog Params
        public void SetFogProperties(FogProperties fogProperties)
        {
            EnableFog(fogProperties.Enabled);
            SetFogStart(fogProperties.Start);
            SetFogEnd(fogProperties.End);
            SetFogColor((fogProperties.EffectColor / 255));
        }

        public void EnableFog(bool enable)
        {
            if (m_EnableFogParameter != null)
                m_EnableFogParameter.SetValue(enable);
        }

        public void SetFogStart(float start)
        {
            if (m_FogStartParameter != null)
                m_FogStartParameter.SetValue(start);
        }

        public void SetFogEnd(float end)
        {
            if (m_FogEndParameter != null)
                m_FogEndParameter.SetValue(end);
        }

        public void SetFogColor(Vector3 color)
        {
            if (m_FogColorParameter != null)
                m_FogColorParameter.SetValue(color);
        }
        #endregion

        #region Lighting Params
        public void EnableLighting(bool enable)
        {
            if (m_EnableLightingParameter != null)
                m_EnableLightingParameter.SetValue(enable);
        }

        #region Tone Mapping
        public void SetToneMappingProperties(ToneMappingProperties toneMapProperties)
        {
            EnableToneMapping(toneMapProperties.Enabled);
        }

        public void EnableToneMapping(bool enable)
        {
            if (m_EnableToneMappingParameter != null)
                m_EnableToneMappingParameter.SetValue(enable);
        }
        #endregion

        #region ReflectionMap
        public void SetReflectionBuffer(Texture2D buffer)
        {
            if (m_ReflectionBufferParameter != null)
                m_ReflectionBufferParameter.SetValue(buffer);
        }

        public void SetReflectionClipPlane(Plane plane)
        {
            if (m_ClipPlaneParameter != null)
            {
                Vector4 planeVec = new Vector4(plane.Normal, plane.D);

                if (planeVec != Vector4.Zero)
                {
                    m_ClipPlaneParameter.SetValue(planeVec);

                    if (m_UseReflectionClipPlaneParameter != null)
                        m_UseReflectionClipPlaneParameter.SetValue(true);
                }
                else
                {
                    m_ClipPlaneParameter.SetValue(Vector4.Zero);

                    if ( m_UseReflectionClipPlaneParameter != null )
                        m_UseReflectionClipPlaneParameter.SetValue(false);
                }
            }
        }
        #endregion    
    
        #region RefractionMap
        public void SetRefractionBuffer(Texture2D buffer)
        {
            if (m_RefractionBufferParameter != null)
                m_RefractionBufferParameter.SetValue(buffer);
        }

        public void SetRefractionClipPlane(Plane plane)
        {
            if (m_ClipPlaneParameter != null)
            {
                Vector4 planeVec = new Vector4(plane.Normal, plane.D);

                if (planeVec != Vector4.Zero)
                {
                    m_ClipPlaneParameter.SetValue(planeVec);

                    if ( m_UseRefractionClipPlaneParameter != null )
                        m_UseRefractionClipPlaneParameter.SetValue(true);
                }
                else
                {
                    m_ClipPlaneParameter.SetValue(Vector4.Zero);

                    if (m_UseRefractionClipPlaneParameter != null)
                        m_UseRefractionClipPlaneParameter.SetValue(false);
                }
            }
        }
        #endregion

        #region OffsetMap
        public void SetOffsetBuffer(Texture2D buffer)
        {
            if (m_OffsetMapParameter != null)
                m_OffsetMapParameter.SetValue(buffer);
        }

        public void SetScrollTime(float time)
        {
            if (m_ScrollTimeParameter != null)
                m_ScrollTimeParameter.SetValue(time);
        }
        #endregion
        #endregion

        #region Normal Map Params
        public void EnableNormalMap(bool enable)
        {
            if (m_EnableNormalBufferParameter != null)
                m_EnableNormalBufferParameter.SetValue(enable);
        }
        #endregion

        public int GetNumTechniques()
        {
            if (m_Effect != null)
                return m_Effect.Techniques.Count;

            return 0;
        }
    }
}
