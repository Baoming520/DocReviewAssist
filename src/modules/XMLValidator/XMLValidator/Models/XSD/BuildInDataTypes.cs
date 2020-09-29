namespace XMLValidator.Models.XSD
{
    // Namespaces
    #region Namespaces
    using System.Collections.Generic;
    #endregion
    //
    // All the primitive types are as follows:
    //
    //
    //                                    anyType
    //                                       |
    //                                       |
    //                                 anySimpleType
    //                                       |
    //     __________________________________|______________________________________
    //     |    |         |        |     |       |         |        |       |      |
    //     | duration  dateTime  time  date  gYearMonth  gYear  gMonthDay  gDay  gMonth
    //   __|______________________________________________________________________
    //   |    |          |           |         |    |    |       |       |       |
    //   | boolean  base64Binary  hexBinary  float  |  double  anyURI  QName  NOTATION
    //   |                                          |
    // string                                     decimal
    //   |                                          |
    // normalizedString                           integer
    //   |                             _____________|_____________
    //   |                             |            |            |
    // token                 nonPositiveInteger    long    nonNegativeInteger
    //   |___________________          |            |         ___|__________________
    //   |         |        |          |            |         |                    |
    // language  Name    NMTOKEN  negativeInteger  int    unsignedLong       positiveInteger
    //             |        !                       |         |
    //           NCName  NMTOKENS                 short   unsignedInt
    //   __________|___________                     |         |
    //   |         |          |                     |         |
    //   ID      IDREF      ENTITY                 byte   unsignedShort
    //             !          !                               |
    //           IDREFS     ENTITIES                      unsignedByte
    // 

    public static class BuildInDataTypes
    {
        /// <summary>
        /// All primitive types.
        /// </summary>
        public static string[] AllTypes
        {
            get
            {
                var types = new List<string>();
                types.AddRange(new URTypes().Types);
                types.AddRange(new BuildInPrimitiveTypes().Types);
                types.AddRange(new BuildInDerivedTypes().Types);

                return types.ToArray();
            }
        }
    }

    public class URTypes
    {
        public URTypes()
        {
            this.Types = new string[]
            {
                "anyType", "anySimpleType"
            };
        }

        public string[] Types { get; set; }
    }

    public class BuildInPrimitiveTypes
    {
        public BuildInPrimitiveTypes()
        {
            this.Types = new string[] 
            {
                "duration", "dateTime", "time", "date", "gYearMonth", "gYear", "gMonthDay", "gDay", "gMonth",
                "boolean", "base64Binary", "hexBinary", "float", "double", "anyURI", "QName", "NOTATION",
                "string", "decimal"
            };
        }

        public string[] Types { get; set; }
    }

    public class BuildInDerivedTypes
    {
        public BuildInDerivedTypes()
        {
            this.Types = new string[] 
            {
                "normalizedString", "integer",
                "token", "nonPositiveInteger", "long", "nonNegativeInteger",
                "language", "Name", "NMTOKEN", "negativeInteger", "int", "unsignedLong", "positiveInteger",
                "NCName", "NMTOKEN", "short", "unsignedInt",
                "ID", "IDREF", "ENTITY", "byte", "unsignedShort",
                "IDREFS", "ENTITIES", "unsignedByte"
            };
        }

        public string[] Types { get; set; }
    }
}
