using System.Collections.Generic;

namespace DigitalIsraelFund_System.Models
{
    public class PostedForm
    {
        public FormComponent RequestForm { get; set; }
        public Dictionary<string, string> Values { get; set; }

        public PostedForm(FormComponent requestForm, Dictionary<string, string> values)
        {
            this.RequestForm = requestForm;
            this.Values = values;
        }
    }
}