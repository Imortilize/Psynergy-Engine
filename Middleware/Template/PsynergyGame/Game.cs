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

/* PSYNERGY */
using Psynergy;
using Psynergy.Graphics;
using Psynergy.Sound;
using Psynergy.Physics;

namespace $safeprojectname$
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Psynergy Engine
        private PsynergyEngine m_PsynergyEngine = null;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);

            // Create Psynergy Engine
            m_PsynergyEngine = new PsynergyEngine(this, graphics);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Could cause performance drop but ensures that draw is called after update
            IsFixedTimeStep = true;

            // Set mouse visible for now.
            this.IsMouseVisible = true;

            // THESE CURRENTLY COME BEFORE THE BASE INITIALISE CALL
            if (m_PsynergyEngine != null)
            {
                // Set what sound and physics engine to use
                m_PsynergyEngine.SetSoundEngine(SoundEngineType.XACT);
                m_PsynergyEngine.SetPhysicsEngine(PhysicsEngineType.eJibLibX);
                m_PsynergyEngine.SetParticleEngine(ParticleEngineType.Mercury);

                // Initial Graphics Settings
                m_PsynergyEngine.useShadows(true);
                m_PsynergyEngine.SetAntiAliasing(4);
                m_PsynergyEngine.SetFogProperties(new FogProperties(Color.Black, 200, 900));
            }

            base.Initialize();

            // THESE CURRENTLY COME AFTER THE BASE INITIALISE CALL
            if (m_PsynergyEngine != null)
            {
                // Post Processing
                m_PsynergyEngine.UsePostProcessing(true);
                m_PsynergyEngine.SetDepthOfFieldProperties(new DepthOfFieldProperties(100, 300, 1));
                m_PsynergyEngine.SetBloomProperties(new BloomProperties(0.7f, 1.2f));
                m_PsynergyEngine.SetFilmGrainProperties(new FilmGrainProperties(0.1f, 1, 2));

                // Show FPS
                m_PsynergyEngine.ShowFPS(true);
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
