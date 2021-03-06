﻿using DigitalIsraelFund_System.Models;
using System.Collections.Generic;
using System;

namespace DigitalIsraelFund_System.DataBase.Managers
{
    public class UserManager
    {
        private static UserManager instance = new UserManager();

        public static UserManager Manager { get { return instance; } }

        private Dictionary<string, int> currectAttempts;
        private Dictionary<string, DateTime> blockedUsers;

        private UserManager()
        {
            this.currectAttempts = new Dictionary<string, int>();
            this.blockedUsers = new Dictionary<string, DateTime>();
        }

        public bool isUserAllowed(string username)
        {
            if (this.blockedUsers.ContainsKey(username))
            {
                // user was blocked
                DateTime now = DateTime.UtcNow;
                DateTime then = this.blockedUsers[username];
                // check if block time is over
                if (now.Subtract(then).Minutes > 5)
                {
                    // user is no longer blocked
                    this.blockedUsers.Remove(username);
                }
            }
            return !this.blockedUsers.ContainsKey(username);
        }

        public int addIncorrectAttempt(string username)
        {
            int currIncorrectAttempt = 1;
            if (currectAttempts.ContainsKey(username))
            {
                currIncorrectAttempt = currectAttempts[username] + 1;
            }
            currectAttempts[username] = currIncorrectAttempt;
            if (currIncorrectAttempt >= 3)
            {
                // user got blocked
                this.currectAttempts.Remove(username);
                this.blockedUsers[username] = DateTime.UtcNow;
            }
            return currIncorrectAttempt;
        }

        public bool Add(string email, string password, string fname, string lname, string type, string office, string phone, string cellPhone)
        {
            // attempts to add the given user to the data base
            var values = new Dictionary<string, string>();
            values["email"] = email;
            values["fname"] = fname;
            values["lname"] = lname;
            values["password"] = password;
            values["type"] = type;
            values["office"] = office;
            values["phone"] = phone;
            values["cell_phone"] = cellPhone;
            return DBManager.Manager.Cmds.Insert("users", values);
        }

        public UserData GetIfCorrect(string email, string password)
        {
            UserData user = new UserData();
            var fields = new List<string>();
            fields.Add("users.id as id");
            fields.Add("email");
            fields.Add("fname");
            fields.Add("lname");
            fields.Add("password");
            fields.Add("type");
            fields.Add("office_name");
            fields.Add("phone");
            fields.Add("cell_phone");
            // get the user with the given email
            List<Dictionary<string, string>> userResult =
                DBManager.Manager.Cmds.Select("users LEFT JOIN offices", fields,
                "users.office=offices.id", "email =\'" + email + "\'", null, null);
            if (userResult == null || userResult.Count != 1)
                // no such user found
                return null;
            if (userResult[0]["password"] != password)
                // passwords do not match
                return null;
            // email & pass are correct, return a new UserData model
            user.Id = userResult[0]["id"];
            user.Name = userResult[0]["lname"] + " " + userResult[0]["fname"];
            user.FirstName = userResult[0]["fname"];
            user.LastName = userResult[0]["lname"];
            user.Email = userResult[0]["email"];
            user.Type = userResult[0]["type"];
            user.Office = userResult[0]["office_name"];
            user.Phone = userResult[0]["phone"];
            user.CellPhone = userResult[0]["cell_phone"];
            return user;
        }

        public bool Change(string user_id, Dictionary<string, string> newValues)
        {
            return DBManager.Manager.Cmds.Update("users", newValues, "id='" + user_id + "'");
        }

        public int Count(string where)
        {
            string on = "users.office=offices.id";
            return DBManager.Manager.Cmds.Count("users LEFT JOIN offices", where, on);
        }

        public List<Dictionary<string, string>> GetAllWhere(string where, string orderBy, int page, int resultsPerPage)
        {
            List<string> fields = new List<string>();
            fields.Add("users.id as id");
            fields.Add("email");
            fields.Add("fname");
            fields.Add("lname");
            fields.Add("office_name");
            fields.Add("phone");
            fields.Add("cell_phone");
            // add limit and order
            string limit = ((page - 1) * resultsPerPage) + "," + resultsPerPage;
            string on = "users.office=offices.id";
            if (orderBy == null || orderBy == "") orderBy = "users.id";
            List<Dictionary<string, string>> requestsResult = DBManager.Manager.Cmds.Select("users LEFT JOIN offices",
                fields, on, where, orderBy, limit);
            return requestsResult;
        }

        public List<string> GetFieldWhere(string field, string where, string orderBy, int page, int resultsPerPage)
        {
            List<string> fields = new List<string>();
            fields.Add("DISTINCT " + field);
            string limit = ((page - 1) * resultsPerPage) + "," + resultsPerPage;
            string on = "users.office=offices.id";
            if (orderBy == null || orderBy == "") orderBy = "users.id";
            List<Dictionary<string, string>> requestsResult = DBManager.Manager.Cmds.Select("users LEFT JOIN offices",
                fields, on, where, orderBy, limit);
            // turn the dictionary result into a list (can do cause only 1 field)
            List<string> results = new List<string>();
            if (requestsResult != null)
                foreach (Dictionary<string, string> row in requestsResult)
                    results.Add(row[field]);
            return results;
        }
    }
}