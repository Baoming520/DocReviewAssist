namespace XMLValidator.Models.Special.Extensions
{
    #region Namespaces
    using DocxParser.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using XMLValidator.Utils;
    #endregion

    public static class extComplexType
    {
        //public static TDComplexType TryToParse(this DocxTable table, Mappings<string, string> colMappings, string sectionPattern = null)
        //{
        //    var ret = new TDComplexType();
        //    if (table == null)
        //    {
        //        throw new ArgumentException("table");
        //    }
        //    if (!String.IsNullOrEmpty(sectionPattern))
        //    {
        //        if (!Regex.IsMatch(table.Section, sectionPattern))
        //        {
        //            ret.ErrorCode = 1;
        //            ret.Message = "The section number does not match the specified pattern.";
        //            return ret;
        //        }
        //    }

        //    var indexMappings = new Dictionary<string, int>();
        //    bool validate = true;
        //    foreach (var colMapping in colMappings)
        //    {
        //        int colIndex = colMappings.GetIndex(colMapping);
        //        if (colMapping.Item1 != table.Rows[0][colIndex])
        //        {
        //            validate = false;
        //            break;
        //        }

        //        indexMappings.Add(colMapping.Item2, colIndex);
        //    }

        //    if (!validate)
        //    {
        //        ret.ErrorCode = 1;
        //        ret.Message = "The table header does not match the specified one.";
        //    }

        //    for (var rIdx = 1; rIdx < table.Rows.Count; rIdx++)
        //    {
        //        if (indexMappings.Keys.Contains("PropertyName"))
        //        {
        //            var propName = table.Rows[rIdx][indexMappings["PropertyName"]];
        //            ret.PropertyNames.Add(propName.RemoveEndNoteTag());
        //        }
        //        if (indexMappings.Keys.Contains("PropertyType"))
        //        {
        //            var propType = table.Rows[rIdx][indexMappings["PropertyType"]];
        //            ret.PropertyTypes.Add(propType.RemoveEndNoteTag());
        //        }
        //        if (indexMappings.Keys.Contains("PropertyDescription"))
        //        {
        //            ret.PropertyDescription.Add(table.Rows[rIdx][indexMappings["PropertyDescription"]]);
        //        }
        //        if (indexMappings.Keys.Contains("DefaultValue"))
        //        {
        //            var defaultVal = table.Rows[rIdx][indexMappings["DefaultValue"]];
        //            ret.DefaultValues.Add(defaultVal.RemoveEndNoteTag());
        //        }
        //    }

        //    return ret;
        //}

        private static string RemoveEndNoteTag(this string target, string endNotePattern = "<[0-9]+>")
        {
            var matches = Regex.Matches(target, endNotePattern);
            if (matches != null && matches.Count > 0)
            {
                foreach (Match m in matches)
                {
                    target = target.Replace(m.Value, "");
                }
            }

            return target;
        }
    }
}
