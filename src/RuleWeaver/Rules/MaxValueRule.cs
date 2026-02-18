using RuleWeaver.Abstractions;

namespace RuleWeaver.Rules
{
    public class MaxValueRule : IValidationRule
    {
        public string Name => "MaxValue";

        public ValueTask<RuleResult> ValidateAsync(object? value, string[] args)
        {
            if (value is null) return new ValueTask<RuleResult>(RuleResult.Success());

            if (args.Length == 0 || !double.TryParse(args[0], out var maxVal))
            {
                return new ValueTask<RuleResult>(RuleResult.Failure("Invalid configuration for MaxValue."));
            }

            if (double.TryParse(value.ToString(), out var currentVal))
            {
                if (currentVal > maxVal)
                {
                    return new ValueTask<RuleResult>(RuleResult.Failure($"Value must be at most {maxVal}."));
                }
                return new ValueTask<RuleResult>(RuleResult.Success());
            }

            return new ValueTask<RuleResult>(RuleResult.Failure("MaxValue applies only to numeric types."));
        }
    }
}
