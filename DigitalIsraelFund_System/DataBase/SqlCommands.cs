using System;
using System.Collections.Generic;

namespace DigitalIsraelFund_System.DataBase
{
    public class SqlCommands : IDBCommands
    {
        private string ConvertToUnicodeRequest(string vars)
        {
            if (vars == null)
                return null;
            // convert the mySql syntax to Sql syntax's "true"
            vars = vars.Replace("true", "1=1");
            // add 'N' before strings
            vars = vars.Replace("='", "=N'");
            vars = vars.Replace("LIKE '", "LIKE N'");
            return vars;
        }

        public bool Update(string tableName, Dictionary<string, string> updates, string where)
        {
            // create the update string
            string query = "UPDATE " + tableName + " SET ";
            foreach (string key in updates.Keys)
                query += key + "=N'" + updates[key] + "',";
            query = query.Substring(0, query.Length - 1);
            where = ConvertToUnicodeRequest(where);
            // add the where
            if (where != null && where != "")
                query += " WHERE " + where;
            return SqlConnector.Connector.RunNonQueryCommand(query);
        }

        public bool Delete(string tableName, string where)
        {
            // create the delete string
            string query = "DELETE FROM " + tableName;
            where = ConvertToUnicodeRequest(where);
            // add the where
            query += " WHERE " + where;
            return SqlConnector.Connector.RunNonQueryCommand(query);
        }

        public bool Insert(string tableName, Dictionary<string, string> values)
        {
            // create the Insert string
            string query = "INSERT INTO " + tableName + " (";
            query += String.Join(",", values.Keys) + ")";
            query += " VALUES(";
            // add the values
            foreach (string key in values.Keys)
                query += "N'" + values[key] + "',";
            query = query.Substring(0, query.Length - 1) + ")";
            return SqlConnector.Connector.RunNonQueryCommand(query);
        }

        public bool InsertOrUpdate(string tableName, Dictionary<string, string> values, ICollection<string> toUpdate, string where)
        {
            // build the insert query
            string insertQuery = "INSERT INTO " + tableName + " (";
            insertQuery += String.Join(",", values.Keys) + ")";
            insertQuery += " VALUES(";
            foreach (string key in values.Keys)
                insertQuery += "N'" + values[key] + "',";
            insertQuery = insertQuery.Substring(0, insertQuery.Length - 1) + ")";
            // build the update query
            string updateQuery = "UPDATE " + tableName + " SET ";
            foreach (string key in toUpdate)
                updateQuery += key + "=N'" + values[key] + "',";
            updateQuery = updateQuery.Substring(0, updateQuery.Length - 1);
            where = ConvertToUnicodeRequest(where);
            if (where != null && where != "")
                updateQuery += " WHERE " + where;
            // join all parts to imitate mySql's "On duplicate key"
            string query = "if exists (select * from " + tableName + " where " + where + ")"
                + " begin " + updateQuery + " end "
                + " else begin " + insertQuery + " end";
            return SqlConnector.Connector.RunNonQueryCommand(query);
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
            where = ConvertToUnicodeRequest(where);
            if (where != null && where != "")
                query += " WHERE " + where;
            if (orderBy != null && orderBy != "")
                query += " ORDER BY " + orderBy;

            if (limit != null && limit != "")
            {
                // convert limit to sql style
                int ofset = int.Parse(limit.Split(',')[0]);
                int numRows = int.Parse(limit.Split(',')[1]);
                if (orderBy != null && orderBy != "")
                {
                    query += " OFFSET " + ofset + " ROWS FETCH NEXT " + numRows + " ROWS ONLY";
                }
            }
            return SqlConnector.Connector.RunQueryCommand(query);
        }

        public int Count(string tableName, string where, string on)
        {
            where = ConvertToUnicodeRequest(where);
            // count the number of rows that where = true for them
            var whereQ = where == null || where == "" ? "" : " WHERE " + where;
            string query = "SELECT COUNT(*) AS total FROM " + tableName;
            if (on != null && on != "")
                query += " ON " + on;
            query += whereQ;
            return int.Parse(SqlConnector.Connector.RunQueryCommand(query)[0]["total"]);
        }
    }
}