using RuleWeaver.Abstractions;
using System.Text.RegularExpressions;

namespace RuleWeaver.Rules
{
    public class RegexRule : IValidationRule
    {
        public string Name => "Regex";

        public bool Validate(object? value, string[] args, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (value is null || string.IsNullOrEmpty(value.ToString())) return true;

            if (args.Length == 0)
            {
                errorMessage = "Invalid Regex configuration.";
                return false;
            }

            try
            {
                if (!Regex.IsMatch(value.ToString()!, args[0]))
                {
                    errorMessage = "The value does not match the required pattern.";
                    return false;
                }
            }
            catch
            {
                errorMessage = "Invalid Regex pattern.";
                return false;
            }

            return true;
        }
    }
}
