using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Psynergy;
using Psynergy.Input;
using Psynergy.Events;
using Psynergy.Sound;
using Psynergy.Graphics;

namespace Psynergy.Menus
{
    public class MenuManager : Singleton<MenuManager>, IListener<PauseGameEvent>
    {
        public enum Menus
        {
            Error = -1,
            MainMenu = 0,
            PauseMenu = 1,
            OptionsMenu = 2,
            GameOptions = 3,
            Credits = 4,
            NumPlayersMenu = 5,
            LoadingMenu = 6,
            EndGameMenuPlayer1 = 7,
            EndGameMenuPlayer2 = 8
        };

        // Sprite batch for menu drawing
        private SpriteBatch m_SpriteBatch = null;

        private MenuResource m_Resource = null;
        private List<String> m_MenuChoices = new List<String>();
        private SortedList<String, Menu> m_Menus = new SortedList<String, Menu>();

        private Menu m_PreviousMenu = null;
        private Menu m_CurrentMenu = null;
        private Menu m_NextMenu = null;

        public MenuManager()
        {

        }

        public override void Initialise()
        {
            String filename = "Resources/GameMenus.xml";
            m_Resource = new MenuResource(filename, m_Menus);
            base.Initialise();
        }

        protected override void RegisterEvents()
        {
            base.RegisterEvents();

            // Register to this event
            EventManager.Instance.Subscribe<PauseGameEvent>(this);
        }

        public override void Load()
        {
            // Load the menu resources
            if (m_Resource != null)
                m_Resource.Load();
            
            // Load all the menus that have been loaded in from the resource file and then initialise them
            foreach (Menu menu in m_Menus.Values)
            {
                menu.Load();
                menu.Initialise();
            }
        }

        public override void UnLoad()
        {
            m_Resource = null;
        }

        public override void Reset()
        {
            base.Reset();

            // Reset all the menus that have been loaded in from the resource file and then initialise them
            foreach (Menu menu in m_Menus.Values)
                menu.Reset();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            // If a new menu is being prompted to show.
            if (m_NextMenu != null)
            {
                bool switchMenu = false;

                // If the current menu is now closed ( finished its exit functionality )
                if ((m_CurrentMenu != null) && !m_CurrentMenu.IsOpen())
                {
                    // Save current menu as the previous menu and then change the current menu to the next menu
                    m_PreviousMenu = m_CurrentMenu;

                    // Switch Menu 
                    switchMenu = true;
                }
                else
                {
                    // Switch Menu 
                    switchMenu = true;
                }

                // If the switch menu switch has been flicked then change the menus accordingly
                if (switchMenu)
                {
                    m_CurrentMenu = m_NextMenu;

                    // There is now no next menu
                    m_NextMenu = null;

                    // Call the new menus onEnter function
                    m_CurrentMenu.OnEnter();
                }
            }

            // Render the current menu if there is one to render.
            if ( (m_CurrentMenu != null) && m_CurrentMenu.IsOpen() )
                m_CurrentMenu.Update(deltaTime);
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);

            // Render the current menu if there is one to render.
            if ((m_CurrentMenu != null) && m_CurrentMenu.IsOpen())
            {
                GraphicsDevice device = RenderManager.Instance.GraphicsDevice;
                Vector2 baseResolution = RenderManager.Instance.BaseResolution;
                float horScaling = (float)device.PresentationParameters.BackBufferWidth / baseResolution.X;
                float verScaling = (float)device.PresentationParameters.BackBufferHeight / baseResolution.Y;
                Vector3 screenScalingFactor = new Vector3(horScaling, verScaling, 1);

                // Scaler
                Matrix globalTransformation = Matrix.CreateScale(screenScalingFactor);

                // Begin a seperate sprite batch for menus
                m_SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, globalTransformation);

                // Render the current menu
                m_CurrentMenu.Render(deltaTime);

