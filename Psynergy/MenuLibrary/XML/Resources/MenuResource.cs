using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

/* Main Library */
using Psynergy;

namespace Psynergy.Menus
{
    class MenuResource : XmlResource
    {
        private SortedList<String, Menu> m_Menus = null;

        public MenuResource(String filename) : base(filename)
        {
        }

        public MenuResource(String filename, SortedList<String, Menu> container) : base(filename)
        {
            Debug.Assert(container != null, "Menu container must be non-null!");

            if (container != null)
                m_Menus = container;
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnLoaded()
        {
            // Check the menus container isn't null
            if (m_Menus != null)
            {
                foreach (XmlNode node in this.ChildNodes)
                {
                    // If it's not the head xml declaration
                    if (node.NodeType != XmlNodeType.XmlDeclaration)
                    {
                        // Load the scene
                        if (node.Name.ToLower() == "menuresource")
                            LoadMenuResource(node);
                    }
                }
            }
        }

        #region Seperate object loading functions
        private void LoadMenuResource(XmlNode node)
        {
            // First load menu linkers ( used to go from one menu to another )
            foreach (XmlNode child in node)
            {
                if (child.Name.ToLower() == "menu")
                    LoadMenuLinkers(child);
            }

            // Now load the menus them selfs
            foreach (XmlNode child in node)
            {
                if (child.Name.ToLower() == "menu")
                    LoadMenu(child);
            }
        }

        private void LoadMenuLinkers(XmlNode node)
        {
            XmlNode menuNameAttr = node.Attributes.GetNamedItem("id");

            if (menuNameAttr != null)
            {
                String menuName = menuNameAttr.InnerText;

                // Add the menu to the menu manager
                MenuManager.Instance.AddMenuLinker(menuName);
            }
        }

        private void LoadMenu(XmlNode node)
        {
            // Create a new sprite node
            Menu newMenu = (Menu)Factory.Instance.Create(node.Name);
            Debug.Assert(newMenu != null, "Menu is NULL!");

            Type type = newMenu.GetType();

            XmlNode menuNameAttr = node.Attributes.GetNamedItem("id");
            String menuName = "";

            if (menuNameAttr != null)
                menuName = menuNameAttr.InnerText;

            // Search through the children nodes to determine what properties to set 
            foreach (XmlNode child in node)
            {
                if (child.Name.ToLower() == "property")
                {
                    if (child.Attributes.Count > 0)
                    {
                        XmlAttribute attr = child.Attributes[0];
                        String propertyName = attr.Name;
                        String value = attr.InnerText;

                        // Set the property through the factory assigned properties
                        bool success = Factory.Instance.SetProperty(type, propertyName, value, newMenu);

                        Debug.Assert(success, "Setting menu property " + propertyName + " failed!");
                    }
                }
                else if (child.Name.ToLower().Contains("menuoption"))
                {
                    MenuOption menuOption = LoadMenuOption(child, type);

                    Debug.Assert(menuOption != null, "Menu option should not be null!");

                    // Add the menu option as a child to the menu
                    newMenu.AddMenuOption(menuOption);
                }
                else if (child.Name.ToLower() == "link")
                {
                    MenuAction newMenuAction;

                    // Create the new menu link
                    newMenuAction = LoadMenuAction(child, type, true);

                    Debug.Assert(newMenuAction != null, "New menu action can not be added if null!");

                    if (newMenuAction != null)
                    {
                        // Add the action to the menu option
                        newMenu.SetMenuAction(newMenuAction);

                        XmlNode menuLinkName = child.Attributes.GetNamedItem("menu");

                        if ((menuLinkName != null) && (menuLinkName.InnerText != ""))
                        {
                            MenuManager.Menus menuToLink = MenuManager.Instance.GetMenuChoiceIndex(menuLinkName.InnerText);

                            // If it was a menu link, store the menu to link to as well
                            (newMenuAction as MenuActionMenuLink).SetMenuLink(menuToLink);
                        }
                    }
                }
                else if (child.Name.ToLower() == "background")
                {
                    MenuBackground newMenuBackground = LoadMenuBackground(child, type);

                    Debug.Assert(newMenuBackground != null, "Menu option should not be null!");

                    // Add the menu option as a child to the menu
                    newMenu.SetMenuBackground(newMenuBackground);
                }
                else if (child.Name.ToLower() == "transition")
                {
                    MenuTransition menuTransition = LoadMenuTransition(child, type);

                    Debug.Assert(menuTransition != null, "Menu Transition is null on menu option " + newMenu.Name);

                    // Set the property through the factory assigned properties
                    bool success = Factory.Instance.SetClass(typeof(Menu), "transition", menuTransition, newMenu);
                }
            }

            if (menuName == "")
                menuName = "NOT-ASSIGNED!";

            // Set the menu name
            newMenu.Name = menuName;

            // Add to the menus list
            m_Menus.Add(menuName, newMenu);
        }

        private MenuOption LoadMenuOption(XmlNode node, Type type)
        {
            // Create the menu option
            MenuOption newMenuOption = (MenuOption)Factory.Instance.Create(node.Name);

            XmlNode menuOptionNameAttr = node.Attributes.GetNamedItem("id");

            if (menuOptionNameAttr != null)
                newMenuOption.Name = menuOptionNameAttr.InnerText;

            foreach (XmlNode menuOptionProperties in node)
            {
                String propName = menuOptionProperties.Name.ToLower();

                if (menuOptionProperties.Name.ToLower() == "property")
                {
                    if (menuOptionProperties.Attributes.Count > 0)
                    {
                        XmlAttribute MOAttr = menuOptionProperties.Attributes[0];
                        String MOPropertyName = MOAttr.Name;
                        String MOValue = MOAttr.InnerText;

                        // Set the property through the factory assigned properties
                        bool success = Factory.Instance.SetProperty(newMenuOption.GetType(), MOPropertyName, MOValue, newMenuOption);

                        Debug.Assert(success, "Setting menu option property " + MOPropertyName + " failed!");
                    }
                }
                else if ((menuOptionProperties.Name.ToLower() == "action") || menuOptionProperties.Name.ToLower() == "link")
                {
                    MenuAction newMenuAction;
                    
                    if (menuOptionProperties.Name.ToLower() == "action")
                        newMenuAction = LoadMenuAction(menuOptionProperties, type, false);
                    else
                        newMenuAction = LoadMenuAction(menuOptionProperties, type, true);

                    Debug.Assert(newMenuAction != null, "New menu action can not be added if null!");

                    if (newMenuAction != null )
                    {
                        // Add the action to the menu option
                        newMenuOption.SetMenuAction(newMenuAction);

                        if ( menuOptionProperties.Name.ToLower() == "link" )
                        {
                            XmlNode menuName = menuOptionProperties.Attributes.GetNamedItem("menu");

                            if ( (menuName != null) && (menuName.InnerText != "") )
                            {
                                MenuManager.Menus menuToLink = MenuManager.Instance.GetMenuChoiceIndex( menuName.InnerText );

                                // If it was a menu link, store the menu to link to as well
                                (newMenuAction as MenuActionMenuLink).SetMenuLink(menuToLink);
                            }
                        }
                    }
                }
            }

            return newMenuOption;
        }

        private MenuAction LoadMenuAction(XmlNode node, Type type, bool isLink)
        {
            XmlNode menuActionNode;
            String menuActionName = "";

            // If this menu action is not a menu link then load the action name from its "id" field.
            // Otherwise use the defaulted link name
            if (!isLink)
            {
                menuActionNode = node.Attributes.GetNamedItem("id");
                menuActionName = menuActionNode.InnerText;
            }
            else
                menuActionName = "MenuLink";

            // Create the menu option and set it's name
            MenuAction newMenuAction = (MenuAction)Factory.Instance.Create(("MenuAction" + menuActionName));
            Debug.Assert(newMenuAction != null, "Menu Action '" + menuActionName + "' should not be null!");

            if (newMenuAction != null)
            {
                newMenuAction.Name = menuActionName;

                // Attempt to get the input attribute
                XmlNode inputAttr = node.Attributes.GetNamedItem("input");

                if (inputAttr != null)
                    newMenuAction.SetKeyPress(inputAttr.InnerText);

                // If this is a menu link then check for any reset flags
                if (menuActionName.Contains("MenuLink"))
                {
                    // Attempt to get the input attribute
                    XmlNode backgroundTransitionResetAttr = node.Attributes.GetNamedItem("resetbackgroundtransition");
                    XmlNode optionsTransitionResetAttr = node.Attributes.GetNamedItem("resetoptionstransition");

                    if (backgroundTransitionResetAttr != null)
                        (newMenuAction as MenuActionMenuLink).ResetMenuBackgroundTransition = System.Convert.ToBoolean(backgroundTransitionResetAttr.Value);

                    if (optionsTransitionResetAttr != null)
                        (newMenuAction as MenuActionMenuLink).ResetMenuOptionsTransition = System.Convert.ToBoolean(optionsTransitionResetAttr.Value);
                }

                // Sound
                XmlNode soundAttr = node.Attributes.GetNamedItem("sound");

                if (soundAttr != null)
                    newMenuAction.Sound = soundAttr.Value;
            }

            return newMenuAction;
        }

        private MenuBackground LoadMenuBackground(XmlNode node, Type type)
        {
            // Create the menu option
            MenuBackground newMenubackgroud = (MenuBackground)Factory.Instance.Create("MenuBackground");

            XmlNode menuBackgroundNameAttr = node.Attributes.GetNamedItem("id");

            if (menuBackgroundNameAttr != null)
                newMenubackgroud.Name = menuBackgroundNameAttr.InnerText;

            foreach (XmlNode menuBackgroundProperties in node)
            {
                String propName = menuBackgroundProperties.Name.ToLower();

                if (menuBackgroundProperties.Name.ToLower() == "property")
                {
                    if (menuBackgroundProperties.Attributes.Count > 0)
                    {
                        XmlAttribute MBAttr = menuBackgroundProperties.Attributes[0];
                        String MOPropertyName = MBAttr.Name;
                        String MOValue = MBAttr.InnerText;

                        // Set the property through the factory assigned properties
                        bool success = Factory.Instance.SetProperty(newMenubackgroud.GetType(), MOPropertyName, MOValue, newMenubackgroud);

                        Debug.Assert(success, "Setting menu basckground property " + MOPropertyName + " failed!");
                    }
                }
            }

            return newMenubackgroud;
        }

        private MenuTransition LoadMenuTransition(XmlNode node, Type type)
        {
            XmlNode menuTransitionTypeAttr = node.Attributes.GetNamedItem("type");
            MenuTransition newMenuTransition = null;

            if (menuTransitionTypeAttr != null)
            {
                // Create the menu option
                newMenuTransition = (MenuTransition)Factory.Instance.Create(menuTransitionTypeAttr.Value);

                XmlNode menuBackgroundNameAttr = node.Attributes.GetNamedItem("id");

                if (menuBackgroundNameAttr != null)
                    newMenuTransition.Name = menuBackgroundNameAttr.InnerText;

                foreach (XmlNode menuTransitionProperties in node)
                {
                    String propName = menuTransitionProperties.Name.ToLower();

                    if (menuTransitionProperties.Name.ToLower() == "property")
                    {
                        if (menuTransitionProperties.Attributes.Count > 0)
                        {
                            XmlAttribute MBAttr = menuTransitionProperties.Attributes[0];
                            String MOPropertyName = MBAttr.Name;
                            String MOValue = MBAttr.InnerText;

                            // Set the property through the factory assigned properties
                            bool success = Factory.Instance.SetProperty(newMenuTransition.GetType(), MOPropertyName, MOValue, newMenuTransition);

                            Debug.Assert(success, "Setting menu transition property " + MOPropertyName + " failed!");
                        }
                    }
                }
            }

            return newMenuTransition;
        }

        #endregion
    }
}
