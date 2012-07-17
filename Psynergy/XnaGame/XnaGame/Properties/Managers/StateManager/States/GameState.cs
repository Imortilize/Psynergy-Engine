using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace XnaGame
{
    class GameState : State
    {
        // Game scene
        //private Hierarchy m_Hierarchy = null;

        public GameState( String name ) : base( name )
        {
            // Create a scene
            //m_Hierarchy = new Hierarchy("GameHierarchy");
        }

        public override void Initialise()
        {
            //if (m_Hierarchy != null)
               // m_Hierarchy.Initialise();

            // Add a sprite node to the hierarchy
           // m_Hierarchy.AddChild( new SpriteNode("testPic", "testPic", new Vector2(0.0f, 0.0f)) );
        }

        public override void Load()
        {
            // Load the hierarchy
           // m_Hierarchy.Load();
        }

        public override void Reset()
        {
           // if (m_Hierarchy != null)
             //   m_Hierarchy.Reset();
        }

        public override void Update( GameTime deltaTime )
        {
           // if (m_Hierarchy != null)
              //  m_Hierarchy.Update(deltaTime);
        }

        public override void Render(GameTime deltaTime)
        {
            //if (m_Hierarchy != null)
              //  m_Hierarchy.Render(deltaTime);
        }
    }
}
