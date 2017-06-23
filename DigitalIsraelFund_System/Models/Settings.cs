using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace DigitalIsraelFund_System.Models
{
    public class Settings
    {
        public int MashovVersion { get; set; }
        public List<int> PossibleMashovVersions { get; set; }

        public static Settings LoadJson(string json)
        {
            return new JavaScriptSerializer().Deserialize<Settings>(json);
        }

        public string GetJson()
        {
            return new JavaScriptSerializer().Serialize(this);
        }
    }
}