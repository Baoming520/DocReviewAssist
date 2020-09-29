namespace XMLValidator
{
    #region Namespaces
    
    using DocxParser.Models.Extensions;
    using DocxParser.Models.TechDoc;
    using DocxParser.Utils;
    using System;
    using XMLValidator.Extensions;
    using XMLValidator.Models.Special.SSAST;
    #endregion

    class Program
    {
        static void Main(string[] args)
        {
            var tdPath = args[0];
            var td = new TechnicalDocument(tdPath);

            // Load all the prefix-namespace mappings from Namespace section.
            var nsSection = td.TableOfContents.GetSectionNumberByName("Namespaces");
            var nsToC = td.TableOfContents.GetToC(nsSection);
            var nsTable = td.Tables.GetTables(nsSection);
            var xsNS = new DocxParser.Utils.XS.XSNamespace(nsToC, nsTable[0], "Namespaces", new string[] { "Prefix", "Namespace URI", "Reference" });


            td.CompareCodeWithTableDefsForDTSX2(xsNS);



            //var context = new CodeTableContext();
            //var vRes = td.CompareCodeWithTableDefsForSSAS(xsNS);

            //var vResults = td.CompareCodeWithRestrictTableForSSAST(context);
            //var vResults = td.CompareCodeWithDefineTableForSSASTDiscover();
            //var vResults = td.CompareCodeWithDefineTableForSSASTExecute("^3\\.1\\.5\\.2\\.1\\.1\\.1\\.[0-9]+$", "Create");
            //var vResults = td.CompareCodeWithDefineTableForSSASTExecute("^3\\.1\\.5\\.2\\.1\\.2\\.1\\.[0-9]+$", "Alter");
            //var vResults = td.CompareCodeWithDefineTableForSSASTExecute("^3\\.1\\.5\\.2\\.1\\.3\\.1\\.[0-9]+$", "Delete");
            //var vResults = td.CompareCodeWithDefineTableForSSASTExecute("^3\\.1\\.5\\.2\\.1\\.4\\.1\\.[0-9]+$", "Rename");
            //var vResults = td.CompareCodeWithDefineTableForSSASTExecute("^3\\.1\\.5\\.2\\.1\\.5\\.1\\.[0-9]+$", "Refresh");
        }
    }
}
