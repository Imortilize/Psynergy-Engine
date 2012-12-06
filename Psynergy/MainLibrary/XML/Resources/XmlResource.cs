using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Psynergy
{
    public class XmlResource : XmlDocument
    {
        private String m_FileName = "";
        protected GameObject m_ParentNode = null;

        public XmlResource(String filename)
        {
            m_FileName = filename;
        }

        public virtual void Initialise()
        {
        }

        public virtual void Load()
        {
            //Load the the document with the last book node.         
            if (m_FileName != null)
            {
                XmlTextReader reader = new XmlTextReader(m_FileName);
                this.Load(reader);

                // This currently assumes the xml loads ok
                OnLoaded();
            }
        }

        protected virtual void OnLoaded()
        {
            //
        }

        public XmlNode GetNode(String nodeName)
        {
            XmlNode child = null;

            for (int i = 0; i < this.ChildNodes.Count; i++)
            {
                if (child.ChildNodes.Item(i).Name == nodeName)
                    child = child.ChildNodes.Item(i);
            }

            return child;
        }

        public XmlNode GetAttribute( String attrName )
        {
            return this.Attributes.GetNamedItem(attrName);
        }
    }
}
