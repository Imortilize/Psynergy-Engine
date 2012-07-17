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
    public class RenderGroupResource : XmlResource
    {
        public RenderGroupResource(String filename) : base(filename)
        {
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
                if ((node.NodeType != XmlNodeType.XmlDeclaration) && (node.NodeType != XmlNodeType.Comment))
                {
                    // Load the scene
                    if (node.Name.ToLower() == "rendergroupresource")
                        LoadRenderGroups(node);
                }
            }
        }

        #region Seperate object loading functions
        private void LoadRenderGroups(XmlNode node)
        {
            // Now load the class registers them selfs
            foreach (XmlNode child in node)
            {
                if (node.NodeType != XmlNodeType.Comment)
                {
                    if (child.Name.ToLower() == "rendergroup")
                        LoadRenderGroup(child);
                }
            }
        }

        private void LoadRenderGroup(XmlNode node)
        {
            // Get the render group name
            XmlNode rendergroupName = node.Attributes.GetNamedItem("id");

            RenderGroup newRenderGroup = new RenderGroup(rendergroupName.InnerText);

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

                        // Check which render query this could be
                        if (propertyName.ToLower() == "enablelighting")
                            newRenderGroup.EnableLighting = Convert.ToBoolean(value);
                        else if (propertyName.ToLower() == "enabletexture")
                            newRenderGroup.EnableTexture = Convert.ToBoolean(value);
                        else if (propertyName.ToLower() == "enableshadow")
                            newRenderGroup.EnableShadow = Convert.ToBoolean(value);
                        else if (propertyName.ToLower() == "enablefog")
                            newRenderGroup.EnableFog = Convert.ToBoolean(value);
                        else if (propertyName.ToLower() == "skinned")
                            newRenderGroup.IsSkinned = Convert.ToBoolean(value);
                    }
                }
            }

            // Now try and add this to the render manager as a new render group
            RenderManager.Instance.AddRenderGroup(newRenderGroup);
        }
        #endregion
    }
}
