using System; 
using System.Reflection; 
using System.Collections.Generic; 
 
using XNACore = Microsoft.Xna.Framework;

public static class FixXNACaps
{
    internal static void FixXNACapsStart()
    {
        Assembly xnaAssemly = Assembly.LoadFrom("C:/Program Files/Microsoft XNA/XNA Game Studio/v4.0/References/Windows/x86/Microsoft.Xna.Framework.Graphics.dll");
        Type profileCapabilities = xnaAssemly.GetType("Microsoft.Xna.Framework.Graphics.ProfileCapabilities");
        MethodInfo getIntanceInfo = profileCapabilities.GetMethod("GetInstance", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        object objProfileReach = getIntanceInfo.Invoke(profileCapabilities, new object[] { Microsoft.Xna.Framework.Graphics.GraphicsProfile.Reach });

        FixSeparateAlphaBlend(objProfileReach);
        FixMaxRenderTargets(objProfileReach);
        FixMaxVertexSamplers(objProfileReach);

        //Validate TextureFormat 
        FixValidTextureFormats(objProfileReach);

        FixMaTextureSize(objProfileReach);
        FixMaxCubeSize(objProfileReach);

        //AL.Log.FatalException(objProfileReach.ToString()); 
    }

    internal static void FixSeparateAlphaBlend(object objProfileReach)
    {
        FieldInfo fieldObj = objProfileReach.GetType().GetField("SeparateAlphaBlend", BindingFlags.NonPublic | BindingFlags.Instance);

        fieldObj.SetValue(objProfileReach, true);
    }

    internal static void FixMaxRenderTargets(object objProfileReach)
    {
        FieldInfo fieldObj = objProfileReach.GetType().GetField("MaxRenderTargets", BindingFlags.NonPublic | BindingFlags.Instance);

        fieldObj.SetValue(objProfileReach, 2);
    }


    internal static void FixMaTextureSize(object objProfileReach)
    {
        FieldInfo fieldObj = objProfileReach.GetType().GetField("MaTextureSize", BindingFlags.NonPublic | BindingFlags.Instance);

        fieldObj.SetValue(objProfileReach, 4096);
    }

    internal static void FixMaxCubeSize(object objProfileReach)
    {
        FieldInfo fieldObj = objProfileReach.GetType().GetField("MaxCubeSize", BindingFlags.NonPublic | BindingFlags.Instance);

        fieldObj.SetValue(objProfileReach, 4096);
    }

    internal static void FixMaxVertexSamplers(object objProfileReach)
    {
        FieldInfo fieldObj = objProfileReach.GetType().GetField("MaxVertexSamplers", BindingFlags.NonPublic | BindingFlags.Instance);

        fieldObj.SetValue(objProfileReach, 4);
    }

    internal static void FixValidTextureFormats(object objProfileReach)
    {
        FieldInfo fieldValidate = objProfileReach.GetType().GetField("ValidTextureFormats", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        List<XNACore.Graphics.SurfaceFormat> validateFormats = (List<XNACore.Graphics.SurfaceFormat>)fieldValidate.GetValue(objProfileReach);
        validateFormats.Add(XNACore.Graphics.SurfaceFormat.Alpha8);
        validateFormats.Add(XNACore.Graphics.SurfaceFormat.Rgba1010102);
        validateFormats.Add(XNACore.Graphics.SurfaceFormat.Rg32);
        validateFormats.Add(XNACore.Graphics.SurfaceFormat.Rgba64);
        validateFormats.Add(XNACore.Graphics.SurfaceFormat.Single);

        validateFormats.Add(XNACore.Graphics.SurfaceFormat.Vector2);
        validateFormats.Add(XNACore.Graphics.SurfaceFormat.Vector4);
        validateFormats.Add(XNACore.Graphics.SurfaceFormat.HalfSingle);
        validateFormats.Add(XNACore.Graphics.SurfaceFormat.HalfVector2);
        validateFormats.Add(XNACore.Graphics.SurfaceFormat.HalfVector4);
    }
}