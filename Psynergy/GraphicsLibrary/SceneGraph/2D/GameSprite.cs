using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Psynergy.Graphics
{
    public class GameSprite : RenderNode, IRegister<GameSprite>
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterClass(typeof(SpriteNode), "SpriteNode", "Sprite");

            base.ClassProperties(factory);
        }
        #endregion

        #region Member Variables
        // Sprite contained within this sprite wrapper
        private SpriteNode m_Sprite = null;

        // A queue of pathing vectors to allow the sprite to move along a path
        private Queue<Vector2> m_PathQueue = new Queue<Vector2>();

        // The location the sprite is currently moving towards
        private Vector2 m_Target = Vector2.Zero;

        // The speed at which the sprite will close with its target
        private float m_MoveSpeed = 1.0f;

        // The two integers represent a clipping range for determining bounding-box style collisions
        // They return the bounding box of the sprite trimmed by a horizontal and vertical offset
        // to get a collision cushion.
        private Vector2 m_CollisionBuffer = Vector2.Zero;

        // Determining the status of the sprite. An inactive sprite will not be updated but will
        // be drawn
        private bool m_UpdateSprite = true;

        // If the sprite should track towards a vector2 target. If set to false, the sprite will not
        // move on its own towards its target, and will not process pathing information
        private bool m_MoveTowardsTarget = true;

        // Determins if the sprite will follow the paths in its path queue. If true, when the sprite has reached
        // its target, the next path node will be pulled from the queue and set as the new target
        private bool m_UsePathing = true;

        // If true, any pathing node popped from the queue will be placed back onto the end of the queue
        private bool m_LoopPath = true;

        // If true, the sprite can collide with other objects. Note that  this is only provided as a 
        // flag for testing with outside code
        private bool m_Collidable = true;

        // If true, the sprite will be deactivated when the pathing queue is empty
        private bool m_DeactivateAtEndOfPath = false;

        // If true, the sprite will be made invisible at the end of its path
        private bool m_HideAtEndOfPath = false;

        // If set, when the patching queue is empty, the named animation will be set as the current
        // animation on the sprite
        private String m_EndPathAnimation = null;
        #endregion

        public GameSprite() : base("")
        {
        }

        public GameSprite(String name) : base(name)
        {
        }

        #region Initialise
        public override void Initialise()
        {
            base.Initialise();

            if (m_Sprite != null)
            {
                m_Sprite.Position = Position;
            }
        }
        #endregion

        #region Load
        public override void Load()
        {
            base.Load();
        }
        #endregion

        #region Update
        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            // Sprite specific update on/off switch so it can be rendered but sitting 
            // idle not updating
            if (m_UpdateSprite)
            {
                if (m_Sprite != null)
                {
                    if (m_MoveTowardsTarget)
                    {
                        if (m_Target != null)
                        {
                            // Get a vector pointing from the current location of the sprite
                            // to the destination
                            Vector3 delta = new Vector3((m_Target.X - m_Sprite.PosX), (m_Target.Y - m_Sprite.PosY), 0);

                            if (delta.Length() > m_MoveSpeed)
                            {
                                delta.Normalize();
                                delta = (delta * (m_MoveSpeed * (float)deltaTime.ElapsedGameTime.TotalSeconds));

                                // Set position accordingly
                                Position += delta;
                            }
                            else
                            {
                                if (m_Target == m_Sprite.Position2D)
                                {
                                    if (m_UsePathing)
                                    {
                                        if (m_PathQueue.Count > 0)
                                        {
                                            m_Target = m_PathQueue.Dequeue();

                                            // Looping so add the path point at the back of the queue
                                            if (m_LoopPath)
                                                m_PathQueue.Enqueue(m_Target);
                                        }
                                        else
                                        {
                                            if (m_EndPathAnimation != null)
                                            {
                                                if (m_Sprite.CurrentAnimation == m_EndPathAnimation)
                                                    m_Sprite.CurrentAnimation = m_EndPathAnimation;
                                            }

                                            if (m_DeactivateAtEndOfPath)
                                                m_UpdateSprite = false;

                                            if (m_HideAtEndOfPath)
                                                ActiveRender = false;
                                        }
                                    }
                                }
                                else
                                {
                                    Position = new Vector3(m_Target.X, m_Target.Y, 0);
                                }
                            }
                        }
                    }

                    // Update the sprite
                    m_Sprite.Update(deltaTime);
                }
            }
        }
        #endregion

        #region Render
        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);
        }
        #endregion

        #region Useful Functions
        public void AddPathNode(Vector2 node)
        {
            m_PathQueue.Enqueue(node);
        }

        public void AddPathNode(int x, int y)
        {
            AddPathNode(new Vector2(x, y));
        }

        public void RemovePath()
        {
            m_PathQueue.Clear();
        }
        #endregion

        #region Property Set / Gets
        public SpriteNode Sprite { get { return m_Sprite; } set { m_Sprite = value; } }
        public Vector2 Target { get { return m_Target; } set { m_Target = value; } }
        public int HorizontalCollisionBuffer { get { return (int)m_CollisionBuffer.X; } set { m_CollisionBuffer.X = value; } }
        public int VerticalCollisionBuffer { get { return (int)m_CollisionBuffer.Y; } set { m_CollisionBuffer.Y = value; } }
        public bool IsPathing { get { return m_UsePathing; } set { m_UsePathing = value; } }
        public bool DeactivateAfterPathing { get { return m_DeactivateAtEndOfPath; } set { m_DeactivateAtEndOfPath = value; } }
        public bool LoopPath { get { return m_LoopPath; } set { m_LoopPath = value; } }
        public String EndPathAnimation { get { return m_EndPathAnimation; } set { m_EndPathAnimation = value; } }
        public bool HideAtEndOfPath { get { return m_HideAtEndOfPath; } set { m_HideAtEndOfPath = value; } }
        public float MoveSpeed { get { return m_MoveSpeed; } set { m_MoveSpeed = value; } }
        public bool UpdateSprite { get { return m_UpdateSprite; } set { m_UpdateSprite = value; } }
        public bool IsMoving { get { return m_MoveTowardsTarget; } set { m_MoveTowardsTarget = value; } }
        public bool IsCollidable { get { return m_Collidable; } set { m_Collidable = value; } }
        public Rectangle BoundingBox 
        { 
            get 
            {
                if (m_Sprite != null)
                    return m_Sprite.BoundingBox;
                else
                    return Rectangle.Empty;
            } 
        }

        public Rectangle CollisionBox
        {
            get
            {
                Rectangle toRet = Rectangle.Empty;

                if (m_Sprite != null )
                {
                    toRet = new Rectangle((m_Sprite.BoundingBox.X + (int)m_CollisionBuffer.X),
                                          (m_Sprite.BoundingBox.Y + (int)m_CollisionBuffer.Y),
                                          ((int)m_Sprite.Width - (2 * (int)m_CollisionBuffer.X)),
                                          ((int)m_Sprite.Height - (2 * (int)m_CollisionBuffer.Y)));
                }

                return toRet;
            }
        }

        public override Vector3 Position
        {
            get { return base.Position; }
            set
            {
                base.Position = value;

                if (m_Sprite != null)
                    m_Sprite.Position = value;
            }
        }
        #endregion
    }
}
