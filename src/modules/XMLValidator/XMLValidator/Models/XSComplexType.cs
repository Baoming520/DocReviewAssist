namespace XMLValidator.Models
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using XMLValidator.Utils;
    #endregion

    public class XSComplexType
    {
        public XSComplexType()
        {
            this.Name = String.Empty;
            this.SequenceProperties = new List<XSProperty>();
            this.Attributes = new List<XSAttribute>();
            this.ErrorCode = 0;
            this.Message = "Success";
        }

        public XSComplexType(XSCode xsCode)
        {
            this.ErrorCode = 0;
            this.Message = "Success";
            if (xsCode == null)
            {
                throw new ArgumentNullException("xsCode");
            }

            if (xsCode.ErrorCode != 0)
            {
                throw new ArgumentException("xsCode");
            }

            this.SequenceProperties = new List<XSProperty>();
            this.Attributes = new List<XSAttribute>();
            try
            {
                var ctNode = xsCode.Node.SelectSingleNode("//xs:complexType[@name]", XSNamespaceManager.Instance);
                if (ctNode == null)
                {
                    this.ErrorCode = 1;
                    this.Message = "The complex type with a 'name' attribute is not found.";

                    return;
                }

                this.Name = ctNode.Attributes["name"].Value;
                var seqPropNodes = ctNode.SelectNodes("./xs:sequence/xs:element", XSNamespaceManager.Instance);
                if (seqPropNodes == null)
                {
                    this.ErrorCode = 1;
                    this.Message = "The properties of complex type are not found.";
                    return;
                }

                foreach (XmlNode seqPropNode in seqPropNodes)
                {
                    this.SequenceProperties.Add(XSProperty.Parse(seqPropNode));
                }

                var attributeNodes = ctNode.SelectNodes("./xs:attribute", XSNamespaceManager.Instance);
                foreach (XmlNode attrNode in attributeNodes)
                {
                    this.Attributes.Add(XSAttribute.Parse(attrNode));
                }
            }
            catch (Exception ex)
            {
                this.ErrorCode = -1;
                this.Message = ex.Message;
            }
        }

        public string Name { get; set; }
        public List<XSProperty> SequenceProperties { get; set; }
        public List<XSAttribute> Attributes { get; set; }
        public int ErrorCode { get; set; }
        public string Message { get; set; }
    }

    public class XSProperty
    {
        public static XSProperty Parse(XmlNode propNode)
        {
            if (propNode.Attributes["ref"] != null)
            {
                return new XSProperty(propNode.Attributes["ref"].Value, "ref", "1", "1");
            }

            var ret = new XSProperty();
            if (propNode.Attributes["name"] != null)
            {
                ret.Name = propNode.Attributes["name"].Value;
            }

            if (propNode.Attributes["type"] != null)
            {
                ret.Type = propNode.Attributes["type"].Value;
            }
            else
            {
            }

            if (propNode.Attributes["minOccurs"] != null)
            {
                ret.MinOccurs = propNode.Attributes["minOccurs"].Value;
            }

            if (propNode.Attributes["maxOccurs"] != null)
            {
                ret.MaxOccurs = propNode.Attributes["maxOccurs"].Value;
            }

            return ret;
        }

        public XSProperty() { }

        public XSProperty(string propName, string propType, string minOccurs, string maxOccurs)
        {
            this.Name = propName;
            this.Type = propType;
            this.MinOccurs = minOccurs;
            this.MaxOccurs = maxOccurs;
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public string MinOccurs { get; set; }
        public string MaxOccurs { get; set; }
    }

    public class XSAttribute
    {
        public static XSAttribute Parse(XmlNode attrNode)
        {
            var ret = new XSAttribute();
            if (attrNode.Attributes["name"] != null)
            {
                ret.Name = attrNode.Attributes["name"].Value;
            }

            if (attrNode.Attributes["type"] != null)
            {
                ret.Type = attrNode.Attributes["type"].Value;
            }
            else
            {
                var restNode = attrNode.SelectSingleNode("/simpleType/restriction");
                if (restNode != null)
                {
                    ret.Type = restNode.Attributes["base"].Value;
                }
                else
                {
                    ret.Type = "unknown";
                }
            }

            if (attrNode.Attributes["use"] != null)
            {
                ret.Use = attrNode.Attributes["use"].Value;
            }

            if (attrNode.Attributes["default"] != null)
            {
                ret.Default = attrNode.Attributes["default"].Value;
            }

            return ret;
        }

        public XSAttribute() { }
        public XSAttribute(string propName, string propType, string use, string defaultVal, string form)
        {
            this.Name = propName;
            this.Type = propType;
            this.Use = use;
            this.Default = defaultVal;
            this.Form = form;
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public string Use { get; set; }
        public string Default { get; set; }
        public string Form { get; set; }
    }

    public class XSAttributeGroup
    {
        public static XSAttributeGroup Parse(XmlNode attrGroupNode)
        {
            var ret = new XSAttributeGroup();
            ret.Name = attrGroupNode.Attributes["name"].Value;
            var attributeNodes = attrGroupNode.SelectNodes("./xs:attribute", XSNamespaceManager.Instance);
            foreach (XmlNode attrNode in attributeNodes)
            {
                var attr = XSAttribute.Parse(attrNode);
                if (attr != null)
                {
                    ret.Attributes.Add(attr);
                }
            }

            return ret;
        }

        public string Name { get; set; }
        public List<XSAttribute> Attributes { get; set; }
    }
}
