using System.Configuration;

namespace DigitalIsraelFund_System.DataBase.Managers
{
    public class DBManager
    {
        private static DBManager instance = new DBManager();

        public static DBManager Manager { get { return instance; } }

        private IDBCommands cmds;

        public IDBCommands Cmds { get { return this.cmds; } }

        private DBManager()
        {
            // check the data base to work with
            switch (ConfigurationManager.AppSettings["db_app"].ToLower())
            {
                case "mysql":
                    // work with mySql data base
                    this.cmds = new MySqlCommands();
                    break;
                case "sql":
                    // work with Sql data base
                    this.cmds = new SqlCommands();
                    break;
            }
        }
    }
}