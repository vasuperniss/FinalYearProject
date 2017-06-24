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
                query += key + "='" + updates[key] + "',";
            query = query.Substring(0, query.Length - 1);
            if (where != null && where != "")
                query += " WHERE " + where;
            return MySqlConnector.Connector.RunNonQueryCommand(query);
        }

        public static bool Delete(string tableName, string where)
        {
            string query = "DELETE FROM " + tableName;
            query += " WHERE " + where;
            return MySqlConnector.Connector.RunNonQueryCommand(query);
        }

        public static bool Insert(string tableName, Dictionary<string, string> values)
        {
            string query = "INSERT IGNORE INTO " + tableName + " (";
            query += String.Join(",", values.Keys) + ")";
            query += " VALUES(";
            foreach (string key in values.Keys)
                query += "'" + values[key] + "',";
            query = query.Substring(0, query.Length - 1) + ")";
            return MySqlConnector.Connector.RunNonQueryCommand(query);
        }

        public static bool InsertOrUpdate(string tableName, Dictionary<string, string> values, string keyField)
        {
            string query = "INSERT INTO " + tableName + " (";
            query += String.Join(",", values.Keys) + ")";
            query += " VALUES(";
            foreach (string key in values.Keys)
                query += "'" + values[key] + "',";
            query = query.Substring(0, query.Length - 1) + ")";
            values.Remove(keyField);
            query += " ON DUPLICATE KEY UPDATE ";
            foreach (string key in values.Keys)
                query += key + "='" + values[key] + "',";
            query = query.Substring(0, query.Length - 1);
            return MySqlConnector.Connector.RunNonQueryCommand(query);
        }

        public static List<Dictionary<string, string>> Select(string tableName, List<string> fields, string on,
            string where, string orderBy, string limit)
        {
            string query = "SELECT ";
            if (fields == null)
                query += "*";
            else
            {
                query += String.Join(",", fields);
            }
            query += " FROM " + tableName;
            if (on != null && on != "")
                query += " ON " + on;
            if (where != null && where != "")
                query += " WHERE " + where;
            if (orderBy != null && orderBy != "")
                query += " ORDER BY " + orderBy;
            if (limit != null && limit != "")
                query += " LIMIT " + limit;
            return MySqlConnector.Connector.RunQueryCommand(query);
        }

        public static int Count(string tableName, string where)
        {
            var whereQ = where == null || where == "" ? "" : " WHERE " + where;
            string query = "SELECT COUNT(*) AS total FROM " + tableName + whereQ;
            return int.Parse(MySqlConnector.Connector.RunQueryCommand(query)[0]["total"]);
        }
    }
}