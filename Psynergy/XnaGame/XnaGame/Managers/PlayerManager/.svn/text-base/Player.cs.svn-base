using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

/* Graphics Library */
using Psynergy.Graphics;

namespace XnaGame
{
    public class Player : GameNode, IFocusable
    {
        public enum PlayerControl
        {
            Human = 0,
            AI = 1
        };

        protected PlayerIndex m_PlayerIndex = PlayerIndex.One;

        // Player control value
        private PlayerControl m_PlayerControl = PlayerControl.Human;

        public Player() : base()
        {
        }

        public Player(String name) : base()
        {
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        public override void Reset()
        {
        }

        public override void OnSelect()
        {
        }

        public virtual void OnDeselect()
        {
        }

        public override void AddToScene(Scene scene)
        {
            base.AddToScene(scene);
        }

        public override void RemoveFromScene()
        {
            base.RemoveFromScene();
        }

        public IFocusable Focus { get { return this; } }
        public PlayerIndex Index { get { return m_PlayerIndex; } set { m_PlayerIndex = value; } }
        public PlayerControl Control { get { return m_PlayerControl; } set { m_PlayerControl = value; } }
    }
}
