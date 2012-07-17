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
    public class AnimationClip
    {
        public AnimationClip(TimeSpan duration, List<KeyFrame> keyframes)
        {
            this.Duration = duration;
            this.KeyFrames = keyframes;
        }

        private AnimationClip()
        {
        }

        // Total length of the clip
        [ContentSerializer]
        public TimeSpan Duration { get; private set; }

        // List of keyframes for all bones, sorted by time
        [ContentSerializer]
        public List<KeyFrame> KeyFrames { get; private set; }

        public String Name { get; set; }
    }
}
