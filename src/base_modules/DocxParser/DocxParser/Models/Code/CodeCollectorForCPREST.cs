namespace DocxParser.Models.Code
{
    #region Namespaces.
    using DocxParser.Utils;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    #endregion

    public class CodeCollectorForCPREST : CodeCollector
    {
        /// <summary>
        /// Matches the following regular expression
        /// 1, Matches a specified property element () which contains a "comment" element;
        /// 2, Matches the format in JSON code. Such as: some whitespaces.
        /// </summary>
        public const string CPREST_COMMENT_PATTERN = "\"([A-Za-z_.-])+\": \\{\\r\\n\\s*\"comment\": \"section ([0-9]+\\.?)+\"\\r\\n\\s*\\}";

        /// <summary>
        /// Uses the following pattern to extract the value of the comment.
        /// </summary>
        public const string CPREST_COMMENT_VALUE_PATTERN = "section ([0-9]+\\.?)+";

        public static bool IsJSONCode(string jCode)
        {
            try
            {
                var jSerializer = new JsonSerializer();
                var sr = new StringReader(jCode.Trim());
                JsonTextReader jTextReader = new JsonTextReader(sr);
                var obj = jSerializer.Deserialize(jTextReader);

                var objType = obj.GetType().ToString();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(objType);
                Console.ResetColor();

                return obj.GetType() == typeof(JObject) || obj.GetType() == typeof(JArray);
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return false;
            }
        }

        public CodeCollectorForCPREST(IEnumerable<DocxCode> codes)
            : base(codes)
        {
            this.CodeTypes = new List<string>();
            this.MergedCodes = new List<DocxCode>();
            this.NotCodes = new List<DocxCode>();
            this.citedSections = new HashSet<string>();
        }

        [JsonProperty("code_types")]
        public List<string> CodeTypes { get; private set; }

        [JsonProperty("merged_codes")]
        public List<DocxCode> MergedCodes { get; private set; }

        [JsonIgnore]
        public List<DocxCode> NotCodes { get; private set; }

        public new void Group()
        {
            var codeCache = string.Empty;
            var sectionCache = string.Empty;
            int pageNumCache = -1;
            foreach (var code in this.RawCodes)
            {
                if (code.IsBoundary)
                {
                    this.StoreCodeSnippet(codeCache, sectionCache, pageNumCache);
                    sectionCache = code.Section;
                    pageNumCache = code.PageNum;
                    codeCache = String.Empty;
                }

                codeCache += code.Code + Constants.CRLF;
            }

            this.StoreCodeSnippet(codeCache, sectionCache, pageNumCache);
        }

        public DocxCode GetMergedCodeBySection(string section)
        {
            var ret = new DocxCode(string.Empty, section);
            foreach (var code in this.MergedCodes)
            {
                if (code.Section == section)
                {
                    ret.Code = code.Code;
                    break;
                }
            }

            return ret;
        }

        public void Integrate()
        {
            foreach (var code in this.Codes)
            {
                var mergedCode = code.Code;
                this.Integrate(ref mergedCode);
                if (!this.citedSections.Contains(code.Section))
                {
                    this.MergedCodes.Add(new DocxCode(mergedCode, code.Section, code.PageNum));
                }
            }
        }

        public void Integrate(ref string code)
        {
            MatchCollection matches = Regex.Matches(code, CPREST_COMMENT_PATTERN);
            if (matches.Count == 0)
            {
                return;
            }

            foreach (var m in matches)
            {
                if (m.GetType() == typeof(Match))
                {
                    var match = (Match)m;

                    // It is impossible that the code block is a JArray type.
                    var mCodeSnippet = (JObject)this.ToJInstance(this.TryToFixJSONString(match.Value));

                    // The jProps should only has one element in it.
                    // So get the property name using the following method directly.
                    var jProps = mCodeSnippet.Properties();
                    var propName = jProps.ToList()[0].Name;

                    var section = this.GetSectionFromComment(match.Value);
                    this.citedSections.Add(section);
                    var refCodes = GetCodesBySection(section);
                    foreach (var rCode in refCodes)
                    {
                        var jInstance = this.ToJInstance(rCode.Code);
                        if (jInstance.GetType() == typeof(JObject))
                        {
                            var jObj = (JObject)jInstance;
                            if (jObj.ContainsKey(propName))
                            {
                                code = code.Replace(match.Value, rCode.Code.TrimStart('{').TrimEnd('}'));
                                code = this.ToFormattedJSON(code);
                                this.Integrate(ref code);
                            }
                        }
                    }
                }
            }
        }

        #region Private methods
        private HashSet<string> citedSections;

        private void StoreCodeSnippet(string code, string section, int pageNum)
        {
            if (!String.IsNullOrEmpty(code))
            {
                var triedTypes = new HashSet<string>();
                object jInstance;
                string errMsg;
                var jsonType = this.DetectJSONType(code, ref triedTypes, out jInstance, out errMsg);
                if (jsonType == typeof(JObject) || jsonType == typeof(JArray))
                {
                    var typeName = jsonType.ToString();
                    this.CodeTypes.Add(typeName.Substring(typeName.LastIndexOf('.') + 1));
                    string formattedJson = this.ToFormattedJSON((JToken)jInstance);
                    this.Codes.Add(new DocxCode(formattedJson, section, pageNum));
                    this.Sections.Add(section);
                }
                else
                {
                    this.NotCodes.Add(new DocxCode(code.Trim(), section, pageNum));
                }
            }
            else
            {
                // Process and store the content with code style, but it is not a code.
                // Such as the code is an empty string.
                this.NotCodes.Add(new DocxCode(code.Trim(), section, pageNum));
            }
        }

        
        /// <summary>
        /// Detect the appropriate JSON type of the specified string.
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <param name="triedTypes">The types have been tried.</param>
        /// <param name="jInstance">Output parameter: A CSharp JSON instance.</param>
        /// <param name="errorMessage">Output parameter: An error message.</param>
        /// <returns></returns>
        private Type DetectJSONType(string json, ref HashSet<string> triedTypes, out object jInstance, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                var jSerializer = new JsonSerializer();
                var sr = new StringReader(json);
                JsonTextReader jTextReader = new JsonTextReader(sr);
                jInstance = jSerializer.Deserialize(jTextReader);
                if (jInstance.GetType() == typeof(JObject) ||
                    jInstance.GetType() == typeof(JArray))
                {
                    return jInstance.GetType();
                }
                else
                {
                    if (triedTypes.Count == 0)
                    {
                        triedTypes.Add(typeof(JObject).ToString());
                        return this.DetectJSONType(TryToFixJSONString(json, fixedWithJObject: true), ref triedTypes, out jInstance, out errorMessage);
                    }
                    else if (triedTypes.Count == 1 && triedTypes.Contains(typeof(JObject).ToString()))
                    {
                        triedTypes.Add(typeof(JArray).ToString());
                        return this.DetectJSONType(TryToFixJSONString(json, fixedWithJObject: false), ref triedTypes, out jInstance, out errorMessage);
                    }
                    else
                    {
                        errorMessage = "fatal error";
                        return jInstance.GetType();
                    }
                }
            }
            catch (Exception ex)
            {
                jInstance = null;
                errorMessage = ex.Message;

                return null;
            }
        }

        private string TryToFixJSONString(string json, bool fixedWithJObject = true)
        {
            string fixedJson = json;
            if (!json.TrimStart().StartsWith("{") &&
                !json.TrimStart().StartsWith("["))
            {
                if (fixedWithJObject)
                {
                    fixedJson = "{" + json + "}";
                }
                else
                {
                    fixedJson = "[" + json + "]";
                }

            }

            return fixedJson;
        }

        private string ToFormattedJSON(string json)
        {
            var jInstance = this.ToJInstance(json);
            if (jInstance.GetType() == typeof(JObject) ||
                jInstance.GetType() == typeof(JArray))
            {
                return this.ToFormattedJSON((JToken)jInstance);
            }
            else
            {
                throw new ArgumentException("json");
            }
        }

        private string ToFormattedJSON(JToken jToken)
        {
            var jSerializer = new JsonSerializer();
            var strWriter = new StringWriter();
            JsonTextWriter jTextWriter = new JsonTextWriter(strWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 2,
                IndentChar = ' '
            };

            jSerializer.Serialize(jTextWriter, jToken);

            return strWriter.ToString();
        }

        private object ToJInstance(string json)
        {
            var jSerializer = new JsonSerializer();
            var sr = new StringReader(json);
            JsonTextReader jTextReader = new JsonTextReader(sr);
            var obj = jSerializer.Deserialize(jTextReader);

            return obj;
        }

        private string GetSectionFromComment(string comment)
        {
            Match match = Regex.Match(comment, CPREST_COMMENT_VALUE_PATTERN);

            return match.Value.Substring("section ".Length);
        }
        #endregion
    }
}
