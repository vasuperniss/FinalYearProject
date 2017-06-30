using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace DigitalIsraelFund_System.DataBase
{
    public class MySqlConnector
    {
        private static MySqlConnector instance = new MySqlConnector();

        public static MySqlConnector Connector
        {
            get { return instance; }
        }

        private MySqlConnection connection;
        private readonly object syncLock = new object();

        private MySqlConnector()
        {
            string server = ConfigurationManager.AppSettings["mySql_server"];
            string database = ConfigurationManager.AppSettings["mySql_database"];
            string uid = ConfigurationManager.AppSettings["mySql_userId"];
            string pass = ConfigurationManager.AppSettings["mySql_password"];
            // create the connection string
            string connStr = "SERVER=" + server + ";DATABASE=" + database;
            connStr += ";UID=" + uid + ";PASSWORD=" + pass + ";";
            this.connection = new MySqlConnection(connStr);
        }

        private bool OpenConnection()
        {
            try
            {
                this.connection.Open();
                return true;
            }
            catch
            {
                // recreate the instance
                instance = new MySqlConnector();
                return false;
            }
        }

        private bool CloseConnection()
        {
            try
            {
                this.connection.Close();
                return true;
            }
            catch
            {
                // recreate the instance
                instance = new MySqlConnector();
                return false;
            }
        }

        public List<Dictionary<string, string>> RunQueryCommand(string cmd)
        {
            // lock the connector
            lock (syncLock)
            {
                List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
                if (!this.OpenConnection()) return null;
                MySqlCommand sqlCmd = new MySqlCommand(cmd, connection);
                try
                {
                    // get the result of the query
                    MySqlDataReader reader = sqlCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // read another row from the result
                        Dictionary<string, string> line = new Dictionary<string, string>();
                        for (int i = 0; i < reader.FieldCount; ++i)
                        {
                            line[reader.GetName(i)] = reader[reader.GetName(i)].ToString();
                        }
                        results.Add(line);
                    }
                    reader.Close();
                    this.CloseConnection();
                    return results;
                }
                catch
                {
                    this.CloseConnection();
                    return null;
                }
            }
        }

        public bool RunNonQueryCommand(string cmd)
        {
            // lock the connector
            lock (syncLock)
            {
                List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
                if (!this.OpenConnection()) return false;
                MySqlCommand sqlCmd = new MySqlCommand(cmd, connection);
                try
                {
                    // attempt to run the command
                    sqlCmd.ExecuteNonQuery();
                    this.CloseConnection();
                    return true;
                }
                catch
                {
                    this.CloseConnection();
                    return false;
                }
            }
        }
    }
}