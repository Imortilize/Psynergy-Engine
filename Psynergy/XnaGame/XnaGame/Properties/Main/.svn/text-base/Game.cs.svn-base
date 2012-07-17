using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace XnaGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager m_Graphics;
        SpriteBatch m_SpriteBatch;

        // Singleton managers
        StateManager m_StateManager = null;
        RenderManager m_RenderManager = null;
        SceneManager m_SceneManager = null;

        public Game()
        {
            m_Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Create the state manager
            m_StateManager = new StateManager( "GameStateManager" );

            // Create and set up the render manager
            m_RenderManager = new RenderManager();
            m_RenderManager.ContentManager = this.Content;
            m_RenderManager.GraphicsDeviceManager = m_Graphics;

            // Scene Manager
            m_SceneManager = new SceneManager();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            m_StateManager.Initialise();
            m_RenderManager.Initialise();
            m_SceneManager.Initialise();

            // Initialise base content
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            m_SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Set render manager sprite batch
            m_RenderManager.SpriteBatch = m_SpriteBatch;

            // Load the state manager objects
            m_StateManager.Load();
            m_SceneManager.Load();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            m_StateManager.UnLoad();
            m_RenderManager.UnLoad();
            m_SceneManager.UnLoad();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Update game
            m_StateManager.Update(gameTime);
            m_RenderManager.Update(gameTime);

            // Update base 
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // render currentScene
            m_RenderManager.Render(gameTime);

            // Update base draw
            base.Draw(gameTime);
        }
    }
}
