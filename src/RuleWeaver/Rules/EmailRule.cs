using RuleWeaver.Abstractions;
using System.Text.RegularExpressions;

namespace RuleWeaver.Rules
{
    public class EmailRule : IValidationRule
    {
        public string Name => "Email";
        private static readonly Regex _emailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ValueTask<RuleResult> ValidateAsync(object? value, string[] args)
        {
            if (value is null || string.IsNullOrEmpty(value.ToString()))
                return new ValueTask<RuleResult>(RuleResult.Success());

            if (!_emailRegex.IsMatch(value.ToString()!))
            {
                return new ValueTask<RuleResult>(RuleResult.Failure("Invalid email format."));
            }
            return new ValueTask<RuleResult>(RuleResult.Success());
        }
    }
}
