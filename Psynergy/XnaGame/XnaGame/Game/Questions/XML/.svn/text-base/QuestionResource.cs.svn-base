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
    class QuestionResource : XmlResource
    {
        List<Question> m_Questions = new List<Question>();

        public QuestionResource(String filename) : base(filename)
        {
        }

        public override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnLoaded()
        {
            // The scene storage must be valid otherwise it is pointless loading the scenes
            if (m_Questions != null)
            {
                foreach (XmlNode node in this.ChildNodes)
                {
                    // If it's not the head xml declaration
                    if (node.NodeType != XmlNodeType.XmlDeclaration)
                    {
                        // Load the scene
                        if (node.Name.ToLower() == "questionresource")
                            LoadQuestionResource(node);
                    }
                }

                // Load and Initialise the loaded scenes
                for (int i = 0; i < m_Questions.Count; i++)
                {
                    m_Questions[i].Initialise();
                    m_Questions[i].Load();
                }
            }
        }

        #region Seperate object loading functions
        private void LoadQuestionResource(XmlNode node)
        {
            foreach (XmlNode child in node)
                LoadNode(child);
        }

        private void LoadNode(XmlNode node)
        {
            // Check if it is a scene resource or not
            if (node.Name.ToLower() == "question")
            {
                // Create the scene
                Question question = CreateQuestion(node);

                // Check the scene exists and add it to the scene list
                if (question != null)
                    AddQuestion(question);
            }
        }

        private Question CreateQuestion(XmlNode questionresource)
        {
            Question toRet = null;

            XmlNode nodeID = questionresource.Attributes.GetNamedItem("id");
            Debug.Assert(nodeID != null, "[WARNING] - A question node must have a unique id!");

            XmlNode textureNode = questionresource.Attributes.GetNamedItem("texture");
            Debug.Assert(textureNode != null, "[WARNING] - A question node must have a texture assigned!");

            XmlNode fadeNode = questionresource.Attributes.GetNamedItem("faderate");

            String nodeName = "";
            String textureName = "";
            float fadeRate = 1.0f;

            if ( nodeID != null )
                nodeName = nodeID.Value;

            if ( textureNode != null )
                textureName = textureNode.Value;

            if ( fadeNode != null )
                fadeRate = Convert.ToSingle(fadeNode.Value);

            // Create the question
            toRet = new Question(nodeName, textureName, fadeRate);

            if (nodeID != null)
            {
                // Add any scene properties
                foreach (XmlNode child in questionresource)
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

        private void AddQuestion(Question question)
        {
            m_Questions.Add(question);
        }
        #endregion

        #region Property Set / Gets
        public List<Question> Questions { get { return m_Questions; } }
        #endregion
    }
}