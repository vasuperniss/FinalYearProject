using DigitalIsraelFund_System.Models;
using System.Collections.Generic;

namespace DigitalIsraelFund_System.DataBase.Managers
{
    public class UserManager
    {
        public static void Add(string email, string password, string fname, string lname, string type)
        {
            var values = new Dictionary<string, string>();
            values["email"] = email;
            values["fname"] = fname;
            values["lname"] = lname;
            values["password"] = password;
            values["type"] = type;
            MySqlCommands.Insert("users", values);
        }

        public static UserData GetIfCorrect(string email, string password)
        {
            UserData user = new UserData();
            var fields = new List<string>();
            fields.Add("id");
            fields.Add("email");
            fields.Add("fname");
            fields.Add("lname");
            fields.Add("password");
            fields.Add("type");
            List<Dictionary<string, string>> userResult =
                MySqlCommands.Select("users", fields, "email =\'" + email + "\'", null, null);
            if (userResult == null || userResult.Count != 1)
                return null;
            if (userResult[0]["password"] != password)
                return null;
            user.Id = userResult[0]["id"];
            user.Name = userResult[0]["lname"] + " " + userResult[0]["fname"];
            user.FirstName = userResult[0]["fname"];
            user.LastName = userResult[0]["lname"];
            user.Email = userResult[0]["email"];
            user.Type = userResult[0]["type"];
            return user;
        }

        public static int Count(string where)
        {
            return MySqlCommands.Count("users", where);
        }

        public static List<Dictionary<string, string>> GetAllWhere(string where, string orderBy, int page, int resultsPerPage)
        {
            List<string> fields = new List<string>();
            fields.Add("id");
            fields.Add("email");
            fields.Add("fname");
            fields.Add("lname");
            string limit = ((page - 1) * resultsPerPage) + "," + resultsPerPage;
            List<Dictionary<string, string>> requestsResult = MySqlCommands.Select("users", fields, where, orderBy, limit);
            return requestsResult;
        }
    }
}