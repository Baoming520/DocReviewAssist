namespace XMLValidator.Models.Special
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using XMLValidator.Utils;
    #endregion

    public class DefinitionTable
    {
        public static DefinitionTable GetDefinitionTable(IEnumerable<DefinitionTable> defTables, string category)
        {
            if (defTables == null || defTables.Count() == 0)
            {
                return null;
            }

            foreach (var defTbl in defTables)
            {
                if (String.Equals(defTbl.Category, category, StringComparison.OrdinalIgnoreCase))
                {
                    return defTbl;
                }
            }

            return null;
        }

        public DefinitionTable(string[,] rows)
        {
            if (rows == null || rows.Length <= 1)
            {
                // DEBUG: Outputs some error message here.
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("The length of parameter 'rows' is invalid.");
                Console.ResetColor();

                return;
            }

            this.Category = rows[0, 0]; // the category info is located at the position (0, 0) of the table.
            this.Rows = DefinitionRow.Parse(rows);
        }

        /// <summary>
        /// The valid strings is as follows:
        /// 1: Element
        /// 2: Attribute
        /// 3: Enumeration value
        /// ...
        /// </summary>
        public string Category { get; set; }
        public List<DefinitionRow> Rows { get; set; }

        public bool Test(string name, string type, out string message)
        {
            message = String.Empty;
            foreach (var r in this.Rows)
            {
                // Use the name as the key.
                if (name != r.Name)
                {
                    continue;
                }

                if (String.IsNullOrEmpty(type))
                {
                    return true;
                }
                
                if (type.EndsWith(r.Type.Trim()))
                {
                    return true;
                }
                else
                {
                    message += String.Format("The type of '{0}' which is defined in XSD is inconsistent with the one defined in the table.\n", name);
                    message += String.Format("XSD Code: {0}\n", type);
                    message += String.Format("DEF Table: {0}\n", r.Type);

                    return false;
                }
            }

            message += string.Format("'{0}' is not found in the definition table.\n", name);

            return false;
        }
    }

    public class DefinitionRow
    {
        public static List<DefinitionRow> Parse(string[,] rows)
        {
            if (rows == null || rows.Length <= 1)
            {
                return null;
            }

            var mappings = new Mappings<string, int>();
            var dRows = new List<DefinitionRow>();
            for (int i = 0; i < rows.GetLength(0); i++)
            {
                for (int j = 0; j < rows.GetLength(1); j++)
                {
                    if (i == 0)
                    {
                        mappings.Add(rows[i, j], j);
                    }
                    else
                    {
                        break;
                    }
                }

                if (i == 0)
                {
                    continue;
                }

                string type = mappings.ItemList1.Contains(TYPE) ? rows[i, mappings[TYPE]] : String.Empty;
                string description = mappings.ItemList1.Contains(DESCRIPTION) ? rows[i, mappings[DESCRIPTION]] : String.Empty;
                dRows.Add(new DefinitionRow(rows[i, 0], type, description));
            }

            return dRows;
        }

        public DefinitionRow(string name, string type, string description)
        {
            this.Name = name;
            this.Type = type;
            this.Description = description;
        }

        /// <summary>
        /// The identifier of the current row.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Optional.
        /// </summary>
        public string Description { get; set; }


        private const string TYPE = "Type definition";
        private const string DESCRIPTION = "Description";
        private Mappings<string, int> mappings;
    }
}
