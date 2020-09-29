namespace DocxParser.Models.Code
{
    #region Namespaces.
    using System;
    using System.Collections.Generic;
    #endregion

    public class CodeCollectorForTDS : CodeCollector
    {
        public CodeCollectorForTDS(IEnumerable<DocxCode> codes)
            : base(codes)
        {
        }

        public List<TDSHexCode> GenHexStringMappings()
        {
            var ret = new List<TDSHexCode>();
            foreach (var code in this.Codes)
            {
                var hexCode = TDSHexCode.Parse(code);
                if (hexCode != null)
                {
                    ret.Add(hexCode);
                }
            }

            return ret;
        }
    }

    public class TDSHexCode
    {
        public static TDSHexCode Parse(DocxCode code)
        {
            if (code == null)
            {
                return null;
            }

            if (code.Section.StartsWith("4."))
            {
                var snippets = code.Code.Split(new String[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (snippets.Length != 2)
                {
                    return null;
                }
                
                return new TDSHexCode(code.Section, snippets[0], snippets[1]);
            }

            return null;
        }

        public TDSHexCode(string section, string hexCode, string xmlCode)
        {
            this.Section = section;
            this.HexCode = hexCode;
            this.XmlCode = xmlCode;
        }

        public string Section { get; private set; }
        public string HexCode { get; private set; }
        public string XmlCode { get; private set; }
    }

    
}
