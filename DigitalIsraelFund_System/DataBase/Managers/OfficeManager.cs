using System.Collections.Generic;

namespace DigitalIsraelFund_System.DataBase.Managers
{
    public class OfficeManager
    {
        private static OfficeManager instance = new OfficeManager();

        public static OfficeManager Manager { get { return instance; } }

        private OfficeManager()
        {

        }

        public List<Dictionary<string, string>> GetAll()
        {
            List<string> fields = new List<string>();
            fields.Add("id");
            fields.Add("office_name");
            List<Dictionary<string, string>> requestsResult = DBManager.Manager.Cmds.Select("offices", fields, null, null, null, null);
            return requestsResult;
        }

        public bool Add(string office_name)
        {
            var values = new Dictionary<string, string>();
            values["office_name"] = office_name;
            return DBManager.Manager.Cmds.Insert("offices", values);
        }
    }
}