namespace XMLValidator.Models.XSD
{
    #region Namespaces
    using System;
    using System.Xml;
    #endregion

    public class AttributeInfo : XSDInfo
    {
        public AttributeInfo(XmlNode node)
            : base(node)
        {
            if (this.LocalName != "attribute")
            {
                // DEBUG: Outputs some error message here.
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(node.OuterXml);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("The parameter 'node' is not an attribute.");
                Console.ResetColor();
                return;
            }

            this.Use = node.Attributes["use"] != null ? node.Attributes["use"].Value : String.Empty;
            this.Default = node.Attributes["default"] != null ? node.Attributes["default"].Value : String.Empty;
            this.Form = node.Attributes["form"] != null ? node.Attributes["form"].Value : String.Empty;
            this.Fixed = node.Attributes["fixed"] != null ? node.Attributes["fixed"].Value : String.Empty;
            this.Type = node.Attributes["type"] != null ? node.Attributes["type"].Value : String.Empty;
            if (!String.IsNullOrEmpty(this.Type))
            {
                return;
            }

            var simpleType = node.SelectSingleNode("./xs:simpleType", XSNamespaceManager.Instance);
            if (simpleType != null)
            {
                this.TypeInfo = new SimpleTypeInfo(simpleType);
                return;
            }

            var complexType = node.SelectSingleNode("./xs:complexType", XSNamespaceManager.Instance);
            if (complexType != null)
            {
                this.TypeInfo = new ComplexTypeInfo(complexType);
            }
        }

        public string Type { get; private set; }
        public TypeInfo TypeInfo { get; private set; }
        public string Use { get; private set; }
        public string Default { get; private set; }
        public string Form { get; private set; }
        public string Fixed { get; private set; }
    }
}
