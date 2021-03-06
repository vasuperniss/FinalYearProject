﻿using DigitalIsraelFund_System.Models;
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
                    // convert to sql column names
                    if (conv.ContainsKey(key))
                        values[conv[key]] = line[key];
                }
                if (values.ContainsKey("file_number") && values["file_number"] != "" && values["file_number"] != "0")
                {
                    // remove if row exists
                    DBManager.Manager.Cmds.Delete("madaan_testers", "file_number='" + values["file_number"] + "'");
                    // insert a new row
                    DBManager.Manager.Cmds.Insert("madaan_testers", values);
                }
            }
        }

        public List<Dictionary<string, string>> GetAllWhere(string where, string orderBy, int page, int resultsPerPage)
        {
            List<string> fields = new List<string>();
            // get these fields below where the <where> statemant is true
            fields.Add("file_number");
            fields.Add("comp_number");
            fields.Add("comp_name");
            fields.Add("status");
            fields.Add("status_date");
            fields.Add("name");
            fields.Add("head_field");
            fields.Add("tester_phone");
            fields.Add("tester_email");
            fields.Add("phone");
            fields.Add("cellphone");
            // add order and limit
            if (orderBy == null || orderBy == "") orderBy = "file_number";
            string limit = ((page - 1) * resultsPerPage) + "," + resultsPerPage;
            List<Dictionary<string, string>> requestsResult = DBManager.Manager.Cmds.Select("madaan_testers",
                fields, null, where, orderBy, limit);
            return requestsResult;
        }

        public List<string> GetFieldWhere(string field, string where, string orderBy, int page, int resultsPerPage)
        {
            List<string> fields = new List<string>();
            fields.Add("DISTINCT " + field);
            string limit = ((page - 1) * resultsPerPage) + "," + resultsPerPage;
            if (orderBy == null || orderBy == "") orderBy = "file_number";
            List<Dictionary<string, string>> requestsResult = DBManager.Manager.Cmds.Select("madaan_testers",
                fields, null, where, orderBy, limit);
            // turn dictionary into list (can do, cause only 1 field)
            List<string> results = new List<string>();
            if (requestsResult != null)
                foreach (Dictionary<string, string> row in requestsResult)
                    results.Add(row[field]);
            return results;
        }

        public int Count(string where)
        {
            return DBManager.Manager.Cmds.Count("madaan_testers", where, null);
        }
    }
}