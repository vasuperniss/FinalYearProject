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
            switch (ConfigurationManager.AppSettings["db_app"].ToLower())
            {
                case "mysql":
                    this.cmds = new MySqlCommands();
                    break;
                case "sql":
                    this.cmds = new SqlCommands();
                    break;
            }
        }
    }
}