namespace XMLValidator.Models
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Xml;
    #endregion

    public class XSNamespaceManager : XmlNamespaceManager
    {
        public static XSNamespaceManager Instance
        {
            get
            {
                return XSNamespaceManager.instance;
            }
        }

        public static void Initialize(Dictionary<string, string> relationships)
        {
            XSNamespaceManager.instance = new XSNamespaceManager(relationships);
        }

        private static XSNamespaceManager instance;

        private XSNamespaceManager(Dictionary<string, string> relationships)
            : base(new NameTable())
        {
            this.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
            foreach (var item in relationships)
            {
                this.AddNamespace(item.Key, item.Value);
            }
        }
    }
}