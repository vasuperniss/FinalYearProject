using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;

namespace DigitalIsraelFund_System.Models
{
    public class FormValues
    {
        public Dictionary<string, string> Values { get; set; }
        public Dictionary<string, HttpPostedFileBase> Files { get; set; }

        public static FormValues LoadJson(string json)
        {
            return new FormValues() { Values = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(json) };
        }

        public string GetJson()
        {
            return new JavaScriptSerializer().Serialize(this.Values);
        }
    }
}