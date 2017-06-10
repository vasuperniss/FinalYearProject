using System.Collections.Generic;

namespace DigitalIsraelFund_System.Models
{
    public class TableResult
    {
        public int Page { get; set; }
        public int ResultsPerPage { get; set; }
        public string OrderBy { get; set; }
        public bool isDesc { get; set; }
        public List<Dictionary<string, string>> Table { get; set; }
    }
}