                // End the sprite batch
                m_SpriteBatch.End();
            }
        }

        public void ShowMenu(Menus menu)
        {
            ShowMenu(menu, true, true);
        }

        public void ShowMenu(Menus menu, bool restartBackground)
        {
            ShowMenu(menu, restartBackground, true);
        }

        public void ShowMenu(Menus menu, bool restartBackground, bool restartOptions)
        {
            Menu nextMenu = null;

            Debug.Assert( (int)menu < m_MenuChoices.Count, "[WARNING] - Menu selection available but no menu set up for this option!" );

            if ((int)menu < m_MenuChoices.Count)
            {
                if (m_Menus.Count > 0)
                    m_Menus.TryGetValue(m_MenuChoices[(int)menu], out nextMenu);

                if (nextMenu != null)
                {
                    m_NextMenu = nextMenu;

                    // Set whether to reset the menu transition or not
                    if (m_NextMenu.Transition != null)
                    {
                        m_NextMenu.Transition.RestartBackgroundTransition = restartBackground;
                        m_NextMenu.Transition.RestartOptionsTransition = restartOptions;
                    }

                    // Reset the next menu now because it is being shown
                    m_NextMenu.Reset();

                    // Close the current menu if one exists
                   /* if (m_CurrentMenu != null)
                    {
                        // Check if isn't already closing
                        if (!m_CurrentMenu.IsClosing() && !m_CurrentMenu.IsClosed())
                            CloseMenu();
                    }*/
                }
            }
        }

        public void ReturnToPreviousMenu()
        {
            Debug.Assert(m_PreviousMenu != null, "[WARNING - 'ReturnToPreviousMenu' but no previous menu exists!");

            // Save the next menu as the previous menu ( no reset required as it is going back to a previous menu )
            if (m_PreviousMenu != null)
                m_NextMenu = m_PreviousMenu;
        }

        public void CloseMenu()
        {
            if ( m_CurrentMenu != null )
            {
                if (m_CurrentMenu.IsOpen())
                    m_CurrentMenu.OnExit();
            }
        }

        public Menu GetMenu( Menus menu )
        {
            Menu toRet = null;

            Debug.Assert((int)menu < m_MenuChoices.Count, "[WARNING] - Menu selection available but no menu set up for this option!");

            if ((int)menu < m_MenuChoices.Count)
            {
                if (m_Menus.Count > 0)
                    m_Menus.TryGetValue(m_MenuChoices[(int)menu], out toRet);
            }

            return toRet;
        }

        public void AddMenuLinker(String menuName)
        {
            Debug.Assert( menuName != "" );

            if (menuName != "")
            {
                Debug.Assert(!m_MenuChoices.Contains(menuName));

                if ( !m_MenuChoices.Contains(menuName) )
                    m_MenuChoices.Add(menuName);
            }
        }

        public Menus GetMenuChoiceIndex( String menuName )
        {
            Menus menuChoice = Menus.Error;

            for (int i = 0; i < m_MenuChoices.Count; i++)
            {
                if ( m_MenuChoices[i] == menuName )
                {
                    menuChoice = (Menus)i;

                    break;
                }
            }

            return menuChoice;
        }

        public bool IsMenuClosed()
        {
            bool toRet = false;

            if (m_CurrentMenu != null)
                toRet = m_CurrentMenu.IsClosed();

            return toRet;
        }

        public bool IsMenuClosed( Menus menu )
        {
            bool toRet = false;

            if (m_CurrentMenu != null)
            {
                Menu menuToCheck = null;

                Debug.Assert( (int)menu < m_MenuChoices.Count, "[WARNING] - Menu selection available but no menu set up for this option!" );

                if ((int)menu < m_MenuChoices.Count)
                {
                    if (m_Menus.Count > 0)
                        m_Menus.TryGetValue(m_MenuChoices[(int)menu], out menuToCheck);

                    if (menuToCheck != null)
                        toRet = menuToCheck.IsClosed();
                }
            }

            return toRet;
        }

        public bool IsMenuClosing()
        {
            bool toRet = false;

            if ( m_CurrentMenu != null )
                toRet = m_CurrentMenu.IsClosing();

            return toRet;
        }

        public bool IsMenuClosing( Menus menu )
        {
            bool toRet = false;
            Menu menuToCheck = null;

            Debug.Assert((int)menu < m_MenuChoices.Count, "[WARNING] - Menu selection available but no menu set up for this option!");

            if ((int)menu < m_MenuChoices.Count)
            {
                if (m_Menus.Count > 0)
                    m_Menus.TryGetValue(m_MenuChoices[(int)menu], out menuToCheck);

                if (menuToCheck != null)
                    toRet = menuToCheck.IsClosing();
            }

            return toRet;
        }

        public Menu GetMenu(String name)
        {
            Menu toRet = null;

            if (name != "")
            {
                int index = m_Menus.IndexOfKey(name);

                if (index >= 0)
                    toRet = m_Menus.Values[index];
            }

            return toRet;
        }

        #region event handlers
        public virtual void Handle(PauseGameEvent message)
        {
            switch (message.Pause)
            {
                case true:
                    {
                        // Show pause menu
                        MenuManager.Instance.ShowMenu(MenuManager.Menus.PauseMenu, true);
                    }
                    break;
                case false:
                    {
                        // Close pause menu if it is open
                        MenuManager.Instance.CloseMenu();

                        // Make sure game music is playing
                        SoundManager.Instance.PlayMusic("gamemusic");
                    }
                    break;
            }
        }
        #endregion

        public SpriteBatch SpriteBatch { get { return m_SpriteBatch; } set { m_SpriteBatch = value; } }
    }
}
