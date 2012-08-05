using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Psynergy.Graphics
{
    public class FrameAnimation : ICloneable
    {
        #region Member Variables
        // The first frame of the animation. We will calculate other frames
        // on the fly based on this frame.
        private Rectangle m_InitialFrame;

        // Number of frames in the animation
        private int m_FrameCount = 1;

        // The frame currently being displayed
        // This value ranges from 0 to (m_FrameCount - 1)
        private int m_CurrentFrame = 0;

        // Amount of time ( int seconds ) to display each frame
        private float m_FrameLength = 0.2f;

        // Amount of time that has passed since we last animated
        private float m_FrameTimer = 0.0f;

        // The number of times this animation has been played.
        private int m_PlayCount = 0; 

        // The animation that should be played after this animation
        private String m_NextAnimation = null;
        #endregion

        #region Constructors
        public FrameAnimation(Rectangle firstFrame, int frames)
        {
            m_InitialFrame = firstFrame;
            m_FrameCount = frames;
        }

        public FrameAnimation(int x, int y, int width, int height, int frames)
        {
            m_InitialFrame = new Rectangle(x, y, width, height);
            m_FrameCount = frames;
        }

        public FrameAnimation(int x, int y, int width, int height, int frames, float frameLength)
        {
            m_InitialFrame = new Rectangle(x, y, width, height);
            m_FrameCount = frames;
            m_FrameLength = frameLength;
        }

        public FrameAnimation(int x, int y, int width, int height, int frames, float frameLength, String nextAnimation)
        {
            m_InitialFrame = new Rectangle(x, y, width, height);
            m_FrameCount = frames;
            m_FrameLength = frameLength;
            m_NextAnimation = nextAnimation;
        }
        #endregion

        #region Update
        public void Update(GameTime deltaTime)
        {
            m_FrameTimer += (float)deltaTime.ElapsedGameTime.TotalSeconds;

            if (m_FrameTimer > m_FrameLength)
            {
                m_FrameTimer = 0.0f;
                m_CurrentFrame = (m_CurrentFrame + 1) % m_FrameCount;

                if (m_CurrentFrame == 0)
                    m_PlayCount = (int)MathHelper.Min((m_PlayCount + 1), int.MaxValue);
            }
        }
        #endregion

        #region Clone Interface
        public object Clone()
        {
            return new FrameAnimation(m_InitialFrame.X, m_InitialFrame.Y,
                                      m_InitialFrame.Width,
                                      m_InitialFrame.Height,
                                      m_FrameCount,
                                      m_FrameLength,
                                      m_NextAnimation);
        }
        #endregion

        #region Property Set/Gets
        public int FrameCount { get { return m_FrameCount; } set { m_FrameCount = value; } }

        // The time ( in seconds ) to display each frame
        public float FrameLength { get { return m_FrameLength; } set { m_FrameLength = value; } }

        // The frame number current being displayed
        public int CurrentFrame { get { return m_CurrentFrame; } set { m_CurrentFrame = (int)MathHelper.Clamp(value, 0, (m_FrameCount - 1)); } }
        public int FrameWidth { get { return m_InitialFrame.Width; } }
        public int FrameHeight { get { return m_InitialFrame.Height; } }

        // The rectangle associated with the current animation frame.
        public Rectangle FrameRectangle
        {
            get
            {
                return new Rectangle(m_InitialFrame.X + (m_InitialFrame.Width * m_CurrentFrame),
                                     m_InitialFrame.Y,
                                     m_InitialFrame.Width,
                                     m_InitialFrame.Height);
            }
        }

        public int PlayCount
        {
            get { return m_PlayCount; }
            set { m_PlayCount = value; }
        }

        public String NextAnimation
        {
            get { return m_NextAnimation; }
            set { m_NextAnimation = value; }
        }
        #endregion
    }
}
