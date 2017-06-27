using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;

namespace DigitalIsraelFund_System.DataBase
{
    public class SqlConnector
    {
        //var connectionString = "Server=tcp:israeldigitalsystemdb.database.windows.net,1433;Initial Catalog=IsraelDigital_system_DB;Persist Security Info=False;User ID=Waternut;Password=Madnut357;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        //using (SqlConnection connection = new SqlConnection(connectionString))
        //{
        //    SqlCommand command = new SqlCommand("select * from users", connection);
        //    command.Connection.Open();
        //    var a = command.ExecuteReader();
        //    while(a.Read())
        //    {
        //        Console.WriteLine(a.GetInt32(0) + " " + a.GetString(1) + " " + a.GetString(2));
        //    }
        //    command.Connection.Close();
        //}

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
                instance = new SqlConnector();
                return false;
            }
        }

        public List<Dictionary<string, string>> RunQueryCommand(string cmd)
        {
            lock (syncLock)
            {
                List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
                if (!this.OpenConnection()) return null;
                SqlCommand sqlCmd = new SqlCommand(cmd, connection);
                try
                {
                    SqlDataReader reader = sqlCmd.ExecuteReader();
                    while (reader.Read())
                    {
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
            lock (syncLock)
            {
                List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
                if (!this.OpenConnection()) return false;
                SqlCommand sqlCmd = new SqlCommand(cmd, connection);
                try
                {
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