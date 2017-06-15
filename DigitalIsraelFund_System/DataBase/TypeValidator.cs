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
            this.regexes["Email"] = new Regex(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            this.regexes["Integer"] = new Regex(@"^-*[0-9]+$");
            this.regexes["Real"] = new Regex("^([0-9]+[.][0-9])|([0-9][.][0-9]+)|([0-9]+)$");
            this.regexes["Letters"] = new Regex("^([- .,A-Za-z0-9א-ת_\n\"]+)$");
            this.regexes["Date"] = new Regex(@"^([0-9]{4,4}-[0-9]{2,2}-[0-9]{2,2})$");
            this.regexes["Password"] = new Regex(@"^([- .@!#$%&*A-Za-z0-9_]+)$");
        }

        public bool Validate(string input, string type)
        {
            if (!regexes.ContainsKey(type))
                return false;
            Match m = regexes[type].Match(input);
            return m.Length == input.Length && m.Success;
        }
    }
}