namespace XMLValidator.Models
{
    #region Namespaces
    using System;
    using System.Xml;
    using System.Xml.Linq;
    #endregion

    public class XSCode
    {
        public XSCode(string xmlCode)
        {
            this.Code = xmlCode;
            try
            {
                this.Element = XElement.Parse(xmlCode);
                this.Node = new XmlDocument();
                this.Node.LoadXml(xmlCode);
                if (this.Element.Name.LocalName == "schema")
                {
                    this.ErrorCode = 0;
                    this.Message = "Success";
                }
                else
                {
                    this.ErrorCode = 1;
                    this.Message = "The XML string is not a XML schema.";
                }
                
            }
            catch (Exception ex)
            {
                this.Code = String.Empty;
                this.Element = null;
                this.Node = null;
                this.ErrorCode = -1;
                this.Message = ex.Message;
            }
        }

        public string Code { get; private set; }

        public XElement Element { get; private set; }

        public XmlDocument Node { get; private set; }

        public int ErrorCode { get; private set; }

        public string Message { get; private set; }
    }
}
