using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;

namespace DigitalIsraelFund_System.DataBase
{
    public class SqlConnector
    {

        private static SqlConnector instance = new SqlConnector();

        public static SqlConnector Connector
        {
            get { return instance; }
        }

        private SqlConnection connection;
        private readonly object syncLock = new object();

        private SqlConnector()
        {
            string server = ConfigurationManager.AppSettings["Sql_server"];
            string database = ConfigurationManager.AppSettings["Sql_database"];
            string uid = ConfigurationManager.AppSettings["Sql_userId"];
            string pass = ConfigurationManager.AppSettings["Sql_password"];
            // create the connection string
            string connStr = "Server=" + server
                + ";Initial Catalog=" + database
                + ";Persist Security Info=False;User ID=" + uid
                + ";Password=" + pass
                + ";MultipleActiveResultSets=False;Encrypt=True;"
                + "TrustServerCertificate=False;Connection Timeout=30;";
            this.connection = new SqlConnection(connStr);
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
                instance = new SqlConnector();
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
                instance = new SqlConnector();
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
                SqlCommand sqlCmd = new SqlCommand(cmd, connection);
                try
                {
                    // get the result of the query
                    SqlDataReader reader = sqlCmd.ExecuteReader();
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
                SqlCommand sqlCmd = new SqlCommand(cmd, connection);
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