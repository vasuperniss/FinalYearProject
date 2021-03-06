﻿using DigitalIsraelFund_System.Models;
using System.Collections.Generic;
using System;

namespace DigitalIsraelFund_System.DataBase.Managers
{
    public class RequestManager
    {
        private static RequestManager instance = new RequestManager();

        public static RequestManager Manager { get { return instance; } }

        private RequestManager() { }

        private readonly object syncLock = new object();

        public void AddOrUpdate(List<Dictionary<string, string>> table, Settings sett)
        {
            lock (syncLock)
            {
                Dictionary<string, string> conv = sett.CreateConversionTableRequests(table[0].Keys);
                foreach (Dictionary<string, string> line in table)
                {
                    var values = new Dictionary<string, string>();
                    foreach (string key in line.Keys)
                    {
                        if (conv.ContainsKey(key))
                            values[conv[key]] = line[key];
                    }
                    ICollection<string> toUpdate = new List<string>(values.Keys);
                    toUpdate.Remove("file_number");
                    values["mashov"] = "אין";
                    if (values.ContainsKey("file_number") && values["file_number"] != "" && values["file_number"] != "0")
                    {
                        var where = "file_number='" + values["file_number"] + "'"; 
                        DBManager.Manager.Cmds.InsertOrUpdate("requests", values, toUpdate, where);
                    }
                }
            }
        }

        internal bool IsRequestAllowedForMomhee(string momhee_id, string file_number)
        {
            List<string> fields = new List<string>();
            fields.Add("file_number");
            var table = DBManager.Manager.Cmds.Select("requests", fields, null,
                "file_number='" + file_number + "' and momhee_id='" + momhee_id + "'",
                null, null);
            return table != null && table.Count != 0;
        }

        public bool UpdateMashov(string file_number, string json_path, string version)
        {
            string date = DateTime.Now.Date.Year + "/"
                + DateTime.Now.Date.Month + "/"
                + DateTime.Now.Date.Day;
            var newValues = new Dictionary<string, string>();
            newValues["mashov"] = json_path;
            newValues["mashov_date"] = date;
            newValues["mashov_ver"] = version;
            return DBManager.Manager.Cmds.Update("requests", newValues, "file_number='" + file_number + "'");
        }

        public List<Dictionary<string, string>> GetAllWhere(string where, string orderBy, int page, int resultsPerPage)
        {
            List<string> fields = new List<string>();
            // get these fields
            fields.Add("file_number");
            fields.Add("comp_name");
            fields.Add("status");
            fields.Add("submiter_name");
            // momhee_name = fname + ' ' + lname
            fields.Add("CONCAT_WS(' ', fname, lname) AS momhee_name");
            fields.Add("mashov");
            fields.Add("mashov_date");
            fields.Add("mashov_ver");
            fields.Add("fund_request");
            fields.Add("madaan_momhee");
            fields.Add("theme");
            fields.Add("madaan_mashov_date");
            fields.Add("is_accepted");
            fields.Add("given_fund");
            fields.Add("given_percentage");
            // number of files connected to this request
            fields.Add("(SELECT COUNT(*) FROM files WHERE requests.file_number=files.file_number) as num_files");
            string limit = ((page - 1) * resultsPerPage) + "," + resultsPerPage;
            string on = "users.id=requests.momhee_id";
            if (orderBy == null || orderBy == "") orderBy = "file_number";
            List<Dictionary<string, string>> requestsResult = DBManager.Manager.Cmds.Select("requests LEFT JOIN users",
                fields, on, where, orderBy, limit);
            return requestsResult;
        }

        public List<string> GetFieldWhere(string field, string where, string orderBy, int page, int resultsPerPage)
        {
            List<string> fields = new List<string>();
            if (field.Contains("CONCAT_WS"))
                field += " as f";
            fields.Add("DISTINCT " + field);
            if (field.Contains("CONCAT_WS"))
                field = "f";
            string limit = ((page - 1) * resultsPerPage) + "," + resultsPerPage;
            string on = "users.id=requests.momhee_id";
            if (orderBy == null || orderBy == "") orderBy = "file_number";
            List<Dictionary<string, string>> requestsResult = DBManager.Manager.Cmds.Select("requests LEFT JOIN users",
                fields, on, where, orderBy, limit);
            // turn the dictionary result into a list (can do cause only 1 field)
            List<string> results = new List<string>();
            if (requestsResult != null)
                foreach (Dictionary<string, string> row in requestsResult)
                    results.Add(row[field]);
            return results;
        }

        public int Count(string where)
        {
            return DBManager.Manager.Cmds.Count("requests LEFT JOIN users", where, "users.id=requests.momhee_id");
        }

        public bool Change(string file_number, Dictionary<string, string> newValues)
        {
            return DBManager.Manager.Cmds.Update("requests", newValues, "file_number='" + file_number + "'");
        }

        public Dictionary<string, string> GetAllColNames()
        {
            var names = new Dictionary<string, string>();
            // return column names for column titles
            names.Add("file_number", "מספר תיק");
            names.Add("comp_name", "שם  חברה או היזם");
            names.Add("status", "סטטוס התיק");
            names.Add("submiter_name", "שם  המגיש");
            names.Add("momhee_name", "בודק של הרשות לחדשנות");
            names.Add("fund_request", "גובה במענק המבוקש");
            names.Add("madaan_momhee", "בודק משרדי");
            return names;
        }

        internal List<string> GetFilesForRequest(string file_number)
        {
            List<string> fields = new List<string>();
            fields.Add("path");
            // get all files asosiated with the request
            string where = "file_number='" + file_number + "'";
            List<Dictionary<string, string>> requestsResult = DBManager.Manager.Cmds.Select("files",
                fields, null, where, null, null);
            // turn the result dictionary into a list (can do cause only 1 field)
            List<string> results = new List<string>();
            if (requestsResult != null)
                foreach (Dictionary<string, string> row in requestsResult)
                    results.Add(row["path"]);
            return results;
        }
    }
}