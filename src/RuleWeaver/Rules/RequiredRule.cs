using RuleWeaver.Abstractions;

namespace RuleWeaver.Rules
{
    public class RequiredRule : IValidationRule
    {
        public string Name => "Required";

        public ValueTask<RuleResult> ValidateAsync(object? value, string[] args)
        {
            if (value is null)
            {
                return new ValueTask<RuleResult>(RuleResult.Failure("This field is required."));
            }

            if (value is string str && string.IsNullOrWhiteSpace(str))
            {
                return new ValueTask<RuleResult>(RuleResult.Failure("This field is required."));
            }

            return new ValueTask<RuleResult>(RuleResult.Success());
        }
    }
}
