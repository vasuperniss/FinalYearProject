using System.Collections.Generic;

namespace DigitalIsraelFund_System.Filters
{
    public class AdminGovExpFilter : BaseFilter
    {
        protected override List<string> getAllowedUsers()
        {
            List<string> allowed = new List<string>();
            // this filter allows users with type = momhee or admin
            allowed.Add("admin");
            allowed.Add("momhee");
            return allowed;
        }
    }
}