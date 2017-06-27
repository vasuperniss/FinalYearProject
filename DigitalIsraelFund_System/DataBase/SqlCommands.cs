using System;
using System.Collections.Generic;

namespace DigitalIsraelFund_System.DataBase
{
    public class SqlCommands : IDBCommands
    {
        public bool Update(string tableName, Dictionary<string, string> updates, string where)
        {
            string query = "UPDATE " + tableName + " SET ";
            foreach (string key in updates.Keys)
                query += key + "='" + updates[key] + "',";
            query = query.Substring(0, query.Length - 1);
            if (where != null && where != "")
                query += " WHERE " + where;
            return SqlConnector.Connector.RunNonQueryCommand(query);
        }

        public bool Delete(string tableName, string where)
        {
            string query = "DELETE FROM " + tableName;
            query += " WHERE " + where;
            return SqlConnector.Connector.RunNonQueryCommand(query);
        }

        public bool Insert(string tableName, Dictionary<string, string> values)
        {
            string query = "INSERT IGNORE INTO " + tableName + " (";
            query += String.Join(",", values.Keys) + ")";
            query += " VALUES(";
            foreach (string key in values.Keys)
                query += "'" + values[key] + "',";
            query = query.Substring(0, query.Length - 1) + ")";
            return SqlConnector.Connector.RunNonQueryCommand(query);
        }

        public bool InsertOrUpdate(string tableName, Dictionary<string, string> values, ICollection<string> toUpdate)
        {
            string query = "INSERT INTO " + tableName + " (";
            query += String.Join(",", values.Keys) + ")";
            query += " VALUES(";
            foreach (string key in values.Keys)
                query += "'" + values[key] + "',";
            query = query.Substring(0, query.Length - 1) + ")";

            query += " ON DUPLICATE KEY UPDATE ";
            foreach (string key in toUpdate)
                query += key + "='" + values[key] + "',";
            query = query.Substring(0, query.Length - 1);
            return SqlConnector.Connector.RunNonQueryCommand(query);
        }

        public List<Dictionary<string, string>> Select(string tableName, List<string> fields, string on,
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
            return SqlConnector.Connector.RunQueryCommand(query);
        }

        public int Count(string tableName, string where, string on)
        {
            var whereQ = where == null || where == "" ? "" : " WHERE " + where;
            string query = "SELECT COUNT(*) AS total FROM " + tableName;
            if (on != null && on != "")
                query += " ON " + on;
            query += whereQ;
            return int.Parse(SqlConnector.Connector.RunQueryCommand(query)[0]["total"]);
        }
    }
}