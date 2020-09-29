namespace XMLValidator.Models.Special
{
    #region Namespaces
    using DocxParser.Models;
    using System;
    using System.Collections.Generic;
    #endregion

    public class XSRelationships
    {
        public const string SECTION_NAME = "Namespaces";
        public static string[] TABLE_HEADER = new string[3] { "Prefix", "Namespace URI", "Reference" };

        public XSRelationships(DocxToC toc, DocxTable table)
        {
            if (toc == null)
            {
                throw new ArgumentException("toc");
            }
            if (table == null)
            {
                throw new ArgumentException("table");
            }
            if (toc.Section != table.Section)
            {
                throw new ArgumentException("mismatch section number of arguments");
            }
            if (toc.Content != SECTION_NAME)
            {
                throw new ArgumentException(String.Format("The section name is not \"Namespaces\" in section {0}", toc.Section));
            }
            if (table.Rows == null || 
                table.Rows[0] == null || 
                table.Rows[0].Count < 2 ||
                table.Rows[0].Count != TABLE_HEADER.Length)
            {
                throw new ArgumentException("table.Rows");
            }

            for (int i = 0; i < table.Rows[0].Count; i++)
            {
                if (table.Rows[0][i] != TABLE_HEADER[i])
                {
                    throw new ArgumentException("table.Rows[0]");
                }
            }

            this.relationships = new Dictionary<string, string>();
            for (int i = 1; i < table.Rows.Count; i++)
            {
                var c = 0;
                this.relationships.Add(table.Rows[i][c++], table.Rows[i][c++]);
            }
        }

        public string this[string key]
        {
            get
            {
                return this.relationships[key];
            }
        }
        
        public Dictionary<string, string> Relationships
        {
            get
            {
                return this.relationships;
            }
        }

        private Dictionary<string, string> relationships;
    }
}
