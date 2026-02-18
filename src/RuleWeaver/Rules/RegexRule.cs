using RuleWeaver.Abstractions;
using System.Text.RegularExpressions;

namespace RuleWeaver.Rules
{
    public class RegexRule : IValidationRule
    {
        public string Name => "Regex";

        public ValueTask<RuleResult> ValidateAsync(object? value, string[] args)
        {
            if (value is null || string.IsNullOrEmpty(value.ToString()))
                return new ValueTask<RuleResult>(RuleResult.Success());

            if (args.Length == 0)
            {
                return new ValueTask<RuleResult>(RuleResult.Failure("Invalid Regex configuration."));
            }

            try
            {
                if (!Regex.IsMatch(value.ToString()!, args[0]))
                {
                    return new ValueTask<RuleResult>(RuleResult.Failure("The value does not match the required pattern."));
                }
            }
            catch
            {
                return new ValueTask<RuleResult>(RuleResult.Failure("Invalid Regex pattern."));
            }

            return new ValueTask<RuleResult>(RuleResult.Success());
        }
    }
}
