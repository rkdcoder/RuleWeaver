using RuleWeaver.Abstractions;

namespace RuleWeaver.Rules
{
    public class MinLengthRule : IValidationRule
    {
        public string Name => "MinLength";

        public ValueTask<RuleResult> ValidateAsync(object? value, string[] args)
        {
            if (value is null) return new ValueTask<RuleResult>(RuleResult.Success());

            if (args.Length == 0 || !int.TryParse(args[0], out var minLen))
            {
                return new ValueTask<RuleResult>(RuleResult.Failure("Invalid configuration for MinLength."));
            }

            if (value is string str)
            {
                if (str.Length < minLen)
                {
                    return new ValueTask<RuleResult>(RuleResult.Failure($"Length must be at least {minLen} characters."));
                }
                return new ValueTask<RuleResult>(RuleResult.Success());
            }

            if (value is System.Collections.ICollection collection)
            {
                if (collection.Count < minLen)
                {
                    return new ValueTask<RuleResult>(RuleResult.Failure($"List must contain at least {minLen} items."));
                }
                return new ValueTask<RuleResult>(RuleResult.Success());
            }

            return new ValueTask<RuleResult>(RuleResult.Failure("MinLength applies only to strings or collections."));
        }
    }
}
