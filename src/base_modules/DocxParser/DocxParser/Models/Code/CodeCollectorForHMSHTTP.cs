namespace DocxParser.Models.Code
{
    #region Namespaces.
    using System.Collections.Generic;
    #endregion

    public class CodeCollectorForHMSHTTP : CodeCollector
    {
        public CodeCollectorForHMSHTTP(IEnumerable<DocxCode> codes)
            : base(codes)
        {
        }
    }
}
