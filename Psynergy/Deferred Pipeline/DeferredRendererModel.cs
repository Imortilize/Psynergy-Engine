using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using TInput = System.String;
using TOutput = System.String;

// Custom skinned model project
using SkinnedModel;

namespace Psynergy.DeferredPipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "Deferred Renderer Model")]
    public class DeferredRendererModel : DeferredRendererBase
    {
        #region Properties
        [DisplayName("Diffuse Map Texture")]
        [Description("If set, this file will be used as the diffuse map on the model, " + "overriding anything found in the opaque data.")]
        [DefaultValue("")]
        public string DiffuseMapTexture
        {
            get { return m_DiffuseMapTexture; }
            set { m_DiffuseMapTexture = value; }
        }

        private string m_DiffuseMapTexture;

        [DisplayName("Diffuse Map Key")]
        [Description("This will be the key that will be used to search the diffuse map in the opaque data of the model")]
        [DefaultValue("DiffuseMap")]
        public string DiffuseMapKey
        {
            get { return m_DiffuseMapKey; }
            set { m_DiffuseMapKey = value; }
        }

        private string m_DiffuseMapKey = "DiffuseMap";

        [DisplayName("Enable Lighting")]
        [Description("Whether to use lighting or not")]
        [DefaultValue(true)]
        public bool EnableLighting
        {
            get { return m_EnableLighting; }
            set { m_EnableLighting = value; }
        }

        private bool m_EnableLighting = true;

        [DisplayName("Enable Normal Map")]
        [Description("Whether to use a normal map or not")]
        [DefaultValue(false)]
        public bool EnableNormalMap
        {
            get { return m_EnableNormalMap; }
            set { m_EnableNormalMap = value; }
        }

        private bool m_EnableNormalMap;

        [DisplayName("Normal Map Texture")]
        [Description("If set, this file will be used as the normal map on the model, " +
        "overriding anything found in the opaque data.")]
        [DefaultValue("")]
        public string NormalMapTexture
        {
            get { return m_NormalMapTexture; }
            set { m_NormalMapTexture = value; }
        }

        private string m_NormalMapTexture;

        [DisplayName("Normal Map Key")]
        [Description("This will be the key that will be used to search the normal map in the opaque data of the model")]
        [DefaultValue("NormalMap")]
        public string NormalMapKey
        {
            get { return m_NormalMapKey; }
            set { m_NormalMapKey = value; }
        }

        private string m_NormalMapKey = "NormalMap";

        [DisplayName("Specular Map Texture")]
        [Description("If set, this file will be used as the specular map on the model, " +
        "overriding anything found in the opaque data.")]
        [DefaultValue("")]
        public string SpecularMapTexture
        {
            get { return m_SpecularMapTexture; }
            set { m_SpecularMapTexture = value; }
        }

        private string m_SpecularMapTexture;

        [DisplayName("Specular Map Key")]
        [Description("This will be the key that will be used to search the specular map in the opaque data of the model")]
        [DefaultValue("SpecularMap")]
        public string SpecularMapKey
        {
            get { return m_SpecularMapKey; }
            set { m_SpecularMapKey = value; }
        }

        private string m_SpecularMapKey = "SpecularMap";

        [Description("If set, this file will be used as the emissive map on the model, " + "overriding anything found in the opaque data.")]
        [DefaultValue("")]
        public string EmissiveMapTexture
        {
            get { return m_EmissiveMapTexture; }
            set { m_EmissiveMapTexture = value; }
        }

        private string m_EmissiveMapTexture;

        [DisplayName("Emissive Map Key")]
        [Description("This will be the key that will be used to search the emissive map in the opaque data of the model")]
        [DefaultValue("EmissiveMap")]
        public string EmissiveMapKey
        {
            get { return m_EmissiveMapKey; }
            set { m_EmissiveMapKey = value; }
        }

        private string m_EmissiveMapKey = "EmissiveMap";

        public string MergeAnimations { get; set; }

        private float degreesX;
        public float DegreesX
        {
            get { return degreesX; }
            set { degreesX = value; }
        }

        private float degreesY;
        public float DegreesY
        {
            get { return degreesY; }
            set { degreesY = value; }
        }

        private float degreesZ;
        public float DegreesZ
        {
            get { return degreesZ; }
            set { degreesZ = value; }
        }

        public override float RotationX
        {
            get
            {
                return base.RotationX;
            }
            set
            {
                DegreesX = value;
                base.RotationX = 0;
            }
        }

        public override float RotationY
        {
            get
            {
                return base.RotationY;
            }
            set
            {
                DegreesY = value;
                base.RotationY = 0;
            }
        }

        public override float RotationZ
        {
            get
            {
                return base.RotationZ;
            }
            set
            {
                DegreesZ = value;
                base.RotationZ = 0;
            }
        }

        private ERenderQueue m_RenderQueue = ERenderQueue.Default;

        public ERenderQueue RenderQueue
        {
            get { return m_RenderQueue; }
            set { m_RenderQueue = value; }
        }

        // Skinned or not?
        protected bool m_IsSkinned = false;

        #region Shadow
        private bool m_CastShadows = false;

        [DisplayName("Cast Shadows")]
        [Description("Whether to cast shadows or not")]
        [DefaultValue(false)]
        public bool CastShadows
        {
            get { return m_CastShadows; } 
            set { m_CastShadows = value; }
        }

        private CullMode m_ShadowCullMode = CullMode.CullCounterClockwiseFace;

        [DisplayName("Shadow Cull Mode")]
        [Description("The cull mode used when drawing to the shadow maps")]
        [DefaultValue(CullMode.CullCounterClockwiseFace)]
        public CullMode ShadowCullMode
        {
            get { return m_ShadowCullMode; } 
            set { m_ShadowCullMode = value; }
        }
        #endregion
        #endregion

        private String m_Directory = "";

        #region Process
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            // Get directory
            m_Directory = Path.GetDirectoryName(input.Identity.SourceFilename);

            // Look up textures
            LookUpTextures(input);

            // merge transforms
            MeshHelper.TransformScene(input, input.Transform);
            input.Transform = Matrix.Identity;

            // Before anything rotate the entire model and animations
            // I use separate animation files so I have to remember to
            // rotated the animations the same way as the 
            // recipient of those animations.
            RotateAll(input, DegreesX, DegreesY, DegreesZ);

            if (!string.IsNullOrEmpty(MergeAnimations))
            {
                foreach (string mergeFile in MergeAnimations.Split(';').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)))
                {
                    MergeAnimation(input, context, mergeFile);
                }
            }

            if (!m_IsSkinned)
            {
                // Not a skinned model so merge transforms for optimisations
                MergeTransforms(input);
            }

            // Chain to the base ModelProcessor class so it can convert the model data.
            ModelContent model = base.Process(input, context);

            // Create model mesh data tag
            MeshMetaData metadata = new MeshMetaData();

            // Normal skinning data
            model = SkinnedData(model, input, context, metadata);

            //gather some information that will be useful in run time
            BoundingBox aabb = new BoundingBox();
            metadata.BoundingBox = ComputeBoundingBox(input, ref aabb, metadata);

            // Return model data
            return model;
        }
        #endregion

        #region Helper Functions

        public static void RotateAll(NodeContent node, float degX, float degY, float degZ)
        {
            Matrix rotate = Matrix.Identity *
                Matrix.CreateRotationX(MathHelper.ToRadians(degX)) *
                Matrix.CreateRotationY(MathHelper.ToRadians(degY)) *
                Matrix.CreateRotationZ(MathHelper.ToRadians(degZ));
            // http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.content.pipeline.graphics.meshhelper.transformscene.aspx
            MeshHelper.TransformScene(node, rotate);
        }


        #endregion

        #region Skinning Functions

        private void MergeAnimation(NodeContent input, ContentProcessorContext context, string mergeFile)
        {
            NodeContent mergeModel = context.BuildAndLoadAsset<NodeContent, NodeContent>(new ExternalReference<NodeContent>(mergeFile), null);

            // Rotate merge file
            RotateAll(mergeModel, DegreesX, DegreesY, DegreesZ);

            BoneContent rootBone = MeshHelper.FindSkeleton(input);

            if (rootBone == null)
            {
                context.Logger.LogWarning(null, input.Identity, "Source model has no root bone.");

                return;
            }

            BoneContent mergeRoot = MeshHelper.FindSkeleton(mergeModel);

            if (mergeRoot == null)
            {
                context.Logger.LogWarning(null, input.Identity, "Merge model '{0}' has no root bone.");

                return;
            }

            int preEndOfFile = mergeFile.LastIndexOf("/");
            String animationKey = "";

            context.Logger.LogImportantMessage("MERGEFILE '{0}'", mergeFile);
            context.Logger.LogImportantMessage("PREENDOFFILE '{0}'", preEndOfFile);

            if (preEndOfFile > -1)
            {
                animationKey = mergeFile.Substring(preEndOfFile + 1);

                // Find the animation name
                int endOfFile = animationKey.IndexOf(".");

                if (endOfFile > -1)
                {
                    animationKey = animationKey.Substring(0, endOfFile);
                }
            }


            foreach (string animationName in mergeRoot.Animations.Keys)
            {
                if (rootBone.Animations.ContainsKey(animationName))
                {
                    context.Logger.LogWarning(null, input.Identity, "CANNOT MERGE ANIAMTION '{0}' FROM '{1}', BECAUSE THIS ANIMATION ALREADY EXISTS.", animationName, mergeFile);

                    //continue;
                }

                context.Logger.LogImportantMessage("MERGING ANIMATION '{0}' FROM '{1}'.", animationName, mergeFile);
                context.Logger.LogImportantMessage("ANIMATION KEY '{0}'.", animationKey);

                if (!rootBone.Animations.ContainsKey(animationKey))
                {
                    rootBone.Animations.Add(animationKey, mergeRoot.Animations[animationName]);

                    context.Logger.LogImportantMessage("ANIMATION COUNT '{0}'.", rootBone.Animations.Count);

                    // Log animation merge
                    context.Logger.LogImportantMessage("ANIMATION '{0}' ADDED!", animationKey);
                }
            }

        }

        private ModelContent SkinnedData(ModelContent model, NodeContent input, ContentProcessorContext context, MeshMetaData metadata)
        {
            // Find the skeleton
            BoneContent skeleton = MeshHelper.FindSkeleton(input);

            if (skeleton != null)
            {
                // Read the bind pose and skeleton hierarchy data.
                IList<BoneContent> bones = MeshHelper.FlattenSkeleton(skeleton);

                // Set up the bone matrices and index lists
                List<Matrix> bindPose = new List<Matrix>();
                List<Matrix> inverseBindPose = new List<Matrix>();
                List<int> skeletonHierarchy = new List<int>();
                Dictionary<String, int> boneIndices = new Dictionary<String, int>();
                Dictionary<String, AnimationClip> animationClips = new Dictionary<String, AnimationClip>();

                // Extract the bind pose transforms, inverse bind pose transforms,
                // and parent bone index of each bone in order
                foreach (BoneContent bone in bones)
                {
                    bindPose.Add(bone.Transform);
                    inverseBindPose.Add(Matrix.Invert(bone.AbsoluteTransform));
                    skeletonHierarchy.Add(bones.IndexOf(bone.Parent as BoneContent));
                    boneIndices.Add(bone.Name, boneIndices.Count);
                }

                // Convert animation data to our runtime format.
                animationClips = ProcessAnimations(skeleton.Animations, bones);

                // Save skinndd data
                metadata.SkinningData = new SkinningData(animationClips, bindPose, inverseBindPose, skeletonHierarchy, boneIndices);
            }

            // Save meta data
            model.Tag = metadata;

            // Return the model with the custom animation data
            return model;
        }


        static Dictionary<String, AnimationClip> ProcessAnimations(AnimationContentDictionary animations, IList<BoneContent> bones)
        {
            // Build up a table mapping bones to indices.
            Dictionary<String, int> boneMap = new Dictionary<String, int>();

            for (int i = 0; i < bones.Count; i++)
                boneMap.Add(bones[i].Name, i);

            Dictionary<String, AnimationClip> animationsClips = new Dictionary<String, AnimationClip>();

            // Convert each animation
            foreach (KeyValuePair<String, AnimationContent> animation in animations)
            {
                // Rotate animation
                //RotateAll(animation.Value);

                // Get animation clip
                AnimationClip processed = ProcessAnimation(animation.Value, boneMap);
                processed.Name = animation.Key;

                // Add the clip
                animationsClips.Add(animation.Key, processed);
            }

            // Return the animations clips
            return animationsClips;
        }

        static AnimationClip ProcessAnimation(AnimationContent animation, Dictionary<String, int> boneMap)
        {
            List<KeyFrame> keyframes = new List<KeyFrame>();

            // For each input animation channel.
            foreach (KeyValuePair<String, AnimationChannel> channel in animation.Channels)
            {
                if (boneMap.ContainsKey(channel.Key))
                {
                    // Look up what bone this channel is controlling.
                    int boneIndex = boneMap[channel.Key];

                    // Convert the keyframe data.
                    foreach (AnimationKeyframe keyframe in channel.Value)
                        keyframes.Add(new KeyFrame(boneIndex, keyframe.Time, keyframe.Transform));
                }

                // Sort the merged frames by time
                keyframes.Sort(CompareKeyframeTimes);
            }

            // Return the new animation clip
            return new AnimationClip(animation.Duration, keyframes);
        }

        static int CompareKeyframeTimes(KeyFrame a, KeyFrame b)
        {
            return a.Time.CompareTo(b.Time);
        }
        #endregion

        #region Optimisations
        private void MergeTransforms(NodeContent input)
        {
            if (input is MeshContent)
            {
                MeshContent mc = (MeshContent)input;
                MeshHelper.TransformScene(mc, mc.Transform);
                mc.Transform = Matrix.Identity;
                MeshHelper.OptimizeForCache(mc);
            }

            foreach (NodeContent c in input.Children)
            {
                MergeTransforms(c);
            }
        }

        private BoundingBox ComputeBoundingBox(NodeContent input, ref BoundingBox aabb, MeshMetaData metadata)
        {
            BoundingBox boundingBox;

            if (input is MeshContent)
            {
                MeshContent mc = (MeshContent)input;
                MeshHelper.TransformScene(mc, mc.Transform);
                mc.Transform = Matrix.Identity;

                // Create bounding box
                boundingBox = BoundingBox.CreateFromPoints(mc.Positions);

                //create sub mesh information
                MeshMetaData.SubMeshMetadata subMeshMetadata = new MeshMetaData.SubMeshMetadata();
                subMeshMetadata.BoundingBox = boundingBox;
                subMeshMetadata.RenderQueue = m_RenderQueue;
                subMeshMetadata.ShadowCullMode = m_ShadowCullMode;
                subMeshMetadata.EnableLighting = m_EnableLighting;
                subMeshMetadata.UseNormalMap = m_EnableNormalMap;
                subMeshMetadata.CastShadows = m_CastShadows;

                // Add sub mesh meta data
                metadata.AddSubMeshMetadata(subMeshMetadata);

                // merge bounding boxes of sub parts
                if (metadata.SubMeshesMetadata.Count > 1)
                    boundingBox = BoundingBox.CreateMerged(boundingBox, aabb);
            }
            else
            {
                boundingBox = aabb;
            }

            // Run through children of node and merge children
            foreach (NodeContent c in input.Children)
            {
                boundingBox = BoundingBox.CreateMerged(boundingBox, ComputeBoundingBox(c, ref boundingBox, metadata));
            }

            return boundingBox;
        }
        #endregion

        #region Look Up Textures
        private void LookUpTextures(NodeContent node)
        {
            MeshContent mesh = node as MeshContent;

            if (mesh != null)
            {
                //this will contatin the path to the normal map texture
                string diffuseMapPath;

                //If the NormalMapTexture property is set, we use that normal map for all meshes in the model.
                //This overrides anything else
                if (!String.IsNullOrEmpty(DiffuseMapTexture))
                {
                    diffuseMapPath = DiffuseMapTexture;
                }
                else
                {
                    //If NormalMapTexture is not set, we look into the opaque data of the model, 
                    //and search for a texture with the key equal to NormalMapKey
                    diffuseMapPath = mesh.OpaqueData.GetValue<string>(DiffuseMapKey, null);
                }

                //if the NormalMapTexture Property was not used, and the key was not found in the model, than normalMapPath would have the value null.
                if (diffuseMapPath == null)
                {
                    //If a key with the required name is not found, we make a final attempt, 
                    //and search, in the same directory as the model, for a texture named 
                    //meshname_n.tga, where meshname is the name of a mesh inside the model.
                    diffuseMapPath = Path.Combine(m_Directory, mesh.Name + "_d.tga");

                    if (!File.Exists(diffuseMapPath))
                    {
                        //if this fails also (that texture does not exist), 
                        //then we use a default texture, named null_normal.tga
                        diffuseMapPath = "../../GraphicsLibrary/GraphicsContent/Models/null_diffuse.tga";
                    }
                }
                else
                {
                    diffuseMapPath = Path.Combine(m_Directory, diffuseMapPath);
                }

                //this will contatin the path to the normal map texture
                string normalMapPath;

                //If the NormalMapTexture property is set, we use that normal map for all meshes in the model.
                //This overrides anything else
                if (!String.IsNullOrEmpty(NormalMapTexture))
                {
                    normalMapPath = NormalMapTexture;
                }
                else
                {
                    //If NormalMapTexture is not set, we look into the opaque data of the model, 
                    //and search for a texture with the key equal to NormalMapKey
                    normalMapPath = mesh.OpaqueData.GetValue<string>(NormalMapKey, null);
                }

                //if the NormalMapTexture Property was not used, and the key was not found in the model, than normalMapPath would have the value null.
                if (normalMapPath == null)
                {
                    //If a key with the required name is not found, we make a final attempt, 
                    //and search, in the same directory as the model, for a texture named 
                    //meshname_n.tga, where meshname is the name of a mesh inside the model.
                    normalMapPath = Path.Combine(m_Directory, mesh.Name + "_n.tga");

                    if (!File.Exists(normalMapPath))
                    {
                        //if this fails also (that texture does not exist), 
                        //then we use a default texture, named null_normal.tga
                        normalMapPath = "../../GraphicsLibrary/GraphicsContent/Models/null_normal.tga";
                    }
                }
                else
                {
                    normalMapPath = Path.Combine(m_Directory, normalMapPath);
                }

                string specularMapPath;

                // If the SpecularMapTexture property is set, we use it
                if (!String.IsNullOrEmpty(SpecularMapTexture))
                {
                    specularMapPath = SpecularMapTexture;
                }
                else
                {
                    //If SpecularMapTexture is not set, we look into the opaque data of the model, 
                    //and search for a texture with the key equal to specularMapKey
                    specularMapPath = mesh.OpaqueData.GetValue<string>(SpecularMapKey, null);
                }
                if (specularMapPath == null)
                {
                    //we search, in the same directory as the model, for a texture named 
                    //meshname_s.tga
                    specularMapPath = Path.Combine(m_Directory, mesh.Name + "_s.tga");

                    if (!File.Exists(specularMapPath))
                    {
                        //if this fails also (that texture does not exist), 
                        //then we use a default texture, named null_specular.tga
                        specularMapPath = "../../GraphicsLibrary/GraphicsContent/Models/null_specular.tga";
                    }
                }
                else
                {
                    specularMapPath = Path.Combine(m_Directory, specularMapPath);
                }

                string emissiveMapPath;

                // If the SpecularMapTexture property is set, we use it
                if (!String.IsNullOrEmpty(EmissiveMapTexture))
                {
                    emissiveMapPath = EmissiveMapTexture;
                }
                else
                {
                    //If SpecularMapTexture is not set, we look into the opaque data of the model, 
                    //and search for a texture with the key equal to specularMapKey
                    emissiveMapPath = mesh.OpaqueData.GetValue<string>(EmissiveMapKey, null);
                }

                if (emissiveMapPath == null)
                {
                    //we search, in the same directory as the model, for a texture named 
                    //meshname_s.tga
                    emissiveMapPath = Path.Combine(m_Directory, mesh.Name + "_e.tga");

                    if (!File.Exists(emissiveMapPath))
                    {
                        //if this fails also (that texture does not exist), 
                        //then we use a default texture, named null_specular.tga
                        emissiveMapPath = "../../GraphicsLibrary/GraphicsContent/Models/null_emissive.tga";
                    }
                }
                else
                {
                    emissiveMapPath = Path.Combine(m_Directory, emissiveMapPath);
                }

                //No matter what key we searched for in the model (specified through NormalMapKey)
                //from this point forward, we will name it "NormalMap". This is what our shaders will expect
                foreach (GeometryContent geometry in mesh.Geometry)
                {
                    // DIFFUSE
                    if (geometry.Material.Textures.ContainsKey(DiffuseMapKey))
                    {
                        ExternalReference<TextureContent> texRef = geometry.Material.Textures[DiffuseMapKey];
                        geometry.Material.Textures.Remove(DiffuseMapKey);
                        geometry.Material.Textures.Add("DiffuseMap", texRef);
                    }
                    else
                        geometry.Material.Textures.Add("DiffuseMap", new ExternalReference<TextureContent>(diffuseMapPath));

                    //in some .fbx files, the key might be found in the textures collection, but not
                    //in the mesh, as we checked above. If this is the case, we need to get it out, and
                    //add it with the "NormalMap" key
                    if (geometry.Material.Textures.ContainsKey(NormalMapKey))
                    {
                        ExternalReference<TextureContent> texRef = geometry.Material.Textures[NormalMapKey];
                        geometry.Material.Textures.Remove(NormalMapKey);
                        geometry.Material.Textures.Add("NormalMap", texRef);
                    }
                    else
                        geometry.Material.Textures.Add("NormalMap", new ExternalReference<TextureContent>(normalMapPath));

                    // SPECULAR
                    if (geometry.Material.Textures.ContainsKey(SpecularMapKey))
                    {
                        ExternalReference<TextureContent> texRef = geometry.Material.Textures[SpecularMapKey];
                        geometry.Material.Textures.Remove(SpecularMapKey);
                        geometry.Material.Textures.Add("SpecularMap", texRef);
                    }
                    else
                        geometry.Material.Textures.Add("SpecularMap", new ExternalReference<TextureContent>(specularMapPath));

                    // Emissive
                    if (geometry.Material.Textures.ContainsKey(EmissiveMapKey))
                    {
                        ExternalReference<TextureContent> texRef = geometry.Material.Textures[EmissiveMapKey];
                        geometry.Material.Textures.Remove(EmissiveMapKey);
                        geometry.Material.Textures.Add("EmissiveMap", texRef);
                    }
                    else
                        geometry.Material.Textures.Add("EmissiveMap", new ExternalReference<TextureContent>(emissiveMapPath));

                }
            }


            // go through all children and apply LookUpTextures recursively
            foreach (NodeContent child in node.Children)
            {
                LookUpTextures(child);
            }
        }
        #endregion

        #region MaterialConverter
        protected override MaterialContent ConvertMaterial(MaterialContent material, ContentProcessorContext context)
        {
            EffectMaterialContent deferredShadingMaterial = new EffectMaterialContent();
            deferredShadingMaterial.Effect = new ExternalReference<EffectContent>("../../GraphicsLibrary/GraphicsContent/Shaders/Deferred Rendering/RenderGBuffer.fx");
            deferredShadingMaterial.CompiledEffect = context.BuildAsset<EffectContent, CompiledEffectContent>(deferredShadingMaterial.Effect, "EffectProcessor");

            // copy the textures in the original material to the new normal mapping
            // material, if they are relevant to our renderer. The
            // LookUpTextures function has added the normal map and specular map
            // textures to the Textures collection, so that will be copied as well.
            foreach (KeyValuePair<String, ExternalReference<TextureContent>> texture in material.Textures)
            {
                if (texture.Key.Contains("DiffuseMap") || texture.Key.Contains("Texture"))
                    deferredShadingMaterial.Textures.Add(texture.Key, texture.Value);

                if (texture.Key.Contains("NormalMap"))
                    deferredShadingMaterial.Textures.Add(texture.Key, texture.Value);

                if (texture.Key.Contains("SpecularMap"))
                    deferredShadingMaterial.Textures.Add(texture.Key, texture.Value);

                if (texture.Key.Contains("EmissiveMap"))
                    deferredShadingMaterial.Textures.Add(texture.Key, texture.Value);
            }

            //extract the extra parameters
            ExtractDefines(deferredShadingMaterial, material, context);

            // Return material
            return context.Convert<MaterialContent, MaterialContent>(deferredShadingMaterial, typeof(DeferredRendererMaterialProcessor).Name);
        }
        #endregion

        #region Setting Defines to our mesh shaders
        /// <summary>
        /// Extract any defines we need from the original material, like alphaMasked, fresnel, reflection, etc, and pass it into
        /// the opaque data
        /// </summary>
        /// <param name="deferredMaterial"></param>
        /// <param name="material"></param>
        /// <param name="context"></param>
        private void ExtractDefines(EffectMaterialContent deferredMaterial, MaterialContent material, ContentProcessorContext context)
        {
            string defines = "";

            /*if (material.OpaqueData.ContainsKey("alphaMasked") && material.OpaqueData["alphaMasked"].ToString() == "True")
            {
                context.Logger.LogMessage("Alpha masked material found");
                lppMaterial.OpaqueData.Add("AlphaReference", (float)material.OpaqueData["AlphaReference"]);
                defines += "ALPHA_MASKED;";
            }*/

            if (m_IsSkinned)
            {
                context.Logger.LogMessage("Skinned mesh found");
                defines += "SKINNED_MESH;";
            }

            if (!String.IsNullOrEmpty(defines))
                deferredMaterial.OpaqueData.Add("Defines", defines);
        }
        #endregion
    }
}