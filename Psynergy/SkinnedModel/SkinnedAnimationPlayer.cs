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
    public class SkinnedAnimationPlayer
    {
        private SkinningData m_SkinningData;
 
        public SkinnedAnimationPlayer(SkinningData skinningData)
        {
            m_SkinningData = skinningData;

            BoneTransforms = new Matrix[skinningData.BindPose.Count];
            WorldTransforms = new Matrix[skinningData.BindPose.Count];
            SkinTransforms = new Matrix[skinningData.BindPose.Count];

            Done = true;
        }

        /* Start plating the entirety of the given clip */
        public void StartClip( String clip, bool loop )
        {
            AnimationClip clipVal = null;

            Debug.Assert(m_SkinningData.AnimationsClips.ContainsKey(clip), "[ Animation Warning ] - Skinningdata does not contain the clip " + clip + " so hasn't played!");

            if (m_SkinningData.AnimationsClips.ContainsKey(clip))
            {
                clipVal = m_SkinningData.AnimationsClips[clip];

                // Call the next start clip function
                StartClip(clip, TimeSpan.FromSeconds(0), clipVal.Duration, loop);
            }
        }

        /* Plays a specific portion of the given clip, from on frame index to another */
        public void StartClip(String clip, int startFrame, int endFrame, bool loop)
        {
            Debug.Assert(m_SkinningData.AnimationsClips.ContainsKey(clip), "[ Animation Warning ] - Skinningdata does not contain the clip " + clip + " so hasn't played!");

            if (m_SkinningData.AnimationsClips.ContainsKey(clip))
            {
                // Set the current clip and relevant variables
                AnimationClip clipVal = m_SkinningData.AnimationsClips[clip];

                // Call the next start clip function
                StartClip(clip, clipVal.KeyFrames[startFrame].Time, clipVal.KeyFrames[endFrame].Time, loop);


            }
        }
        
        /* Plays a specific portion of the given clip, from one time to another */
        public void StartClip(String clip, TimeSpan startTime, TimeSpan endTime, bool loop)
        {
            Debug.Assert(m_SkinningData.AnimationsClips.ContainsKey(clip), "[ Animation Warning ] - Skinningdata does not contain the clip " + clip + " so hasn't played!");

            if (m_SkinningData.AnimationsClips.ContainsKey(clip))
            {
                CurrentClip = m_SkinningData.AnimationsClips[clip];
                Done = false;

                m_CurrentTime = TimeSpan.FromSeconds(0);
                m_CurrentKeyFrame = 0;
                m_StartTime = startTime;
                m_EndTime = endTime;
                m_Loop = loop;

                // Copy the bind pose to the bone transforms array to reset the animation
                m_SkinningData.BindPose.CopyTo(BoneTransforms, 0);
            }
        }

        /* Update animation player */
        public void Update(TimeSpan time, Matrix rootTransform)
        {
            if ((CurrentClip == null) || Done)
                return;

            // Commit updates to animation transforms
            UpdateBoneTransforms(time);
            UpdateWorldTransforms(rootTransform, BoneTransforms);
            UpdateSkinTransforms();
        }

        // Helper used by the update method to refresh the BoneTransforms data
        public void UpdateBoneTransforms(TimeSpan time)
        {
            // Increate current animation time
            m_CurrentTime += time;

            // If the current time has passed the end of the animation...
            while (m_CurrentTime >= (m_EndTime - m_StartTime))
            {
                // If we are looping, reduce the time until we are back in the animation's time frame
                if (m_Loop)
                {
                    m_CurrentTime -= (m_EndTime - m_StartTime);
                    m_CurrentKeyFrame = 0;
                    m_SkinningData.BindPose.CopyTo(BoneTransforms, 0);
                }
                // Otherwise, clamp to the end of the animation
                else
                {
                    Done = true;
                    m_CurrentTime = m_EndTime;

                    break;
                }
            }

            // Read keyframe matrices
            IList<KeyFrame> keyframes = CurrentClip.KeyFrames;

            // Read keyframes until we have found the latest frame before the current item.
            while (m_CurrentKeyFrame < keyframes.Count)
            {
                KeyFrame keyframe = keyframes[m_CurrentKeyFrame];

                // Stop when we've read up to the current time position.
                if (keyframe.Time > (m_CurrentTime + m_StartTime))
                    break;

                // Use this transform
                BoneTransforms[keyframe.Bone] = keyframe.Transform;

                // Increase the current keyframe
                m_CurrentKeyFrame++;
            }
        }

        // Helper used by the update method to refresh the WorldTransforms data
        public void UpdateWorldTransforms(Matrix rootTransform, Matrix[] boneTransforms)
        {
            // Root bone
            WorldTransforms[0] = (boneTransforms[0] * rootTransform);

            // For each child bone
            for (int bone = 1; bone < WorldTransforms.Length; bone++)
            {
                // Add the transform of the parent bone
                int parentBone = m_SkinningData.SkeletonHierarchy[bone];

                WorldTransforms[bone] = (boneTransforms[bone] * WorldTransforms[parentBone]);
            }
        }

        // Helper used by the update method to refresh the SkinTransforms data
        public void UpdateSkinTransforms()
        {
            for (int bone = 0; bone < SkinTransforms.Length; bone++)
                SkinTransforms[bone] = (m_SkinningData.InverseBonePose[bone] * WorldTransforms[bone]);
        }

        public Matrix[] GetBoneTransforms()
        {
            return BoneTransforms;
        }

        public void Reset()
        {
            Stop();
        }

        public void Stop()
        {
            CurrentClip = null;
            Done = true;
            m_StartTime = TimeSpan.Zero;
            m_EndTime = TimeSpan.Zero;
            m_CurrentTime = TimeSpan.Zero;
            m_Loop = false;
            m_CurrentKeyFrame = 0;
            m_SkinningData.BindPose.CopyTo(BoneTransforms, 0);
        }

        // The currently playing clip. if there is one
        public AnimationClip CurrentClip { get; private set; }

        // Whether the current animation has finished or not
        public bool Done { get; private set; }

        // Timing values
        private TimeSpan m_StartTime = TimeSpan.Zero;
        private TimeSpan m_EndTime = TimeSpan.Zero;
        private TimeSpan m_CurrentTime = TimeSpan.Zero;
        private bool m_Loop = false;
        private int m_CurrentKeyFrame = 0;

        // Transforms
        public Matrix[] BoneTransforms { get; private set; }
        public Matrix[] WorldTransforms { get; private set; }
        public Matrix[] SkinTransforms { get; private set; }
    }
}
