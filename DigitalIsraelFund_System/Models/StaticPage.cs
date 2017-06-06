using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DigitalIsraelFund_System.Models
{
    public class StaticPage
    {
        public string Title { get; set; }

        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string HtmlContent { get; set; }

        public static StaticPage LoadJson(string json)
        {
            return new JavaScriptSerializer().Deserialize<StaticPage>(json);
        }

        public string GetJson()
        {
            return new JavaScriptSerializer().Serialize(this);
        }
    }
}