using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

/* Input Library */
using Psynergy.Input;

namespace Psynergy.Menus
{
    public class Menu : Node, IRegister<Menu>
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterClass(typeof(MenuTransition), "transition", "Transition");
            factory.RegisterBool("showloading", "ShowLoading");

            base.ClassProperties(factory);
        }

        #endregion

        protected enum MenuState
        {
            Open,
            Update,
            Close,
            Finished
        }

        private MenuBackground m_MenuBackground = null;
        private List<KeyValuePair<String, MenuOption>> m_MenuOptions = new List<KeyValuePair<String, MenuOption>>();
        private MenuOption m_CurrentMenuOption = null;
        private int m_MenuOptionIndex = 0;

        private bool m_Open = false;        // Whether the menu is open or not

        // The current state the menu is in
        private MenuState m_MenuState = MenuState.Open;

        // Menu action attached to the entire menu ( mainly the back action )
        private MenuAction m_Action = null;

        // Menu option transition
        private MenuTransition m_Transition = null;

        // Flag to say whether to show loading screen once closed
        private bool m_ShowLoading = false;

        public Menu() : base("")
        {
        }

        public Menu(String name) : base(name)
        {
        }

        public Menu(String name, String resource) : base(name)
        {
        }

        public override void Initialise()
        {
            base.Initialise();

            if (m_MenuBackground != null)
            {
                m_MenuBackground.Initialise();

                // Add the background to the transition if it exists
                if (m_Transition != null)
                    m_Transition.AddMenuBackground(m_MenuBackground);
            }

            for (int i = 0; i < m_MenuOptions.Count; i++)
            {
                if (m_MenuOptions[i].Value != null)
                {
                    m_MenuOptions[i].Value.Initialise();

                    // Add this menu option to the menu transition if it exists
                    if (m_Transition != null)
                        m_Transition.AddMenuOption(m_MenuOptions[i].Value);
                }
            }

            // Initialise menu transition
            if (m_Transition != null)
                m_Transition.Initialise();

            // Reset the menu
            Reset();
        }

        public override void Reset()
        {
            if (m_MenuBackground != null)
                m_MenuBackground.Reset();

            for (int i = 0; i < m_MenuOptions.Count; i++)
            {
                // Set the first element of the menu to be selected. All other elements should be shown unselected
                if (i == 0)
                {
                    m_MenuOptionIndex = i;  // Menu option index should be set to the first element

                    // Set the current option to the first element and select it 
                    m_CurrentMenuOption = m_MenuOptions.ElementAt(0).Value;
                    m_CurrentMenuOption.Select();
                }
                else
                    m_MenuOptions.ElementAt(i).Value.Deselect();
            }

            // Initialise menu transition
            if (m_Transition != null)
                m_Transition.Reset();

            m_Open = false; // Set the menu to no be open ( this is done when updating the state from OnEnter )
        }

        public override void Load()
        {
            if (m_MenuBackground != null)
                m_MenuBackground.Load();

            for (int i = 0; i < m_MenuOptions.Count; i++)
            {
                if ( m_MenuOptions[i].Value != null )
                    m_MenuOptions[i].Value.Load();
            }

            // Load the menu transition if it exists
            if (m_Transition != null)
                m_Transition.Load();
        }

        public override void UnLoad()
        {
            if (m_MenuBackground != null)
                m_MenuBackground.UnLoad();

            for (int i = 0; i < m_MenuOptions.Count; i++)
            {
                if (m_MenuOptions[i].Value != null)
                    m_MenuOptions[i].Value.UnLoad();
            }

            // Load the menu transition if it exists
            if (m_Transition != null)
                m_Transition.UnLoad();
        }

        public virtual void OnEnter()
        {
            // Set the menu to the open state
            m_MenuState = MenuState.Open;

            // Reset the transition so it can run any opening transitions
            if (m_Transition != null)
                m_Transition.OnEnter();

            // if a background is assigned, play music associated with it 
            if (m_MenuBackground != null)
                m_MenuBackground.PlayMusic();

            // Set the menu is be open
            m_Open = true;
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            // Run the appropiate menu state
            switch (m_MenuState)
            {
                case MenuState.Open:
                    {
                        UpdateOpenMenu(deltaTime);
                    }
                    break;

                case MenuState.Update:
                    {
                        UpdateMenu(deltaTime);
                    }
                    break;

                case MenuState.Close:
                    {
                        UpdateCloseMenu(deltaTime);
                    }
                    break;
                case MenuState.Finished:
                    {
                        // The menu is no longer open
                        m_Open = false;

                        // If application is paused, unpause it
                        InputHandle.PauseApplication = false;
                    }
                    break;
            }
        }

        // Overridable by inheritable menus where menus update specifically
        protected virtual void UpdateOpenMenu(GameTime deltaTime)
        {
            bool finished = true;

            // If a transition exists then run the open state on it
            if (m_Transition != null)
            {
                bool menuBackgroundFinished = m_Transition.UpdateMenuBackgroundOpenTransition(deltaTime);
                bool menuOptionFinished = m_Transition.UpdateMenuOptionOpenTransitions(deltaTime);

                // If either the background or menu options arn't finished then flag that 
                // the opening sequence hasn't finished yet
                if ((!menuBackgroundFinished || !menuOptionFinished) && finished)
                    finished = false;
            }

            if (finished)
            {
                // Default to go to menu update
                m_MenuState = MenuState.Update;
            }

            // Update items
            UpdateMenuOptions(deltaTime);
        }

        // Overridable by inheritable menus where menus update specifically
        protected virtual void UpdateMenu(GameTime deltaTime)
        {
            // Update menu background
            // Render the background first
            if (m_MenuBackground != null)
                m_MenuBackground.Update(deltaTime);

            // Update items
            UpdateMenuOptions(deltaTime);
        }

        // Overridable by inheritable menus where menus update specifically
        protected virtual void UpdateCloseMenu(GameTime deltaTime)
        {
            bool finished = true;

            // If a transition exists then run the open state on it
            if (m_Transition != null)
            {
                bool menuBackgroundFinished = m_Transition.UpdateMenuBackgroundCloseTransition(deltaTime);
                bool menuOptionFinished = m_Transition.UpdateMenuOptionCloseTransitions(deltaTime);

                // If either the background or menu options arn't finished then flag that 
                // the opening sequence hasn't finished yet
                if ((!menuBackgroundFinished || !menuOptionFinished) && finished)
                    finished = false;
            }

            if (finished)
            {
                // Default to go to the menu finished state
                m_MenuState = MenuState.Finished;

                if (m_ShowLoading)
                {
                    // If show loading is true then show loading screen from menu manager
                    MenuManager.Instance.ShowMenu(MenuManager.Menus.LoadingMenu);
                }
            }
        }

        private void UpdateMenuOptions(GameTime deltaTime)
        {
            // Check which menu option is selected
            CheckMenuOptionSelected();

            // Update the current menu option
            if (m_CurrentMenuOption != null)
                m_CurrentMenuOption.Update(deltaTime);

            // Check if the menu specific action exists.
            UpdateAction();
        }

        private void UpdateAction()
        { 
            // If so then run the action applicable
            if (m_Action != null)
            {
                // Check if the action is used or not
                // If so then run it, otherwise keep polling
                if (m_Action.CheckAction())
                    m_Action.RunAction();
            }
        }

        public override void Render(GameTime deltaTime)
        {
            // Render the background first
            if (m_MenuBackground != null)
                m_MenuBackground.Render(deltaTime);

            for (int i = 0; i < m_MenuOptions.Count; i++)
            {
                if (m_MenuOptions[i].Value != null)
                    m_MenuOptions[i].Value.Render(deltaTime);
            }
        }

        public virtual void OnExit()
        {
            // Set the menu to the close state
            m_MenuState = MenuState.Close;

            // Reset the transition so it can run any close transitions
            if (m_Transition != null)
                m_Transition.OnClose();
        }

        public void SetMenuAction(MenuAction menuAction)
        {
            m_Action = menuAction;
        }

        public void AddMenuOption(MenuOption menuOption)
        {
            // Add the menu option as a child to this menu
            m_MenuOptions.Add(new KeyValuePair<String, MenuOption>(menuOption.Name, menuOption));
        }

        public void RemoveMenuOption(MenuOption menuOption)
        {
            // RemoveAdd the menu option as a child to this menu
            for ( int i = 0; i < m_MenuOptions.Count; i++ )
            {
                if (m_MenuOptions[i].Value != null)
                {
                    if ( m_MenuOptions[i].Value == menuOption )
                        m_MenuOptions.RemoveAt(i);
                }
            }
        }

        private void CheckMenuOptionSelected()
        {
            // Check that there are more then just 1 menu option available
            if (m_MenuOptions.Count > 1)
            {
                bool menuOptionChange = false;

                // Check if a new menu option is selected
                if (InputHandle.Down(PlayerIndex.One))
                {
                    if ((m_MenuOptionIndex + 1) < m_MenuOptions.Count)
                    {
                        // Select the new menu option
                        m_MenuOptionIndex++;

                        // Menu option has changed
                        menuOptionChange = true;
                    }
                }
                else if (InputHandle.Up(PlayerIndex.One))
                {
                    if (m_MenuOptionIndex > 0)
                    {
                        // Select the new menu option
                        m_MenuOptionIndex--;

                        // Menu option has changed
                        menuOptionChange = true;
                    }
                }

                // Check mouse input  for selection and clicking
                if (InputHandle.IsMouseConnected())
                {
                    Vector2 currentMousePos = InputHandle.MousePosition;

                    for ( int i = 0; i < m_MenuOptions.Count; i++ )
                    {
                        KeyValuePair<String, MenuOption> option = m_MenuOptions[i];
                        MenuOption menuOption = option.Value;

                        // Check if the mouse is within this menu option
                        if (menuOption.IsMouseOverOption(currentMousePos))
                        {
                            // If the found collision is not the option already selected
                            if (m_MenuOptionIndex != i)
                            {
                                // Set new menu option index
                                m_MenuOptionIndex = i;

                                // Select the new menu option
                                menuOptionChange = true;
                            }

                            // Leave loop
                            break;
                        }
                    }
                }

                if (menuOptionChange)
                {
                    // Make the current menu option not to be selected anymore
                    m_CurrentMenuOption.Deselect();

                    // Change to the new option
                    m_CurrentMenuOption = m_MenuOptions.ElementAt(m_MenuOptionIndex).Value;
                        
                    // Set the new option to be selected
                    m_CurrentMenuOption.Select();
                }
            }
        }

        public void SetMenuBackground(MenuBackground menuBackground)
        {
            Debug.Assert(m_MenuBackground == null, "Background already set on menu " + Name);

            // Set the background
            m_MenuBackground = menuBackground;            
        }

        public bool IsOpen()
        {
            return m_Open;
        }

        public bool IsClosed()
        {
            return (!m_Open || (m_MenuState == MenuState.Finished));
        }

        public bool IsClosing()
        {
            return (m_MenuState == MenuState.Close);
        }

        public MenuOption GetOption( String name )
        {
            MenuOption toRet = null;

            if (name != "")
            {
                foreach (KeyValuePair<String, MenuOption> option in m_MenuOptions)
                {
                    if (option.Key == name)
                        toRet = option.Value;
                }
            }

            return toRet;
        }

        #region Property Set / Gets
        public MenuTransition Transition { get { return m_Transition; } set { m_Transition = value; } }
        public bool ShowLoading { get { return m_ShowLoading; } set { m_ShowLoading = value; } }
        #endregion
    }
}
