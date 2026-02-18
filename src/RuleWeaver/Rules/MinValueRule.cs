using RuleWeaver.Abstractions;

namespace RuleWeaver.Rules
{
    public class MinValueRule : IValidationRule
    {
        public string Name => "MinValue";

        public ValueTask<RuleResult> ValidateAsync(object? value, string[] args)
        {
            if (value is null) return new ValueTask<RuleResult>(RuleResult.Success());

            if (args.Length == 0 || !double.TryParse(args[0], out var minVal))
            {
                return new ValueTask<RuleResult>(RuleResult.Failure("Invalid configuration for MinValue."));
            }

            if (double.TryParse(value.ToString(), out var currentVal))
            {
                if (currentVal < minVal)
                {
                    return new ValueTask<RuleResult>(RuleResult.Failure($"Value must be at least {minVal}."));
                }
                return new ValueTask<RuleResult>(RuleResult.Success());
            }

            return new ValueTask<RuleResult>(RuleResult.Failure("MinValue applies only to numeric types."));
        }
    }
}
