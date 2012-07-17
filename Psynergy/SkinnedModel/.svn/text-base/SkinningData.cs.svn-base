using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SkinnedModel
{
    public class SkinningData
    {
        // Gets a collection of animation clips, stored by name
        [ContentSerializer]
        public Dictionary<String, AnimationClip> AnimationsClips { get; private set; }

        // Bind pose matrices for each bone in the skeleton, relative to the parent bone.
        [ContentSerializer]
        public List<Matrix> BindPose { get; private set; }

        // Vertex to bonespace transforms for each bone in the skeleton
        [ContentSerializer]
        public List<Matrix> InverseBonePose { get; private set; }

        // For each bone in the skeleton, stores the index of the parent bone.
        [ContentSerializer]
        public List<int> SkeletonHierarchy { get; private set; }

        // Dictionary mapping bone names to their indices in the preceding lists.
        [ContentSerializer]
        public Dictionary<String, int> BoneIndices { get; private set; }

        public SkinningData(Dictionary<String, AnimationClip> animationsClips, List<Matrix> bindPose, List<Matrix> inverseBindPose, List<int> skeletonHierarchy, Dictionary< String, int > boneIndices)
        {
            this.AnimationsClips = animationsClips;
            this.BindPose = bindPose;
            this.InverseBonePose = inverseBindPose;
            this.SkeletonHierarchy = skeletonHierarchy;
            this.BoneIndices = boneIndices;
        }

        private SkinningData()
        {
        }
    }
}
