using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DigitalIsraelFund_System.DataBase
{
    public class TypeValidator
    {
        private Dictionary<string, Regex> regexes;

        private static TypeValidator validator = new TypeValidator();

        public static TypeValidator Validator { get { return validator; } }

        private TypeValidator()
        {
            this.regexes = new Dictionary<string, Regex>();
            // add all supported Types into the regex dictionary
            this.regexes["Email"] = new Regex(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            this.regexes["Integer"] = new Regex(@"^-*[0-9]+$");
            this.regexes["Real"] = new Regex("^([0-9]+[.][0-9])|([0-9][.][0-9]+)|([0-9]+)$");
            this.regexes["Name"] = new Regex("^([- .A-Za-z0-9א-ת\"]+)$");
            this.regexes["Letters"] = new Regex("^([- .,A-Za-z0-9א-ת_\n\r\"]+)$");
            this.regexes["Date"] = new Regex(@"^([0-9]{4,4}-[0-9]{2,2}-[0-9]{2,2})$");
            this.regexes["Password"] = new Regex(@"^([- .@!#$%&*A-Za-z0-9_]+)$");
            this.regexes["Phone"] = new Regex(@"^([0-9-]*)$");
        }

        public bool Validate(string input, string type)
        {
            if (input == null || !regexes.ContainsKey(type))
                return false;
            Match m = regexes[type].Match(input);
            // true = successive match & equal length
            return m.Length == input.Length && m.Success;
        }
    }
}