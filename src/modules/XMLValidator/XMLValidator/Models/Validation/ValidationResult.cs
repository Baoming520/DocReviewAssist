namespace XMLValidator.Models.Validation
{
    #region Namespaces
    using System;
    #endregion

    public class ValidationResult
    {
        public static string GenHTML(string heading, string rowDataHtml)
        {
            const string html =
                "<html>" +
                  "<head>" +
                    "<meta http-equiv=\"Content-Type\" content=\"text/html;charset=utf-8\" />" +
                  "</head>" +
                  "<body>" +
                    "<h1>{0}</h1>" +
                    "<table border=\"1\" cellspacing=\"0\" cellpadding=\"5\">" +
                      "<thead>" +
                        "<tr>" +
                          "<th>ErrorCode</th>" +
                          "<th>ErrorMessage</th>" +
                          "<th>Section</th>" +
                          "<th>Code</th>" +
                        "</tr>" +
                      "</thead>" +
                      "<tbody>{1}</tbody>" +
                    "</table>" +
                  "</body>" +
                "</html>";

            return String.Format(html, heading, rowDataHtml);
        }

        public static string GenRowDataHtml(ValidationResult vResult)
        {
            const string colPattern = "<td valign=\"top\">{0}</td>";
            var rowData = "<tr>";
            rowData += String.Format(colPattern, vResult.ErrorCode);
            rowData += String.Format(colPattern, vResult.ErrorMessage);
            rowData += String.Format(colPattern, vResult.Section);
            rowData += String.Format(colPattern, vResult.Code);
            rowData += "</tr>";

            return rowData;
        }

        public ValidationResult()
        {
            this.ErrorCode = 0;
            this.ErrorMessage = "Passed";
        }

        public string Section { get; set; }
        public string Code { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
