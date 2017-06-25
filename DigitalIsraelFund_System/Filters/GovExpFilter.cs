using System.Collections.Generic;

namespace DigitalIsraelFund_System.Filters
{
    public class GovExpFilter : BaseFilter
    {
        protected override List<string> getAllowedUsers()
        {
            List<string> allowed = new List<string>();
            allowed.Add("momhee");
            return allowed;
        }
    }
}