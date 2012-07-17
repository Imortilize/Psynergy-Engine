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
    public class KeyFrame
    {
        public KeyFrame(int bone, TimeSpan time, Matrix transform)
        {
            this.Bone = bone;
            this.Time = time;
            this.Transform = transform;
        }

        private KeyFrame()
        {
        }

        // Index of the bone this keyframe animates
        [ContentSerializer]
        public int Bone { get; private set; }

        // Time from the beginning of the animation of this keyframe
        [ContentSerializer]
        public TimeSpan Time { get; private set; }

        // Bone transform for this keyframe
        [ContentSerializer]
        public Matrix Transform { get; private set; }
    }
}
