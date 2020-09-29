namespace XMLValidator
{
    #region Namespaces.
    using DocxParser.Models.Code;
    using DocxParser.Models.Extensions;
    using DocxParser.Models.TechDoc;
    using DocxParser.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;
    using XMLValidator.Extensions;
    using XMLValidator.Models;
    using XMLValidator.Models.Special;
    using XMLValidator.Models.Special.Extensions;
    using XMLValidator.Models.Special.SSAST;
    using XMLValidator.Models.Validation;
    using XMLValidator.Models.XSD;
    using XMLValidator.Utils;
    #endregion

    public static class CoreAPIs
    {
        public static List<ValidationResult> CompareCodeWithTableDefsForDTSX2(this TechnicalDocument doc, DocxParser.Utils.XS.XSNamespace xsNS)
        {
            const string SHORT_NAME = "MS-DTSX2";
            var vResults = new List<ValidationResult>();
            if (doc.ShortName != SHORT_NAME)
            {
                var vRes = new ValidationResult();
                vRes.ErrorCode = 2;
                vRes.ErrorMessage = String.Format("The document short name is not {0}.", SHORT_NAME);
                vResults.Add(vRes);

                return vResults;
            }

            var cc = new XMLCodeCollector(doc.Codes, xsNS);
            cc.Group();

            var nsTables = doc.Tables.GetTables("2.2");
            var nsToc = doc.TableOfContents.GetToC("2.2");
            var xsRelationships = new XSRelationships(nsToc, nsTables[0]);
            XSNamespaceManager.Initialize(xsRelationships.Relationships);

            var xsdInfoList = new List<XSDInfo>();
            foreach (var code in cc.Codes)
            {
                if (cc.IsXSDCode(code.Code) == true)
                {
                    var vRes = new ValidationResult();
                    vRes.Section = code.Section;
                    var xsCode = new XSCode(cc.HandleXSDCodeSnippet(code.Code));
                    if (xsCode.ErrorCode == 0)
                    {
                        // point to 2.7.1
                        // point to 2.4
                        // point to 2.4.4.1.2.1
                        // point to 2.7.1.1.1.1.4.1.1
                        // point to 2.4.4.1.2.1.2.1.1
                        if (code.Section == "2.4.4.1.2.1.2.1.2")
                        {
                            //Console.WriteLine(code.Section);
                            var d_paras = doc.Paragraphs.GetParagraphs("2.7.1.1.1.1.4.1.1.1");
                            var d_scope = doc.Paragraphs.GetSectionScope("2.7.1.1.1.1.4.1.1.1");
                            var d_tabls = doc.Tables.GetTables("2.7.1.1.1.1.4.1.1.1", d_scope);

                            d_paras = doc.Paragraphs.GetParagraphs("2.4.4.1.2.1.2.1.2");
                            d_scope = doc.Paragraphs.GetSectionScope("2.4.4.1.2.1.2.1.2");
                            d_tabls = doc.Tables.GetTables("2.4.4.1.2.1.2.1.2", d_scope);

                            
                        }

                        if (code.Section == "2.7.1.1.1.1.4.1.1")
                        {
                            var sect = doc.Paragraphs.GetSectionNumberByOrder(code.OrderRange[0]);
                        }
                        //else
                        //{
                        //    continue;
                        //}

                        string actSection = code.Section.GetSectionLevel() < 9 ? 
                            code.Section : 
                            doc.Paragraphs.GetSectionNumberByOrder(code.OrderRange[0]);
                        Tuple<int, int> scope = null;
                        if (code.Section.GetSectionLevel() >= 9)
                        {
                            scope = doc.Paragraphs.GetSectionScope(actSection);
                        }

                        // Check the consistency between the code and its definition table
                        var tables = doc.Tables.GetTables(actSection, scope: scope);
                        if (tables == null || tables.Count == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("[{0}]: The defined table is not found in section {1}.", "Warning", actSection);
                            Console.ResetColor();
                        }

                        // The instances of the definition tables
                        var defTbls = new List<DefinitionTable>();
                        foreach (var tbl in tables)
                        {
                            defTbls.Add(new DefinitionTable(tbl.Content));
                        }

                        var xsdInfos = XSDInfo.Parse(xsCode);
                        xsdInfoList.AddRange(xsdInfos);
                        foreach (var xsdInfo in xsdInfos)
                        {
                            // Check all the element in complex type.
                            var elems = xsdInfo.GetElements();
                            if (elems != null)
                            {
                                foreach (var elem in elems)
                                {
                                    DefinitionTable targetTbl = DefinitionTable.GetDefinitionTable(defTbls, elem.LocalName);
                                    if (targetTbl == null)
                                    {
                                        break;
                                    }

                                    string msg;
                                    if (String.IsNullOrEmpty(elem.Name))
                                    {
                                        continue;
                                    }

                                    if (!targetTbl.Test(elem.Name, ((ElementInfo)elem).Type, out msg))
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("[Failure]: Section {0}", actSection);
                                        Console.WriteLine(msg);
                                        Console.ResetColor();
                                    }
                                }
                            }


                            var attrs = xsdInfo.GetAttributes();
                            if (attrs != null)
                            {
                                foreach (var attr in attrs)
                                {
                                    foreach (var defTbl in defTbls)
                                    {
                                        if (String.Equals(defTbl.Category, attr.LocalName, StringComparison.OrdinalIgnoreCase))
                                        {
                                        }
                                    }
                                }
                            }


                            var attrGroups = xsdInfo.GetAttributeGroups();
                            if (attrGroups != null)
                            {
                                foreach (var attrGroup in attrGroups)
                                {
                                    foreach (var defTbl in defTbls)
                                    {
                                        if (String.Equals(defTbl.Category, attrGroup.LocalName, StringComparison.OrdinalIgnoreCase))
                                        {
                                        }
                                    }
                                }
                            }
                        }

                        // Check the consistency between the code and the paragraphs in the current setion
                        var paragraphs = doc.Paragraphs.GetParagraphs(actSection).Where(p => p.OrderNum < code.OrderRange[0] || p.OrderNum > code.OrderRange[1]);
                        foreach (var paragraph in paragraphs)
                        {
                            var words = paragraph.GetWords().Where(w => !String.IsNullOrEmpty(w)).ToArray();
                        }

                    }
                    else
                    {
                    }
                }
                else
                {
                }
            }


            return vResults;
        }

        public static List<ValidationResult> CompareCodeWithTableDefsForSSAS(this TechnicalDocument doc, DocxParser.Utils.XS.XSNamespace xsNS)
        {
            const string SHORT_NAME = "MS-SSAS";
            var vResults = new List<ValidationResult>();
            if (doc.ShortName != SHORT_NAME)
            {
                var vRes = new ValidationResult();
                vRes.ErrorCode = 2;
                vRes.ErrorMessage = String.Format("The document short name is not {0}.", SHORT_NAME);
                vResults.Add(vRes);

                return vResults;
            }

            var cc = new XMLCodeCollector(doc.Codes, xsNS);
            cc.Group();

            var nsTables = doc.Tables.GetTables("2.2.1");
            var nsToc = doc.TableOfContents.GetToC("2.2.1");
            var xsRelationships = new XSRelationships(nsToc, nsTables[0]);
            XSNamespaceManager.Initialize(xsRelationships.Relationships);

            foreach (var code in cc.Codes)
            {
                if (cc.IsXSDCode(code.Code) == true)
                {
                    var vRes = new ValidationResult();
                    vRes.Section = code.Section;
                    var xsCode = new XSCode(cc.HandleXSDCodeSnippet(code.Code));
                    if (xsCode.ErrorCode == 0)
                    {
                        var cTypeFromCode = new TDComplexType(xsCode, code.Section);
                        var relatedTbls = doc.Tables.GetTables(cTypeFromCode.Section);
                        foreach (var tbl in relatedTbls)
                        {
                            //tbl.TryToParse
                            //var cTypeFromTabl = tbl.TryToParse(context.ColumnMappingsList[idx]);
                        }


                    }
                }
            }




            return vResults;
        }
        //public static List<ValidationResult> CompareCodeWithRestrictTableForSSAST(this TechnicalDocument doc, CodeTableContext context)
        //{
        //    const string SHORT_NAME = "MS-SSAS-T";
        //    var vResults = new List<ValidationResult>();
        //    if (doc.ShortName != SHORT_NAME)
        //    {
        //        var vRes = new ValidationResult();
        //        vRes.ErrorCode = 2;
        //        vRes.ErrorMessage = String.Format("The document short name is not {0}.", SHORT_NAME);
        //        vResults.Add(vRes);

        //        return vResults;
        //    }

        //    var cc = new CodeCollector(doc.Codes);
        //    cc.Group();

        //    // Get all namespaces from section 2.2.1 in the protocol MS-SSAS-T.
        //    var nsTables = doc.Tables.GetTables("2.2.1");
        //    var nsToc = doc.TableOfContents.GetToC("2.2.1");
        //    var xsRelationships = new XSRelationships(nsToc, nsTables[0]);
        //    XSNamespaceManager.Initialize(xsRelationships.Relationships);

        //    foreach (var code in cc.Codes)
        //    {
        //        var vRes = new ValidationResult();
        //        vRes.Section = code.Section;
        //        var xsCode = new XSCode(code.Code);
        //        if (xsCode.ErrorCode == 0)
        //        {
        //            var cTypeFromCode = new TDComplexType(xsCode, code.Section);
        //            var relatedTbls = doc.Tables.GetTables(cTypeFromCode.Section);
        //            foreach (var tbl in relatedTbls)
        //            {
        //                for (int idx = 0; idx < context.Count; idx++)
        //                {
        //                    if (Regex.IsMatch(tbl.Section, context[idx].SectionPattern))
        //                    {
        //                        var cTypeFromTabl = tbl.TryToParse(context.ColumnMappingsList[idx], context.SectionPatterns[idx]);
        //                        if (cTypeFromTabl.ErrorCode == 0)
        //                        {
        //                            if (cTypeFromCode.PropertyNames.Count != cTypeFromTabl.PropertyNames.Count)
        //                            {
        //                                vRes.ErrorCode = 2;
        //                                vRes.ErrorMessage = String.Format("The length of property name is {0} in the code, but {1} in the table.",
        //                                    cTypeFromCode.PropertyNames.Count, cTypeFromTabl.PropertyNames.Count);
        //                                break;
        //                            }

        //                            for (int j = 0; j < cTypeFromTabl.PropertyNames.Count; j++)
        //                            {
        //                                if (cTypeFromTabl.PropertyNames[j] != cTypeFromCode.PropertyNames[j])
        //                                {
        //                                    vRes.ErrorCode = 2;
        //                                    vRes.ErrorMessage = String.Format(
        //                                        "The property name '{0}' in the code is different with the property name '{1}' in restriction table.",
        //                                        cTypeFromCode.PropertyNames[j], cTypeFromTabl.PropertyNames[j]);
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            vRes.ErrorCode = cTypeFromTabl.ErrorCode;
        //                            vRes.ErrorMessage = cTypeFromTabl.Message;
        //                            vResults.Add(vRes);
        //                        }
        //                        break;
        //                    }
        //                }

        //                vResults.Add(vRes);
        //            }
        //        }
        //        else
        //        {
        //            vRes.ErrorCode = xsCode.ErrorCode;
        //            vRes.ErrorMessage = xsCode.Message;
        //            vResults.Add(vRes);
        //        }
        //    }

        //    return vResults;
        //}

        //public static List<ValidationResult> CompareCodeWithDefineTableForSSASTDiscover(this TechnicalDocument doc)
        //{
        //    const string SHORT_NAME = "MS-SSAS-T";
        //    var vResults = new List<ValidationResult>();
        //    if (doc.ShortName != SHORT_NAME)
        //    {
        //        var vRes = new ValidationResult();
        //        vRes.ErrorCode = 2;
        //        vRes.ErrorMessage = String.Format("The document short name is not {0}.", SHORT_NAME);
        //        vResults.Add(vRes);

        //        return vResults;
        //    }

        //    var cc = new CodeCollector(doc.Codes);
        //    cc.Group();

        //    // Get all namespaces from section 2.2.1 in the protocol MS-SSAS-T.
        //    var nsTables = doc.Tables.GetTables("2.2.1");
        //    var nsToc = doc.TableOfContents.GetToC("2.2.1");
        //    var xsRelationships = new XSRelationships(nsToc, nsTables[0]);
        //    XSNamespaceManager.Initialize(xsRelationships.Relationships);

        //    foreach (var code in cc.Codes)
        //    {
        //        // Process the code in Discover section.
        //        if (Regex.IsMatch(code.Section, "^3\\.1\\.5\\.1\\.1\\.[0-9]+\\.2\\.1$"))
        //        {
        //            var xsCode = new XSCode(code.Code);
        //            if (xsCode.ErrorCode != 0)
        //            {
        //                Console.WriteLine(code.Section);
        //                Console.WriteLine(xsCode.Message);
        //            }
        //            var complexTypeFromCode = new TDComplexType(xsCode, code.Section);
        //            if (complexTypeFromCode.ErrorCode != 0)
        //            {
        //                Console.WriteLine(complexTypeFromCode.Message);
        //                continue;
        //            }
        //            var grandParentSection = code.Section.GetParentSection().GetParentSection();
        //            var grandParent = doc.TableOfContents.GetToC(grandParentSection);
        //            if (grandParent == null)
        //            {
        //                continue;
        //            }

        //            var splits = grandParent.Content.Remove(0, "TMSCHEMA_".Length).Split('_');
        //            var targetSectionName = String.Empty;
        //            foreach (var s in splits)
        //            {
        //                targetSectionName += (s.First().ToString().ToUpper() + s.Substring(1).ToLower()).ToSingular();
        //            }

        //            targetSectionName += " Object";
        //            var targetSection = doc.TableOfContents.GetSectionNumberByName(targetSectionName);
        //            var targetDefTables = doc.Tables.GetTables(targetSection);
        //            if (targetDefTables == null || targetDefTables.Count == 0)
        //            {
        //                continue;
        //            }

        //            var colMappings = new Mappings<string, string>();
        //            colMappings.Add("Name", "PropertyName");
        //            colMappings.Add("Type", "PropertyType");
        //            colMappings.Add("Description", "PropertyDescription");
        //            TDComplexType complexTypeFromTable = null;
        //            foreach (var defTab in targetDefTables)
        //            {
        //                complexTypeFromTable = defTab.TryToParse(colMappings);
        //                if (complexTypeFromTable.ErrorCode == 0)
        //                {
        //                    break;
        //                }
        //            }

        //            if (complexTypeFromTable.ErrorCode != 0)
        //            {
        //                Console.WriteLine(complexTypeFromTable.Message);
        //                continue;
        //            }

        //            string msg = String.Empty;
        //            var seq = new List<int>();
        //            for (int i = 0; i < complexTypeFromCode.PropertyNames.Count; i++)
        //            {
        //                bool flag = false;
        //                for (int j = 0; j < complexTypeFromTable.PropertyNames.Count; j++)
        //                {
        //                    if (complexTypeFromCode.PropertyNames[i] == complexTypeFromTable.PropertyNames[j] &&
        //                        (complexTypeFromCode.PropertyTypes[i] == "xs:" + complexTypeFromTable.PropertyTypes[j] ||
        //                        complexTypeFromTable.PropertyTypes[j] == "enumeration"))
        //                    {
        //                        seq.Add(j);
        //                        flag = true;
        //                        break;
        //                    }
        //                }

        //                if (!flag)
        //                {
        //                    msg = String.Format("Code.Property \"{0}: {1}\" is not found in definition table", complexTypeFromCode.PropertyNames[i], complexTypeFromCode.PropertyTypes[i]);
        //                }
        //            }

        //            if (seq.Count != complexTypeFromCode.PropertyNames.Count)
        //            {
        //                Console.WriteLine(code.Section);
        //                Console.WriteLine("Miss some properties.");
        //                continue;
        //            }

        //            if (!VerifySequenceArray(seq))
        //            {
        //                Console.WriteLine(code.Section);
        //                Console.WriteLine("sequence error.");
        //                continue;
        //            }

        //        }
        //    }

        //    return vResults;
        //}

        //public static List<ValidationResult> CompareCodeWithDefineTableForSSASTExecute(this TechnicalDocument doc, string sectionPattern, string actionName)
        //{
        //    const string SHORT_NAME = "MS-SSAS-T";
        //    var vResults = new List<ValidationResult>();
        //    if (doc.ShortName != SHORT_NAME)
        //    {
        //        var vRes = new ValidationResult();
        //        vRes.ErrorCode = 2;
        //        vRes.ErrorMessage = String.Format("The document short name is not {0}.", SHORT_NAME);
        //        vResults.Add(vRes);

        //        return vResults;
        //    }

        //    var cc = new CodeCollector(doc.Codes);
        //    cc.Group();

        //    // Get all namespaces from section 2.2.1 in the protocol MS-SSAS-T.
        //    var nsTables = doc.Tables.GetTables("2.2.1");
        //    var nsToc = doc.TableOfContents.GetToC("2.2.1");
        //    var xsRelationships = new XSRelationships(nsToc, nsTables[0]);
        //    XSNamespaceManager.Initialize(xsRelationships.Relationships);

        //    foreach (var code in cc.Codes)
        //    {
        //        // Process the code in Execute section.
        //        if (Regex.IsMatch(code.Section, sectionPattern))
        //        {
        //            var xsCode = new XSCode(code.Code);
        //            if (xsCode.ErrorCode != 0)
        //            {
        //                Console.WriteLine(code.Section);
        //                Console.WriteLine(xsCode.Message);
        //            }
        //            var complexTypeFromCode = new TDComplexType(xsCode, code.Section);
        //            if (complexTypeFromCode.ErrorCode != 0)
        //            {
        //                Console.WriteLine(complexTypeFromCode.Message);
        //                continue;
        //            }

        //            var sectionName = doc.TableOfContents.GetSectionNameByNumber(code.Section);
        //            var targetSectionName = sectionName.Substring((actionName + " ").Length).ToSingular() + " Object";
        //            var targetSection = doc.TableOfContents.GetSectionNumberByName(targetSectionName);
        //            if (String.IsNullOrEmpty(targetSection))
        //            {
        //                continue;
        //            }
        //            var targetDefTables = doc.Tables.GetTables(targetSection);
        //            if (targetDefTables == null || targetDefTables.Count == 0)
        //            {
        //                continue;
        //            }

        //            var colMappings = new Mappings<string, string>();
        //            colMappings.Add("Name", "PropertyName");
        //            colMappings.Add("Type", "PropertyType");
        //            colMappings.Add("Description", "PropertyDescription");
        //            TDComplexType complexTypeFromTable = null;
        //            foreach (var defTab in targetDefTables)
        //            {
        //                complexTypeFromTable = defTab.TryToParse(colMappings);
        //                if (complexTypeFromTable.ErrorCode == 0)
        //                {
        //                    break;
        //                }
        //            }

        //            if (complexTypeFromTable.ErrorCode != 0)
        //            {
        //                Console.WriteLine(complexTypeFromTable.Message);
        //                continue;
        //            }

        //            string msg = String.Empty;
        //            var seq = new List<int>();
        //            int specCount = 0;
        //            for (int i = 0; i < complexTypeFromCode.PropertyNames.Count; i++)
        //            {
        //                if (complexTypeFromCode.PropertyNames[i].Contains("."))
        //                {
        //                    if (Regex.IsMatch(complexTypeFromCode.PropertyNames[i], "^[a-zA-Z0-9_]+(\\.[a-zA-Z0-9_]+)+$"))
        //                    {
        //                        int fIdx = complexTypeFromCode.PropertyNames[i].IndexOf('.');
        //                        var propName = complexTypeFromCode.PropertyNames[i].Substring(0, fIdx);
        //                        if (complexTypeFromTable.PropertyNames.Contains(propName) &&
        //                            complexTypeFromCode.PropertyTypes[i] == "xs:string")
        //                        {
        //                            msg += String.Format("[Special Property]: {0}\r\n", complexTypeFromCode.PropertyNames[i]);
        //                            specCount++;
        //                        }
        //                    }

        //                    continue;
        //                }
        //                bool flag = false;
        //                for (int j = 0; j < complexTypeFromTable.PropertyNames.Count; j++)
        //                {
        //                    if (complexTypeFromCode.PropertyNames[i] == complexTypeFromTable.PropertyNames[j] &&
        //                        (complexTypeFromCode.PropertyTypes[i] == "xs:" + complexTypeFromTable.PropertyTypes[j] ||
        //                        complexTypeFromTable.PropertyTypes[j] == "enumeration"))
        //                    {
        //                        seq.Add(j);
        //                        flag = true;
        //                        break;
        //                    }
        //                }

        //                if (!flag)
        //                {
        //                    msg += String.Format("Code.Property \"{0}: {1}\" is not found in definition table.\r\n", complexTypeFromCode.PropertyNames[i], complexTypeFromCode.PropertyTypes[i]);
        //                }
        //            }

        //            if (seq.Count != complexTypeFromCode.PropertyNames.Count - specCount)
        //            {
        //                Console.WriteLine(code.Section + " vs " + targetSection);
        //                Console.WriteLine("Miss some properties.");
        //                Console.WriteLine(msg);
        //                continue;
        //            }

        //            if (!VerifySequenceArray(seq))
        //            {
        //                Console.WriteLine(code.Section + " vs " + targetSection);
        //                Console.WriteLine("sequence error.");
        //                Console.WriteLine(msg);
        //                continue;
        //            }
        //        }
        //    }


        //    return vResults;
        //}

        private static bool VerifySequenceArray(IEnumerable<int> sequence)
        {
            if (sequence == null || sequence.Count() <= 1)
            {
                return true;
            }

            bool ret = true;
            var seqArr = sequence.ToArray();
            for (int i = 1; i < seqArr.Length - 1; i++)
            {
                if (!(seqArr[i - 1] < seqArr[i] && seqArr[i] < seqArr[i + 1]))
                {
                    ret = false;
                    break;
                }
            }

            return ret;
        }
    }
}
