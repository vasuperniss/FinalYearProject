using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalIsraelFund_System.DataBase
{
    public interface IDBCommands
    {
        bool Update(string tableName, Dictionary<string, string> updates, string where);

        bool Delete(string tableName, string where);

        bool Insert(string tableName, Dictionary<string, string> values);

        bool InsertOrUpdate(string tableName, Dictionary<string, string> values, ICollection<string> toUpdate, string where);

        List<Dictionary<string, string>> Select(string tableName, List<string> fields, string on,
            string where, string orderBy, string limit);

        int Count(string tableName, string where, string on);
    }
}
