namespace XMLValidator.Models.XSD
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Xml;
    #endregion

    public class TypeInfo : XSDInfo
    {
        public TypeInfo(XmlNode node) 
            : base(node)
        {
            if (this.LocalName != "simpleType" && this.LocalName != "complexType")
            {
                // DEBUG: Outputs some error message here.
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(node.OuterXml);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("The parameter 'node' is not a type.");
                Console.ResetColor();

                return;
            }
        }

        public string Base { get; protected set; }
    }

    public class SimpleTypeInfo : TypeInfo
    {
        public SimpleTypeInfo(XmlNode node)
            : base(node)
        {
            if (this.LocalName != "simpleType")
            {
                // DEBUG: Outputs some error message here.
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(node.OuterXml);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("The parameter 'node' is not a simpleType.");
                Console.ResetColor();
                return;
            }

            this.EnumValues = new List<string>();
            XmlNode rest = null;
            XmlNodeList enums = null;

            // Handle the "union" element in its children.
            var union = node.SelectSingleNode("./xs:union", XSNamespaceManager.Instance);
            if (union != null)
            {
                rest = union.SelectSingleNode("./xs:simpleType/xs:restriction", XSNamespaceManager.Instance);
                if (rest != null)
                {
                    this.Base = rest.Attributes["base"] != null ? rest.Attributes["base"].Value : String.Empty;
                    enums = rest.SelectNodes("./xs:enumeration", XSNamespaceManager.Instance);
                    if (enums != null && enums.Count > 0)
                    {
                        foreach (XmlNode en in enums)
                        {
                            // DEBUG: Outputs some error message here.
                            if (en.Attributes["value"] == null)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine("The enumeration has no attribute named as 'value'.");
                                Console.ResetColor();
                                continue;
                            }

                            this.EnumValues.Add(en.Attributes["value"].Value);
                        }
                    }

                    return;
                }
            }

            rest = node.SelectSingleNode("./xs:restriction", XSNamespaceManager.Instance);
            if (rest != null)
            {
                this.Base = rest.Attributes["base"] != null ? rest.Attributes["base"].Value : String.Empty;
                enums = rest.SelectNodes("./xs:enumeration", XSNamespaceManager.Instance);
                if (enums != null && enums.Count > 0)
                {
                    foreach (XmlNode en in enums)
                    {
                        // DEBUG: Outputs some error message here.
                        if (en.Attributes["value"] == null)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine("The enumeration has no attribute named as 'value'.");
                            Console.ResetColor();
                            continue;
                        }

                        this.EnumValues.Add(en.Attributes["value"].Value);
                    }
                }

                var minIncl = rest.SelectSingleNode("./xs:minInclusive", XSNamespaceManager.Instance);
                this.MinInclusive = minIncl != null && minIncl.Attributes["value"] != null ? minIncl.Attributes["value"].Value : String.Empty;
                var maxIncl = rest.SelectSingleNode("./xs:maxInclusive", XSNamespaceManager.Instance);
                this.MaxInclusive = maxIncl != null && maxIncl.Attributes["value"] != null ? maxIncl.Attributes["value"].Value : String.Empty;

                return;
            }
        }

        public bool IsUnion { get; private set; }
        public string MemberType { get; private set; }

        public List<string> EnumValues { get; private set; }
        public string MinInclusive { get; private set; }
        public string MaxInclusive { get; private set; }
    }

    public class ComplexTypeInfo : TypeInfo
    {
        public ComplexTypeInfo(XmlNode node)
            : base(node)
        {
            if (this.LocalName != "complexType")
            {
                // DEBUG: Outputs some error message here.
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(node.OuterXml);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("The parameter 'node' is not a complexType.");
                Console.ResetColor();
                return;
            }

            this.Elements = new List<ElementInfo>();
            this.Attributes = new List<AttributeInfo>();
            this.AttributeGroups = new List<AttributeGroupInfo>();

            var elems = node.SelectNodes(".//xs:sequence/xs:element", XSNamespaceManager.Instance);
            if (elems != null && elems.Count > 0)
            {
                foreach (XmlNode elem in elems)
                {
                    this.Elements.Add(new ElementInfo(elem));
                }
            }

            elems = node.SelectNodes(".//xs:choice/xs:element", XSNamespaceManager.Instance);
            if (elems != null && elems.Count > 0)
            {
                foreach (XmlNode elem in elems)
                {
                    var elemInfo = new ElementInfo(elem);
                    elemInfo.IsExclusive = true;
                    this.Elements.Add(elemInfo);
                }
            }

            var attrs = node.SelectNodes("./xs:attribute", XSNamespaceManager.Instance);
            if (attrs != null && attrs.Count > 0)
            {
                foreach (XmlNode attr in attrs)
                {
                    this.Attributes.Add(new AttributeInfo(attr));
                }
            }

            var attrGroups = node.SelectNodes("./xs:attributeGroup", XSNamespaceManager.Instance);
            if (attrGroups != null && attrGroups.Count > 0)
            {
                foreach (XmlNode attrGroup in attrGroups)
                {
                    this.AttributeGroups.Add(new AttributeGroupInfo(attrGroup));
                }
            }

            var ext = node.SelectSingleNode("./xs:complexContent/xs:extension", XSNamespaceManager.Instance);
            if (ext != null)
            {
                this.Base = ext.Attributes["base"] != null ? ext.Attributes["base"].Value : String.Empty;
                elems = ext.SelectNodes("./xs:element", XSNamespaceManager.Instance);
                if (elems != null && elems.Count > 0)
                {
                    foreach (XmlNode elem in elems)
                    {
                        var elemInfo = new ElementInfo(elem);
                        elemInfo.IsExtension = true;
                        this.Elements.Add(elemInfo);
                    }
                }

                attrs = ext.SelectNodes("./xs:attribute", XSNamespaceManager.Instance);
                if (attrs != null && attrs.Count > 0)
                {
                    foreach (XmlNode attr in attrs)
                    {
                        this.Attributes.Add(new AttributeInfo(attr));
                    }
                }
                return;
            }

            ext = node.SelectSingleNode("./xs:simpleContent/xs:extension", XSNamespaceManager.Instance);
            if (ext != null)
            {
                this.Base = ext.Attributes["base"] != null ? ext.Attributes["base"].Value : String.Empty;
                elems = ext.SelectNodes("./xs:element", XSNamespaceManager.Instance);
                if (elems != null && elems.Count > 0)
                {
                    foreach (XmlNode elem in elems)
                    {
                        var elemInfo = new ElementInfo(elem);
                        elemInfo.IsExtension = true;
                        this.Elements.Add(elemInfo);
                    }
                }

                attrs = ext.SelectNodes("./xs:attribute", XSNamespaceManager.Instance);
                if (attrs != null && attrs.Count > 0)
                {
                    foreach (XmlNode attr in attrs)
                    {
                        this.Attributes.Add(new AttributeInfo(attr));
                    }
                }
                return;
            }
        }

        public List<ElementInfo> Elements { get; set; }
        public List<AttributeInfo> Attributes { get; set; }
        public List<AttributeGroupInfo> AttributeGroups { get; set; }
    }
}
