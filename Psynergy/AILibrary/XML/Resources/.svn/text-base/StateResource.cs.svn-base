using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

/* Main Library */
using Psynergy;

namespace Psynergy.AI
{
    public class StateResource<T> : XmlResource
    {
        private StateMachine<T> m_StateManager = null;

        public StateResource(String filename, StateMachine<T> stateManager) : base(filename)
        {
            m_StateManager = stateManager;
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
                    if (node.Name.ToLower() == "stateresource")
                        LoadStateResource(node);
                }
            }
        }

        #region Seperate object loading functions
        private void LoadStateResource(XmlNode node)
        {
            foreach (XmlNode child in node)
            {
                if (child.Name.ToLower() == "state")
                    LoadState(child);
                else if (child.Name.ToLower() == "defaultstate")
                    LoadDefaultState(child);
            }
        }

        private void LoadState(XmlNode node)
        {
            String id = "";
            String type = "";

            foreach (XmlAttribute attr in node.Attributes)
            {
                if (attr.Name.ToLower() == "id")
                    id = attr.InnerText;
                else if (attr.Name.ToLower() == "type")
                    type = attr.InnerText;
            }

            if ( (id != "") && (type != "") )
            {
                GameObject created = Factory.Instance.Create(type);
                State<T> newState = (State<T>)created;

                m_StateManager.AddState(id, newState);
            }
        }

        private void LoadDefaultState(XmlNode node)
        {
            XmlNode attr = node.Attributes.GetNamedItem("id");

            if (attr != null)
                m_StateManager.SetDefaultState(attr.InnerText);
        }
        #endregion
    }
}
