using System.Collections.Generic;

namespace DigitalIsraelFund_System.Filters
{
    public class AdminFilter : BaseFilter
    {
        protected override List<string> getAllowedUsers()
        {
            List<string> allowed = new List<string>();
            // this filter allows only users with type = admin
            allowed.Add("admin");
            return allowed;
        }
    }
}