using System.Collections.Generic;

namespace DigitalIsraelFund_System.DataBase.Managers
{
    public class LogManager
    {
        private static LogManager instance = new LogManager();

        public static LogManager Manager { get { return instance; } }

        private LogManager() { }

        public List<Dictionary<string, string>> GetAllWhere(string where, string orderBy, int page, int resultsPerPage)
        {
            List<string> fields = new List<string>();
            fields.Add("user_id");
            fields.Add("time_stamp");
            fields.Add("action");
            fields.Add("email");
            fields.Add("CONCAT_WS(' ', fname, lname) AS user_name");
            string limit = ((page - 1) * resultsPerPage) + "," + resultsPerPage;
            string on = "users.id=logs.user_id";
            List<Dictionary<string, string>> requestsResult = MySqlCommands.Select("logs LEFT JOIN users",
                fields, on, where, orderBy, limit);
            return requestsResult;
        }

        public void Add(string user_id, string action)
        {
            var values = new Dictionary<string, string>();
            values["user_id"] = user_id;
            values["action"] = action;
            values["time_stamp"] = System.DateTime.Now.ToLongDateString();
            MySqlCommands.Insert("logs", values);
        }
    }
}