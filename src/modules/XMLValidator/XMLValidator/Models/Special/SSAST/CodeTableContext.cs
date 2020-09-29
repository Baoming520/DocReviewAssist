namespace XMLValidator.Models.Special.SSAST
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using XMLValidator.Utils;
    #endregion

    public class CodeTableContext
    {
        public CodeTableContext()
        {
            this.SectionPatterns = new List<string>();
            this.ColumnMappingsList = new List<Mappings<string, string>>();

            this.Initialize();
        }

        public int Count
        {
            get
            {
                return this.SectionPatterns.Count;
            }
        }

        public List<string> SectionPatterns { get; set; }
        public List<Mappings<string, string>> ColumnMappingsList { get; set; }
        public CodeTableInfo this[int index]
        {
            get
            {
                return new CodeTableInfo(this.SectionPatterns[index], this.ColumnMappingsList[index]);
            }
        }

        private void Initialize()
        {
            // Message: Discover
            // For section 3.1.5.1.1.x.2.1 Column
            this.SectionPatterns.Add("^3\\.1\\.5\\.1\\.1\\.[0-9]+\\.2\\.1$");
            var columnMappings = new Mappings<string, string>();
            columnMappings.Add("Name", "PropertyName");
            columnMappings.Add("Restriction", String.Empty);
            ColumnMappingsList.Add(columnMappings);

            // Message: Execute
            // For section 3.1.5.2.1.1.1.x Create <Object>
            this.SectionPatterns.Add("^3\\.1\\.5\\.2\\.1\\.1\\.1\\.[0-9]+$");
            columnMappings = new Mappings<string, string>();
            columnMappings.Add("Element", "PropertyName");
            columnMappings.Add("Default value", "DefaultValue");
            ColumnMappingsList.Add(columnMappings);

            // Message: Execute
            // For section 3.1.5.2.1.2.1.x Alter <Object>
            this.SectionPatterns.Add("^3\\.1\\.5\\.2\\.1\\.2\\.1\\.[0-9]+$");
            columnMappings = new Mappings<string, string>();
            columnMappings.Add("Element", "PropertyName");
            ColumnMappingsList.Add(columnMappings);

            // Message: Execute
            // For section 3.1.5.2.1.3.1.x Delete <Object>
            this.SectionPatterns.Add("^3\\.1\\.5\\.2\\.1\\.3\\.1\\.[0-9]+$");
            columnMappings = new Mappings<string, string>();
            columnMappings.Add("Element", "PropertyName");
            ColumnMappingsList.Add(columnMappings);

            // Message: Execute
            // For section 3.1.5.2.1.4.1.x Rename <Object>
            this.SectionPatterns.Add("^3\\.1\\.5\\.2\\.1\\.4\\.1\\.[0-9]+$");
            columnMappings = new Mappings<string, string>();
            columnMappings.Add("Element", "PropertyName");
            ColumnMappingsList.Add(columnMappings);

            // Message: Execute
            // For section 3.1.5.2.1.5.1.x Refresh <Object> | Out-of-Line Bindings | Pushed Data
            this.SectionPatterns.Add("^3\\.1\\.5\\.2\\.1\\.5\\.1\\.[0-9]+$");
            columnMappings = new Mappings<string, string>();
            columnMappings.Add("Element", "PropertyName");
            columnMappings.Add("Default value", "DefaultValue");
            columnMappings.Add("Description", "PropertyDescription");
            ColumnMappingsList.Add(columnMappings);
        }
    }

    public class CodeTableInfo
    {
        public CodeTableInfo(string sectionPattern, Mappings<string, string> colMappings)
        {
            this.SectionPattern = sectionPattern;
            this.ColumnMappings = colMappings;
        }
        public string SectionPattern { get; set; }
        public Mappings<string, string> ColumnMappings { get; set; }
    }
}
