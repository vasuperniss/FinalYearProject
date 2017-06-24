using DigitalIsraelFund_System.Models;
using System.Collections.Generic;

namespace DigitalIsraelFund_System.DataBase.Managers
{
    public class MadaanMomhimManager
    {
        private static MadaanMomhimManager instance = new MadaanMomhimManager();

        public static MadaanMomhimManager Manager { get { return instance; } }

        private MadaanMomhimManager() { }

        public void AddOrUpdate(List<Dictionary<string, string>> table, Settings sett)
        {
            Dictionary<string, string> conv = sett.CreateConversionTableMadaanMomhim(table[0].Keys);
            foreach (Dictionary<string, string> line in table)
            {
                var values = new Dictionary<string, string>();
                foreach (string key in line.Keys)
                {
                    if (conv.ContainsKey(key))
                        values[conv[key]] = line[key];
                }
                if (values.ContainsKey("file_number"))
                {
                    MySqlCommands.Delete("madaan_testers", "file_number='" + values["file_number"] + "'");
                    MySqlCommands.Insert("madaan_testers", values);
                }
            }
        }

        public List<Dictionary<string, string>> GetAllWhere(string where, string orderBy, int page, int resultsPerPage)
        {
            List<string> fields = new List<string>();
            fields.Add("file_number");
            fields.Add("comp_number");
            fields.Add("comp_name");
            fields.Add("status");
            fields.Add("status_date");
            fields.Add("request_date");
            fields.Add("name");
            fields.Add("head_field");
            fields.Add("theme");
            fields.Add("tester_phone");
            fields.Add("tester_email");
            fields.Add("phone");
            fields.Add("cellphone");
            string limit = ((page - 1) * resultsPerPage) + "," + resultsPerPage;
            List<Dictionary<string, string>> requestsResult = MySqlCommands.Select("madaan_testers",
                fields, null, where, orderBy, limit);
            return requestsResult;
        }

        public List<Dictionary<string, string>> GetFieldWhere(string field, string where, string orderBy, int page, int resultsPerPage)
        {
            List<string> fields = new List<string>();
            fields.Add("DISTINCT " + field);
            string limit = ((page - 1) * resultsPerPage) + "," + resultsPerPage;
            List<Dictionary<string, string>> requestsResult = MySqlCommands.Select("madaan_testers",
                fields, null, where, orderBy, limit);
            return requestsResult;
        }
    }
}