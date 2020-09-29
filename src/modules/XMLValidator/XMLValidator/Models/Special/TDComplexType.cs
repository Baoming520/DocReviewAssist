namespace XMLValidator.Models.Special
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    #endregion

    public class TDComplexType : XSComplexType
    {
        public TDComplexType() 
            : base()
        {
            this.Section = String.Empty;
            this.PropertyDescription = new List<string>();
            this.DefaultValues = new List<string>();
        }

        public TDComplexType(XSCode xsCode, string section) 
            : base(xsCode)
        {
            this.Section = section;
        }

        public string Section { get; set; }
        public List<string> DefaultValues { get; set; }
        public List<string> PropertyDescription { get; set; }
    }
}
