using System;
using System.Collections.Generic;

namespace DigitalIsraelFund_System.DataBase
{
    public class MySqlCommands
    {
        public static bool Update(string tableName, Dictionary<string, string> updates, string where)
        {
            string query = "UPDATE " + tableName + " SET ";
            foreach (string key in updates.Keys)
                query += key + "='" + updates[key] + "' ";
            if (where != null && where != "")
                query += "WHERE " + where;
            return MySqlConnector.Connector.RunNonQueryCommand(query);
        }

        public static bool Insert(string tableName, Dictionary<string, string> values)
        {
            string query = "INSERT INTO " + tableName + " (";
            query += String.Join(",", values.Keys) + ")";
            query += " VALUES(";
            foreach (string key in values.Keys)
                query += "'" + values[key] + "',";
            query = query.Substring(0, query.Length - 1) + ")";
            return MySqlConnector.Connector.RunNonQueryCommand(query);
        }

        public static List<Dictionary<string, string>> Select(string tableName, List<string> fields,
            string where, string orderBy)
        {
            string query = "SELECT ";
            if (fields == null)
                query += "*";
            else
            {
                query += String.Join(",", fields);
            }
            query += " FROM " + tableName;
            if (where != null && where != "")
                query += " WHERE " + where;
            if (orderBy != null && orderBy != "")
                query += " ORDER BY " + orderBy;
            return MySqlConnector.Connector.RunQueryCommand(query);
        }
    }
}