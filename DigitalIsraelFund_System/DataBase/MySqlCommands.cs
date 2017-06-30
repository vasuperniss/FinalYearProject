using System;
using System.Collections.Generic;

namespace DigitalIsraelFund_System.DataBase
{
    public class MySqlCommands : IDBCommands
    {
        public bool Update(string tableName, Dictionary<string, string> updates, string where)
        {
            // create the update string
            string query = "UPDATE " + tableName + " SET ";
            foreach (string key in updates.Keys)
                query += key + "='" + updates[key] + "',";
            query = query.Substring(0, query.Length - 1);
            // add the where
            if (where != null && where != "")
                query += " WHERE " + where;
            return MySqlConnector.Connector.RunNonQueryCommand(query);
        }

        public bool Delete(string tableName, string where)
        {
            // create the delete string & add the where
            string query = "DELETE FROM " + tableName;
            query += " WHERE " + where;
            return MySqlConnector.Connector.RunNonQueryCommand(query);
        }

        public bool Insert(string tableName, Dictionary<string, string> values)
        {
            // create the Insert string
            string query = "INSERT INTO " + tableName + " (";
            query += String.Join(",", values.Keys) + ")";
            query += " VALUES(";
            // add the values
            foreach (string key in values.Keys)
                query += "'" + values[key] + "',";
            query = query.Substring(0, query.Length - 1) + ")";
            return MySqlConnector.Connector.RunNonQueryCommand(query);
        }

        public bool InsertOrUpdate(string tableName, Dictionary<string, string> values, ICollection<string> toUpdate, string where)
        {
            // build the insert query
            string query = "INSERT INTO " + tableName + " (";
            query += String.Join(",", values.Keys) + ")";
            query += " VALUES(";
            foreach (string key in values.Keys)
                query += "'" + values[key] + "',";
            query = query.Substring(0, query.Length - 1) + ")";
            // build the update query
            query += " ON DUPLICATE KEY UPDATE ";
            foreach (string key in toUpdate)
                query += key + "='" + values[key] + "',";
            query = query.Substring(0, query.Length - 1);
            return MySqlConnector.Connector.RunNonQueryCommand(query);
        }

        public List<Dictionary<string, string>> Select(string tableName, List<string> fields, string on,
            string where, string orderBy, string limit)
        {
            // build thee select string
            string query = "SELECT ";
            if (fields == null)
                query += "*";
            else
            {
                query += String.Join(",", fields);
            }
            // add the from, on, where and order by
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

        public int Count(string tableName, string where, string on)
        {
            // count the number of rows that where = true for them
            var whereQ = where == null || where == "" ? "" : " WHERE " + where;
            string query = "SELECT COUNT(*) AS total FROM " + tableName;
            if (on != null && on != "")
                query += " ON " + on;
            query += whereQ;
            return int.Parse(MySqlConnector.Connector.RunQueryCommand(query)[0]["total"]);
        }
    }
}