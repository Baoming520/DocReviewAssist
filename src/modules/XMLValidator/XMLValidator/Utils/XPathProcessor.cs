namespace XMLValidator.Utils
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using XMLValidator.Models;
    #endregion

    public static class XPathProcessor
    {
        public static XElement Select(this string xml, string xPath)
        {
            if (XSNamespaceManager.Instance == null)
            {
                throw new NullReferenceException("initialize the instance of XSNamespaceManager first");
            }

            XElement xelem = null;
            XElement res = null;
            try
            {
                xelem = XElement.Parse(xml);
                res = xelem.XPathSelectElement(xPath, XSNamespaceManager.Instance);
            }
            catch
            {
                return res;
            }

            return res;
        }

        public static List<XElement> SelectAll(this string xml, string xPath)
        {
            if (XSNamespaceManager.Instance == null)
            {
                throw new NullReferenceException("initialize the instance of XSNamespaceManager first");
            }

            XElement xelem = null;
            IEnumerable<XElement> xelems = null;
            try
            {
                xelem = XElement.Parse(xml);
                xelems = xelem.XPathSelectElements(xPath, XSNamespaceManager.Instance);
            }
            catch
            {
                return null;
            }

            return xelems.ToList();
        }
    }
}
