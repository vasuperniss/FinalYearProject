using System.Collections.Generic;

namespace DigitalIsraelFund_System.DataBase.Managers
{
    public class RequestManager
    {
        private static RequestManager instance = new RequestManager();

        public static RequestManager Manager { get { return instance; } }

        public List<Dictionary<string, string>> GetAllWhere(string where, string orderBy, int page, int resultsPerPage)
        {
            List<string> fields = new List<string>();
            fields.Add("file_number");
            fields.Add("comp_name");
            fields.Add("status");
            fields.Add("submiter_name");
            fields.Add("CONCAT_WS(' ', fname, lname) AS momhee_name");
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
    }
}