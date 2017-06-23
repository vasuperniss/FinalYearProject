using System.Collections.Generic;

namespace DigitalIsraelFund_System.DataBase.Managers
{
    public class RequestManager
    {
        private static RequestManager instance = new RequestManager();

        public static RequestManager Manager { get { return instance; } }

        private RequestManager() { }

        public bool UpdateMashov(string file_number, string json_path)
        {
            string date = System.DateTime.Now.Date.Year + "/"
                + System.DateTime.Now.Date.Month + "/"
                + System.DateTime.Now.Date.Day;
            var newValues = new Dictionary<string, string>();
            newValues["mashov"] = json_path;
            newValues["mashov_date"] = date;
            return MySqlCommands.Update("requests", newValues, "file_number='" + file_number + "'");
        }

        public List<Dictionary<string, string>> GetAllWhere(string where, string orderBy, int page, int resultsPerPage)
        {
            List<string> fields = new List<string>();
            fields.Add("file_number");
            fields.Add("comp_name");
            fields.Add("status");
            fields.Add("submiter_name");
            fields.Add("CONCAT_WS(' ', fname, lname) AS momhee_name");
            fields.Add("mashov");
            fields.Add("fund_request");
            fields.Add("madaan_momhee");
            string limit = ((page - 1) * resultsPerPage) + "," + resultsPerPage;
            string on = "users.id=requests.momhee_id";
            List<Dictionary<string, string>> requestsResult = MySqlCommands.Select("requests LEFT JOIN users",
                fields, on, where, orderBy, limit);
            return requestsResult;
        }

        public List<Dictionary<string, string>> GetFieldWhere(string field, string where, string orderBy, int page, int resultsPerPage)
        {
            List<string> fields = new List<string>();
            fields.Add("DISTINCT " + field);
            string limit = ((page - 1) * resultsPerPage) + "," + resultsPerPage;
            string on = "users.id=requests.momhee_id";
            List<Dictionary<string, string>> requestsResult = MySqlCommands.Select("requests LEFT JOIN users",
                fields, on, where, orderBy, limit);
            return requestsResult;
        }

        public int Count(string where)
        {
            return MySqlCommands.Count("requests", where);
        }

        public bool Change(string file_number, Dictionary<string, string> newValues)
        {
            return MySqlCommands.Update("requests", newValues, "file_number='" + file_number + "'");
        }

        public Dictionary<string, string> GetAllColNames()
        {
            var names = new Dictionary<string, string>();
            names.Add("file_number", "מספר תיק");
            names.Add("comp_name", "שם  חברה או היזם");
            names.Add("status", "סטטוס התיק");
            names.Add("submiter_name", "שם  המגיש");
            names.Add("momhee_name", "בודק של הרשות לחדשנות");
            names.Add("fund_request", "גובה במענק המבוקש");
            names.Add("madaan_momhee", "בודק משרדי");
            return names;
        }
    }
}