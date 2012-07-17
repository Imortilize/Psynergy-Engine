using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

/* Main Library */
using Psynergy;

namespace Psynergy.Graphics
{
    public class SceneFragment : XmlResource
    {
        Hierarchy m_Hierarchy = null;

        public SceneFragment(String filename, Hierarchy hierarchy)
            : base(filename)
        {
            m_Hierarchy = hierarchy;

            if ( m_Hierarchy != null )
                m_ParentNode = hierarchy.RootNode;
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnLoaded()
        {
            // Check the parent node to attach objects to is set
            if ( m_ParentNode != null )
            {
                foreach (XmlNode node in this.ChildNodes)
                {
                    // If it's not the head xml declaration
                    if ((node.NodeType != XmlNodeType.XmlDeclaration) && (node.NodeType != XmlNodeType.Comment))
                    {
                        // Load the scene
                        if (node.Name.ToLower() == "scenefragment")
                            LoadSceneFragment(node);
                    }
                }
            }
        }

        #region Seperate object loading functions
        private void LoadSceneFragment(XmlNode node)
        {
            foreach (XmlNode child in node)
            {
                if ( child.NodeType != XmlNodeType.Comment )
                    LoadNode(child);
            }
        }

        private GameObject LoadNode(XmlNode node)
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
                                GameObject containedNode = LoadNode(child);

                                // Set the property through the factory assigned properties
                                bool success = Factory.Instance.SetClass(type, propertyName, containedNode, newObject);
                            }
                        }
                    }
                }

                if (newObject.GetType().IsSubclassOf(typeof(Node)))
                {
                    Node newNode = (newObject as Node);

                    // Check if a parent name was set
                    if ((newNode.ParentName != null) && (newNode.ParentName != ""))
                    {
                        if (newNode.ParentName == m_ParentNode.Name)
                            m_ParentNode.AddChild(newNode);
                        else
                        {
                            Node parentNode = m_ParentNode.FindChild(newNode.ParentName);

                            if (parentNode != null)
                                parentNode.AddChild(newNode);
                        }

                        Debug.Assert(newNode.Parent != null, "Parent node not set to node " + newNode.Name + ".");
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

