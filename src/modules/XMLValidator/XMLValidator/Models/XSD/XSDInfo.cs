namespace XMLValidator.Models.XSD
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Xml;
    #endregion

    public class XSDInfo
    {
        public static List<XSDInfo> Parse(XSCode xsCode)
        {
            var ret = new List<XSDInfo>();
            var elems = xsCode.Node.SelectNodes("/xs:schema/xs:element", XSNamespaceManager.Instance);
            if (elems.Count > 0)
            {
                foreach (XmlNode elem in elems)
                {
                    ret.Add(new ElementInfo(elem));
                }
            }

            var attrs = xsCode.Node.SelectNodes("/xs:schema/xs:attribute", XSNamespaceManager.Instance);
            if (attrs.Count > 0)
            {
                foreach (XmlNode attr in attrs)
                {
                    ret.Add(new AttributeInfo(attr));
                }
            }

            var attrGroups = xsCode.Node.SelectNodes("/xs:schema/xs:attributeGroup", XSNamespaceManager.Instance);
            if (attrGroups.Count > 0)
            {
                foreach (XmlNode attrGroup in attrGroups)
                {
                    ret.Add(new AttributeGroupInfo(attrGroup));
                }
            }

            var simpleTypes = xsCode.Node.SelectNodes("/xs:schema/xs:simpleType", XSNamespaceManager.Instance);
            if (simpleTypes.Count > 0)
            {
                foreach (XmlNode simpleType in simpleTypes)
                {
                    ret.Add(new SimpleTypeInfo(simpleType));
                }
            }

            var complexTypes = xsCode.Node.SelectNodes("/xs:schema/xs:complexType", XSNamespaceManager.Instance);
            if (complexTypes.Count > 0)
            {
                foreach (XmlNode complexType in complexTypes)
                {
                    ret.Add(new ComplexTypeInfo(complexType));
                }
            }

            return ret;
        }

        public XSDInfo(XmlNode node)
        {
            if (node == null)
            {
                // DEBUG: Outputs some error message here.
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(node);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("The parameter 'node' is nullable.");
                Console.ResetColor();
                return;
            }

            this.node = node;
            this.LocalName = node.LocalName;
            this.Name = node.Attributes["name"] != null ? node.Attributes["name"].Value : String.Empty;
        }

        public string LocalName { get; set; }
        public string Name { get; set; }

        public List<XSDInfo> GetElements()
        {
            return this.GetXSDInfos(".//xs:element");
        }

        public List<XSDInfo> GetAttributes()
        {
            return this.GetXSDInfos(".//xs:attribute");
        }

        public List<XSDInfo> GetAttributeGroups()
        {
            return this.GetXSDInfos(".//xs:attributeGroup");
        }

        public List<XSDInfo> GetXSDInfos(string xPath)
        {
            if (String.IsNullOrEmpty(xPath))
            {
                // DEBUG: Outputs some error message here.
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("The value of parameter 'xPath' is invalid.");
                Console.ResetColor();

                return null;
            }

            List<XSDInfo> ret = null;
            var targets = this.node.SelectNodes(xPath, XSNamespaceManager.Instance);
            if (targets != null && targets.Count > 0)
            {
                ret = new List<XSDInfo>();
                foreach (XmlNode target in targets)
                {
                    if (xPath.EndsWith("xs:element"))
                    {
                        ret.Add(new ElementInfo(target));
                    }
                    else if (xPath.EndsWith("xs:attribute"))
                    {
                        ret.Add(new AttributeInfo(target));
                    }
                    else if (xPath.EndsWith("xs:attributeGroup"))
                    {
                        ret.Add(new AttributeGroupInfo(target));
                    }
                    else
                    {
                        ret.Add(new XSDInfo(target));
                    }
                }
            }

            return ret;
        }

        protected XmlNode node;
    }
}
