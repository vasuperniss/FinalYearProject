﻿using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace DigitalIsraelFund_System.Models
{
    public class Settings
    {
        public int MashovVersion { get; set; }
        public List<int> PossibleMashovVersions { get; set; }
        public Dictionary<string, List<string>> MadaanMomhimExcelFieldsMatching { get; set; }
        public Dictionary<string, List<string>> RequestsExcelFieldsMatching { get; set; }

        public static Settings LoadJson(string json)
        {
            return new JavaScriptSerializer().Deserialize<Settings>(json);
        }

        public string GetJson()
        {
            return new JavaScriptSerializer().Serialize(this);
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