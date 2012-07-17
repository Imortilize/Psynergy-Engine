using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Psynergy
{
    class ClassRegistryResource : XmlResource
    {
        public ClassRegistryResource(String filename) : base(filename)
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
                    if (node.Name.ToLower() == "classregistry")
                        LoadClassRegistry(node);
                }
            }
        }

        #region Seperate object loading functions
        private void LoadClassRegistry(XmlNode node)
        {
            // Now load the class registers them selfs
            foreach (XmlNode child in node)
            {
                if (node.NodeType != XmlNodeType.Comment)
                {
                    if (child.Name.ToLower() == "class")
                        RegisterClass(child);
                }
            }
        }

        private void RegisterClass(XmlNode node)
        {
            XmlNode className = node.Attributes.GetNamedItem("id");

            // Assuming this class was registered properly
            if (className != null)
                Factory.Instance.Register(className.InnerText);
        }
        #endregion
    }
}
