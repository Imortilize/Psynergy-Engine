using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Psynergy;
using Psynergy.Graphics;
using Psynergy.Graphics.Terrain;
using Psynergy.Menus;
using Psynergy.Input;
using Psynergy.Sound;
using Psynergy.Physics;
using Psynergy.Events;
using Psynergy.AI;

namespace Psynergy
{
    public class PsynergyEngine : DrawableGameComponent
    {
        private Game m_Game = null;
        private GraphicsDeviceManager m_Graphics;
        private SpriteBatch m_SpriteBatch;

        private bool m_UseFpsCounter = false;
        private FPSCounter m_FpsCounter = null;

        public PsynergyEngine(Game game) : base(game)
        {
            // Save the actual game to the engine
            m_Game = game;

            // if the game exists ( Should do ) then add this as a component to the game
            if (m_Game != null)
            {
                m_Game.Components.Add(this);

                // Set graphics device being used
                m_Graphics = new GraphicsDeviceManager(game);
            }

            Debug.Assert( m_Graphics != null, "Graphics Device Manager must not be null to use the Psynergy Engine!");

            if (m_Graphics != null)
            {
                m_Graphics.PreparingDeviceSettings += InitialiseDevice;
                m_Graphics.DeviceResetting += ResettingDevice;
                m_Graphics.DeviceReset += ResetDevice;
            }

            ContentManager content = null;

            // Attain the game content remotely
            if (m_Game != null)
            {
                content = m_Game.Content;
                content.RootDirectory = "Content";
            }

            // Create the managers
            new Factory();
            new RenderManager(content, m_Graphics, new StateMachine<GameObject>("GameStateManager", "Resources/State.xml"));
            new InputManager();
            new SceneManager();
            new MenuManager();
            new EventManager(new EventAggregator(SynchronizationContext.Current));
            new MathLib();
            new UIManager();
            new PhysicsManager();

            // Sound library integration
            new SoundManager(content);

            // Terrain manager ( will be used a lot more later on with paged terrains)
            new TerrainManager();
        }

        private void InitialiseDevice(object sender, PreparingDeviceSettingsEventArgs e)
        {
            PresentationParameters pp =
                e.GraphicsDeviceInformation.PresentationParameters;

            foreach (GraphicsAdapter adapter in GraphicsAdapter.Adapters)
            {             
                if (adapter.Description.Contains("NVIDIA PerfHUD"))
                {
                    GraphicsAdapter.UseReferenceDevice = true;
                    e.GraphicsDeviceInformation.Adapter = adapter;

 
                    break;
                }
            }

            GraphicsAdapter defaultAdapter = GraphicsAdapter.DefaultAdapter;

            //if ( defaultAdapter.IsWideScreen )
                RenderManager.Instance.BaseResolution = new Vector2(1280, 720);
           // else
               // RenderManager.Instance.BaseResolution = new Vector2(1024, 768);
        }

        private void ResettingDevice(object sender, EventArgs e)
        {
            int spug = 0;
        }

        private void ResetDevice(object sender, EventArgs e)
        {
            if ( RenderManager.Instance != null )
            {
                // Reset render targets if needed
                RenderManager.Instance.Reset();
            }
        }

        public void SetRenderEngine(RendererEngineType engineType)
        {
            if (RenderManager.Instance != null)
                RenderManager.Instance.SetEngineType(engineType);
        }

        public void SetSoundEngine(SoundEngineType engineType)
        {
            if ( SoundManager.Instance != null )
                SoundManager.Instance.SetEngineType(engineType);
        }

        public void SetPhysicsEngine(PhysicsEngineType engineType)
        {
            if ( PhysicsManager.Instance != null )
                PhysicsManager.Instance.SelectEngine(engineType);
        }

        public void SetParticleEngine(ParticleEngineType engineType)
        {
            if (RenderManager.Instance != null)
                RenderManager.Instance.SetParticleEngine(engineType);
        }

