namespace DocxParser.Models.Code
{
    #region Namespaces
    using DocxParser.Utils;
    using DocxParser.Utils.XS;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    #endregion
    public class XMLCodeCollector : CodeCollector
    {
        public XMLCodeCollector(IEnumerable<DocxCode> codes, XSNamespace xsNamespace)
            : base(codes)
        {
            this.xsNamespace = xsNamespace;
            this.nsManager = new XSNamespaceManager(this.xsNamespace.Relationships);
            this.CodeTypes = new List<string>();
            this.hasRefCode = new bool[codes.Count()];
        }

        public new void Group()
        {
            base.Group();
            int codeIdx = 0;
            foreach (var code in this.Codes)
            {
                bool? isXSDCode = this.IsXSDCode(code.Code);
                this.CodeTypes.Add(isXSDCode != null ? (isXSDCode == true ? "xsd" : "xml") : "unknown");
                if (isXSDCode == null)
                {
                    continue;
                }

                try
                {
                    this.hasRefCode[codeIdx] = true;
                    var xmlDoc = new XmlDocument();
                    if (isXSDCode == true)
                    {
                        var xmlCode = this.HandleXSDCodeSnippet(code.Code);
                        xmlDoc.LoadXml(xmlCode);
                        var refElems = xmlDoc.OuterXml.SelectAll("//*[@ref]", this.nsManager);
                        if (refElems == null || refElems.Count == 0)
                        {
                            this.hasRefCode[codeIdx++] = false;
                            continue;
                        }

                        var refDict = new Dictionary<string, string>();
                        foreach (var refElem in refElems)
                        {
                            string refName = refElem.Attribute("ref").Value;
                            string localName = refElem.Name.LocalName;
                            string sectionNum = code.Section;
                            var refCode = this.GetRefCode(refName, localName, sectionNum);
                            refDict.Add(refName, refCode);
                        }

                        //this.RefCodes.Add(refDict);
                    }
                    else if (isXSDCode == false)
                    {
                        xmlDoc.LoadXml(this.FixXMLCode(code.Code));
                    }
                }
                catch (Exception ex)
                {
                    this.CodeTypes[this.CodeTypes.Count - 1] = "invalid";
                }

            }
        }

        /// <summary>
        /// Uses very primitive method to judge if a string is with xml, xsd or unknow format.
        /// TODO: This method should be refined later.
        /// </summary>
        /// <param name="xmlCode">Input a code string.</param>
        /// <returns>Returns true for xsd code, and false for xml code, otherwise null.</returns>
        public bool? IsXSDCode(string xmlCode)
        {
            if (xmlCode.Contains(XML_HEAD))
            {
                xmlCode = xmlCode.Replace(XML_HEAD, "");
            }

            xmlCode = this.FixXMLCode(xmlCode);
            if (xmlCode.StartsWith("<xsd:") || xmlCode.StartsWith("<xs:"))
            {
                return true;
            }
            else if (xmlCode.StartsWith("<"))
            {
                return false;
            }

            return null;
        }

        public string HandleXSDCodeSnippet(string xmlCode)
        {
            xmlCode = this.FixXMLCode(xmlCode);
            if (!xmlCode.StartsWith("<xsd:schema") || !xmlCode.StartsWith("<xs:schema"))
            {
                if (xmlCode.StartsWith("<xsd:"))
                {
                    return this.GenXSWrapper().Replace(PH_CODE_SNIPPET, xmlCode);
                }
                else
                {
                    return this.GenXSWrapper("xs").Replace(PH_CODE_SNIPPET, xmlCode);
                }
            }

            return xmlCode;
        }

        [JsonProperty("code_types")]
        public List<string> CodeTypes { get; private set; }
        public Dictionary<string, List<RefCodeInfo>> RefCodes { get; private set; }

        [JsonIgnore]
        public List<DocxCode> NotCodes { get; private set; }

        #region Private members
        private const string PH_CODE_SNIPPET = "$CODE_SNIPPET$";
        private const string XML_HEAD = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
        private XSNamespace xsNamespace;
        private XSNamespaceManager nsManager;
        private bool[] hasRefCode;

        private string GenXSWrapper(string xsPrefixName = "xsd")
        {
            if (this.xsNamespace == null)
            {
                throw new ArgumentNullException("xsNamespace", "The member 'xsNamespace' must not be set to null.");
            }

            string attributes = String.Empty;
            foreach (var prefix in xsNamespace.Relationships.Keys)
            {
                attributes += String.Format("xmlns:{0}=\"{1}\" \r\n", prefix, xsNamespace[prefix]);
            }

            return String.Format("<{0}:schema {1}>{2}</{3}:schema>", xsPrefixName, attributes, PH_CODE_SNIPPET, xsPrefixName);
        }


        private string FixXMLCode(string xmlCode)
        {
            if (xmlCode.Contains(XML_HEAD))
            {
                xmlCode = xmlCode.Replace(XML_HEAD, "");
            }

            return xmlCode
                .Trim(' ')
                .Trim('\r')
                .Trim('\n')
                .Trim('\r')
                .Trim(' ');
        }

        private string GetRefCode(string refName, string elementName, string citedSection)
        {
            var cites = new List<Cite>();
            string[] splits = refName.Split(':');
            string prefix = splits.Length > 1 ? splits[0] : String.Empty;
            refName = splits.Length > 1 ? splits[1] : splits[0];
            string xPath = String.Format("/xs:{0}[@name='{1}']", elementName, refName);
            foreach (var code in this.Codes)
            {
                if (this.IsXSDCode(code.Code) != true)
                {
                    continue;
                }

                try
                {
                    var xmlDoc = new XmlDocument();
                    var xmlCode = this.HandleXSDCodeSnippet(code.Code);
                    xmlDoc.LoadXml(xmlCode);
                    var tElem = xmlDoc.OuterXml.Select(xPath, this.nsManager);
                    if (tElem != null)
                    {
                        cites.Add(new Cite(citedSection, code.Section, tElem.ToString()));
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                    continue;
                }

            }

            //Console.WriteLine("[{0}: {1}->{2}] Cited Codes' Count: " + cites.Count, citedSection, elementName, refName);
            //foreach (var res in cites)
            //{
            //    Console.WriteLine(res.Section + ":");
            //    Console.WriteLine(res.Content);
            //}

            var cite = Cite.GetAppropriateCite(cites);
            //Console.ForegroundColor = ConsoleColor.Green;
            //Console.WriteLine(cite);
            //Console.ResetColor();

            return cite.Content ?? null;
        }

        #endregion
    }

    public class RefCodeInfo
    {
        public string Prefix { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
