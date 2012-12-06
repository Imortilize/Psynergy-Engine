using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

/* Main Library */
using Psynergy;

namespace XnaGame
{
    class GameBoardResource : XmlResource
    {
        private GameBoard m_Gameboard = null;

        public GameBoardResource(String filename, GameBoard gameBoard) : base(filename)
        {
            m_Gameboard = gameBoard;
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnLoaded()
        {
            foreach (XmlNode node in this.ChildNodes)
            {
                // If it's not the head xml declaration
                if (node.NodeType != XmlNodeType.XmlDeclaration)
                {
                    // Load the scene
                    if (node.Name.ToLower() == "gameboardresource")
                        LoadGameBoardResource(node);
                }
            }
        }

        #region Seperate object loading functions
        private void LoadGameBoardResource(XmlNode node)
        {
            foreach (XmlNode child in node)
            {
                if (child.Name.ToLower() != "")
                    LoadBoardSquare(child);
            }

            // Now the board has been loaded, load all the board connections
            if (m_Gameboard != null)
            {
                m_Gameboard.LoadBoardConnections();
            }
        }

        private GameObject LoadBoardSquare(XmlNode node)
        {
            // Create a new sprite node
            GameObject newObject = (GameObject)Factory.Instance.Create(node.Name);

            if (newObject != null)
            {
                Type type = newObject.GetType();

                // Look for a node id attribute
                XmlNode idAttr = node.Attributes.GetNamedItem("id");
                String id = "";

                if (idAttr != null)
                    id = idAttr.InnerText;

                // Save the node name
                newObject.Name = id;

                // Search through the children nodes to determine what properties to set 
                foreach (XmlNode child in node)
                {
                    // If it is not commented out etc..
                    if (child.Attributes != null)
                    {
                        if (child.Attributes.Count > 0)
                        {
                            XmlAttribute attr = child.Attributes[0];
                            String propertyName = attr.Name;
                            String value = attr.InnerText;

                            // If it is a class then load this node and it's own attributes
                            if (child.Name == "property")
                            {
                                // Set the property through the factory assigned properties
                                bool success = Factory.Instance.SetProperty(type, propertyName, value, newObject);
                            }
                            else
                            {
                                // This is likely to be a node contained as a property to another node
                                // Load this node
                                GameObject containedNode = LoadBoardSquare(child);

                                // Set the property through the factory assigned properties
                                bool success = Factory.Instance.SetClass(type, propertyName, containedNode, newObject);
                            }
                        }
                    }
                }

                if (newObject.GetType().IsSubclassOf(typeof(GameObject)))
                {
                    // Add it as a child to the game board
                    if (m_Gameboard != null)
                    {
                        BoardSquare squareExists = m_Gameboard.GetBoardSquare(newObject.Name);
                        String squareExistsName = "";

                        if (squareExists != null)
                            squareExistsName = squareExists.Name;

                        Debug.Assert(squareExists == null, "BoardSquare " + squareExistsName + " already exists!");

                        if (squareExists == null)
                        {
                            // Initialise board square
                            newObject.Initialise();

                            // Add board square
                            m_Gameboard.AddChild(newObject);
                        }
                    }
                }

                // Now that all the properties for this node in the xml file have been assigned,
                // Assign any default properties that are expected to be set in the xml file but havn't been
                newObject.DefaultProperties();
            }

            return newObject;
        }
        #endregion
    }
}