        public override void Initialize()
        {
            // Set bigger screen size
            if (m_Graphics != null)
            {
                m_Graphics.PreferredBackBufferWidth = (int)RenderManager.Instance.BaseResolution.X;
                m_Graphics.PreferredBackBufferHeight = (int)RenderManager.Instance.BaseResolution.Y;

                // Apply Vsync and multisampling
                m_Graphics.PreferMultiSampling = true;
                m_Graphics.SynchronizeWithVerticalRetrace = true;

                m_Graphics.ApplyChanges();
            }

            // Initialise psynergy engine components
            Factory.Instance.Initialise();

            // This must come second so that  singleton classes don't get broken by the auto even registration
            EventManager.Instance.Initialise();
            /**/
            
            MenuManager.Instance.Initialise();
            RenderManager.Instance.Initialise();

            if (m_Graphics.GraphicsDevice != null)
                InputManager.Instance.ViewPort = m_Graphics.GraphicsDevice.Viewport;

            InputManager.Instance.Initialise();
            UIManager.Instance.Initialise();

            /* Physics Engine */
            // Set engin\e to use and initialise it 
            PhysicsManager.Instance.Initialise();
            /**/

            /* Sound Library */
            SoundManager.Instance.Initialise();
            /**/

            // Terrain handling
            TerrainManager.Instance.Initialise();
    
            if ((m_Game != null) && m_UseFpsCounter)
            {
                // Attach FPS counter
                m_FpsCounter = new FPSCounter(m_Game, "");
                m_Game.Components.Add(m_FpsCounter);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            m_SpriteBatch = new SpriteBatch(GraphicsDevice);
            RenderManager.Instance.SpriteBatch = m_SpriteBatch;
            MenuManager.Instance.SpriteBatch = m_SpriteBatch;
            UIManager.Instance.SpriteBatch = m_SpriteBatch;

            // Set render manager sprite batch
            Factory.Instance.Load();
            EventManager.Instance.Load();
            MenuManager.Instance.Load();
            RenderManager.Instance.Load();
            UIManager.Instance.Load();
            PhysicsManager.Instance.Load();

            /* Sound Library */
            SoundManager.Instance.Load();
            /**/

            // Terrain handling
            TerrainManager.Instance.Load();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            // Terrain handling
            TerrainManager.Instance.UnLoad();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            RenderManager.Instance.UnLoad();
            MenuManager.Instance.UnLoad();
            Factory.Instance.UnLoad();
            UIManager.Instance.UnLoad();
            PhysicsManager.Instance.UnLoad();

            /* Sound Library */
            SoundManager.Instance.UnLoad();
            /**/
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (InputManager.Instance.ExitApplication)
            {
                if ( m_Game != null )
                    m_Game.Exit();
            }

            // Update input manager viewport
            if (GraphicsDevice != null)
                InputManager.Instance.ViewPort = GraphicsDevice.Viewport;

            // Update the input manager
            InputManager.Instance.Update(gameTime);

            // Only update the render items if the game isn't paused
            if (!InputManager.Instance.PauseApplication)
            {
                // Update Render ( camera )
                RenderManager.Instance.Update(gameTime);
            }

            // Update the User Interface
            UIManager.Instance.Update(gameTime);

            // Update menus 
            MenuManager.Instance.Update(gameTime);

            // Update sound manager
            SoundManager.Instance.Update(gameTime);
        }

        public override void  Draw(GameTime gameTime)
        {
            // render game
            RenderManager.Instance.Render(gameTime);

            // Update the User Interface
            //UIManager.Instance.Render(gameTime);

            // render menus
            MenuManager.Instance.Render(gameTime);

            // Render Debug Info
            RenderManager.Instance.RenderDebug(gameTime);
        }

        #region Helper Settings
        public void ShowFPS(bool show)
        {
            m_UseFpsCounter = show;
        }
        #endregion

        #region Graphics Settings
        public void useShadows(bool useShadows)
        {
            RenderManager.Instance.UseShadows(useShadows);
        }

        public void SetFogProperties(FogProperties fogProperties)
        {
            RenderManager.Instance.SetFogProperties(fogProperties);
        }

        public void UsePostProcessing(bool use)
        {
            RenderManager.Instance.UsePostProcesing(use);
        }

        public void SetToneMappingProperties(ToneMappingProperties properties)
        {
            RenderManager.Instance.SetToneMappingProperties(properties);
        }

        public void SetFXAAProperties(FXAAProperties properties)
        {
            RenderManager.Instance.SetFXAAProperties(properties);
        }

        public void SetDepthOfFieldProperties(DepthOfFieldProperties properties)
        {
            RenderManager.Instance.SetDepthOfFieldProperties(properties);
        }

        public void SetBloomProperties(BloomProperties properties)
        {
            RenderManager.Instance.SetBloomProperties(properties);
        }

        public void SetFilmGrainProperties(FilmGrainProperties properties)
        {
            RenderManager.Instance.SetFilmGrainProperties(properties);
        }
        #endregion

        #region Property Set / Get
        #endregion
    }
}
