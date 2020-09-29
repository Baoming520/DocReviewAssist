namespace XMLValidator.Models.XSD
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Xml;
    #endregion

    public class AttributeGroupInfo : XSDInfo
    {
        public AttributeGroupInfo(XmlNode node)
            : base(node)
        {
            if (this.LocalName != "attributeGroup")
            {
                // DEBUG: Outputs some error message here.
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(node.OuterXml);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("The parameter 'node' is not an attributeGroup.");
                Console.ResetColor();
                return;
            }

            this.Attributes = new List<AttributeInfo>();
            this.Ref = node.Attributes["ref"] != null ? node.Attributes["ref"].Value : String.Empty;
            if (!String.IsNullOrEmpty(this.Ref))
            {
                return;
            }

            var attrs = node.SelectNodes("./xs:attribute", XSNamespaceManager.Instance);
            if (attrs != null && attrs.Count > 0)
            {
                foreach (XmlNode attr in attrs)
                {
                    this.Attributes.Add(new AttributeInfo(attr));
                }
            }
        }

        public string Ref { get; private set; }
        public List<AttributeInfo> Attributes { get; private set; }
    }
}
