using RuleWeaver.Abstractions;
using System.Text.RegularExpressions;

namespace RuleWeaver.Rules
{
    public class EmailRule : IValidationRule
    {
        public string Name => "Email";
        private static readonly Regex _emailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool Validate(object? value, string[] args, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (value is null || string.IsNullOrEmpty(value.ToString())) return true;

            if (!_emailRegex.IsMatch(value.ToString()!))
            {
                errorMessage = "Invalid email format.";
                return false;
            }
            return true;
        }
    }
}
