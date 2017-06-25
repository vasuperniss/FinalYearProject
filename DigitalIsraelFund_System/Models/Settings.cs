using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web.Hosting;

namespace DigitalIsraelFund_System.Models
{
    public class Settings
    {
        public int MashovVersion { get; set; }
        public List<int> PossibleMashovVersions { get; set; }
        public string LastUIDSeen { get; set; }
        public Dictionary<string, List<string>> MadaanMomhimExcelFieldsMatching { get; set; }
        public Dictionary<string, List<string>> RequestsExcelFieldsMatching { get; set; }

        public static Settings GetSettings()
        {
            var dataFile = HostingEnvironment.MapPath("~/App_Data/Settings.json");
            string json = System.IO.File.ReadAllText(@dataFile);
            return new JavaScriptSerializer().Deserialize<Settings>(json);
        }

        public void Save()
        {
            var json = new JavaScriptSerializer().Serialize(this);
            System.IO.File.WriteAllText(HostingEnvironment.MapPath("~/App_Data/Settings.json"), json);
        }

        public Dictionary<string, string> CreateConversionTableMadaanMomhim(ICollection<string> colNames)
        {
            return this.CreateConversionTable(colNames, this.MadaanMomhimExcelFieldsMatching);
        }

        public Dictionary<string, string> CreateConversionTableRequests(ICollection<string> colNames)
        {
            return this.CreateConversionTable(colNames, this.RequestsExcelFieldsMatching);
        }

        private Dictionary<string, string> CreateConversionTable(ICollection<string> colNames, Dictionary<string, List<string>> FieldsMatching)
        {
            Dictionary<string, string> conv = new Dictionary<string, string>();
            foreach (string name in colNames)
            {
                foreach (string key in FieldsMatching.Keys)
                {
                    bool found = false;
                    foreach (string posibleMatch in FieldsMatching[key])
                    {
                        if (name == posibleMatch)
                        {
                            conv[name] = key;
                            found = true;
                            break;
                        }
                    }
                    if (found)
                        break;
                }
            }
            return conv;
        }
    }
}