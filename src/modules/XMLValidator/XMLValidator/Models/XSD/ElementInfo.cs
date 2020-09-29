namespace XMLValidator.Models.XSD
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Xml;
    #endregion

    public class ElementInfo : XSDInfo
    {
        public ElementInfo(XmlNode node)
            : base(node)
        {
            if (this.LocalName != "element")
            {
                // DEBUG: Outputs some error message here.
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(node.OuterXml);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("The parameter 'node' is not an element.");
                Console.ResetColor();
                return;
            }

            this.IsExtension = false; // default value
            this.IsExclusive = false; // default value
            this.MinOccurs = node.Attributes["minOccurs"] != null ? node.Attributes["minOccurs"].Value : String.Empty;
            this.MaxOccurs = node.Attributes["maxOccurs"] != null ? node.Attributes["maxOccurs"].Value : String.Empty;
            this.Form = node.Attributes["form"] != null ? node.Attributes["form"].Value : String.Empty;
            this.Ref = node.Attributes["ref"] != null ? node.Attributes["ref"].Value : String.Empty;
            if (!String.IsNullOrEmpty(this.Ref))
            {
                return;
            }

            this.Type = node.Attributes["type"] != null ? node.Attributes["type"].Value : String.Empty;
            if (String.IsNullOrEmpty(this.Type))
            {
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
        }

        public string Ref { get; private set; }
        public string Type { get; private set; }
        public TypeInfo TypeInfo { get; private set; }
        public string MinOccurs { get; private set; }
        public string MaxOccurs { get; private set; }
        public string Form { get; private set; }
        public bool IsExtension { get; set; }
        public bool IsExclusive { get; set; }
        //public List<ElementInfo> ExclusiveElements { get; set; }

        /// <summary>
        /// At least one of the following members MUST be existent:
        /// 1: Ref
        /// 2: Type
        /// 3: TypeInfo
        /// </summary>
        /// <returns>Returns the validation result.</returns>
        public bool Validate()
        {
            bool res = !String.IsNullOrEmpty(this.Ref);
            if (!res)
            {
                res = !String.IsNullOrEmpty(this.Type);
            }
            if (!res)
            {
                res = this.TypeInfo != null;
            }

            return res;
        }
    }
}
