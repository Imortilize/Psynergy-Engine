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
    class SceneResource : XmlResource
    {
        SortedList<String, Scene> m_Scenes = new SortedList<String, Scene>();

        public SceneResource(String filename)
            : base(filename)
        {
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnLoaded()
        {
            // The scene storage must be valid otherwise it is pointless loading the scenes
            if (m_Scenes != null)
            {
                foreach (XmlNode node in this.ChildNodes)
                {
                    // If it's not the head xml declaration
                    if (node.NodeType != XmlNodeType.XmlDeclaration)
                    {
                        // Load the scene
                        if (node.Name.ToLower() == "sceneresource")
                            LoadSceneResource(node);
                    }
                }

                // Load and Initialise the loaded scenes
                for (int i = 0; i < m_Scenes.Count; i++)
                {
                    m_Scenes.ElementAt(i).Value.Initialise();
                    m_Scenes.ElementAt(i).Value.Load();
                }
            }
        }

        #region Seperate object loading functions
        private void LoadSceneResource(XmlNode node)
        {
            foreach (XmlNode child in node)
                LoadNode(child);
        }

        private void LoadNode(XmlNode node)
        {
            // Check if it is a scene resource or not
            if (node.Name.ToLower() == "scenefragment")
            {
                // Create the scene
                Scene scene = CreateScene(node);

                // Check the scene exists and add it to the scene list
                if (scene != null)
                    AddScene(scene);
            }
        }

        private Scene CreateScene(XmlNode sceneresource)
        {
            Scene toRet = null;

            XmlNode nodeID = sceneresource.Attributes.GetNamedItem("id");
            Debug.Assert(nodeID != null, "[WARNING] - A scene resource node must have a unique id!");

            if (nodeID != null)
            {
                // See if a filename was attached
                XmlNode sceneFile = sceneresource.Attributes.GetNamedItem("file");
                Debug.Assert(sceneFile != null, "[WARNING] - A scene resource node must have a file attached!");

                String nodeIDName = nodeID.InnerText;
                String nodeFile = sceneFile.InnerText;

                // Create the scene accordingly whether it is 3D or not
                toRet = new Scene(nodeIDName, nodeFile);

                // Add any scene properties
                foreach (XmlNode child in sceneresource)
                {
                    if (child.Attributes.Count > 0)
                    {
                        XmlAttribute attr = child.Attributes[0];
                        String propertyName = attr.Name;
                        String value = attr.InnerText;

                        // Set the property through the factory assigned properties
                        bool success = Factory.Instance.SetProperty(toRet.GetType(), propertyName, value, toRet);

                        Debug.Assert(success, "[Warning] - Loading property " + propertyName + " on node " + toRet.Name + " failed!");
                    }
                }
            }

            return toRet;
        }

        private void AddScene(Scene scene)
        {
            m_Scenes.Add(scene.Name, scene);
        }

        public int GetSceneIndex(String name)
        {
            int toRet = -1;

            // Try and get the index
            toRet = m_Scenes.IndexOfKey(name);

            return toRet;
        }

        public Scene GetScene(String name)
        {
            Scene toRet = null;

            // Search for the scene index
            int index = GetSceneIndex(name);

            // If the scene exists the index will be 0 or above
            if (index >= 0)
                toRet = m_Scenes.ElementAt(index).Value;

            return toRet;
        }

        #endregion
    }
}