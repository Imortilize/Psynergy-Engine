using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Psynergy.Camera
{
    class CameraResource : XmlResource
    {
        private GraphicsDevice m_GraphicsDevice = null;
        private SortedList<String, BaseCamera> m_Cameras = new SortedList<String, BaseCamera>();

        public CameraResource(String filename, GraphicsDevice graphicsDevice) : base(filename)
        {
            m_GraphicsDevice = graphicsDevice;
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnLoaded()
        {
            // The scene storage must be valid otherwise it is pointless loading the scenes
            if (m_Cameras != null)
            {
                foreach (XmlNode node in this.ChildNodes)
                {
                    // If it's not the head xml declaration
                    if (node.NodeType != XmlNodeType.XmlDeclaration)
                    {
                        // Load the scene
                        if ( node.Name.ToLower() == "cameraresource" )
                            LoadCameraResource(node);
                    }
                }

                // Load and Initialise the loaded scenes
                for (int i = 0; i < m_Cameras.Count; i++)
                {
                    // Save graphics device if any and then initialise and load the camera
                    m_Cameras.ElementAt(i).Value.GraphicsDevice = m_GraphicsDevice;
                    m_Cameras.ElementAt(i).Value.Initialise();
                    m_Cameras.ElementAt(i).Value.Load();
                }
            }
        }

        #region Seperate object loading functions
        private void LoadCameraResource(XmlNode node)
        {
            foreach (XmlNode child in node)
            {
                if (child.NodeType != XmlNodeType.Comment)
                    LoadNode(child);
            }
        }

        private void LoadNode(XmlNode node)
        {
            // Create the scene
            BaseCamera camera = CreateCamera(node);

            // Check the scene exists and add it to the scene list
            if (camera != null)
                AddCamera(camera);
        }

        private BaseCamera CreateCamera(XmlNode cameraNode)
        {
            BaseCamera toRet = (Factory.Instance.Create(cameraNode.Name) as BaseCamera);

            if (toRet != null)
            {
                XmlNode nodeID = cameraNode.Attributes.GetNamedItem("id");
                Debug.Assert(nodeID != null, "[WARNING] - A camera node must have a unique id!");

                if (nodeID != null)
                {
                    String nodeIDName = nodeID.InnerText;

                    if ( nodeIDName != "" )
                        toRet.Name = nodeIDName;

                    // Add any scene properties
                    foreach (XmlNode child in cameraNode)
                    {
                        if (child.NodeType != XmlNodeType.Comment)
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
                }
            }

            return toRet;
        }

        private void AddCamera(BaseCamera camera)
        {
            m_Cameras.Add(camera.Name, camera);
        }
        #endregion

        #region Property Set / Gets
        public SortedList<String, BaseCamera> LoadedCameras { get { return m_Cameras; } }
        #endregion
    }
}
