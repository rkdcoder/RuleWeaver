using RuleWeaver.Abstractions;

namespace RuleWeaver.Rules
{
    public class MaxLengthRule : IValidationRule
    {
        public string Name => "MaxLength";

        public ValueTask<RuleResult> ValidateAsync(object? value, string[] args)
        {
            if (value is null) return new ValueTask<RuleResult>(RuleResult.Success());

            if (args.Length == 0 || !int.TryParse(args[0], out var maxLen))
            {
                return new ValueTask<RuleResult>(RuleResult.Failure("Invalid configuration for MaxLength."));
            }

            if (value is string str)
            {
                if (str.Length > maxLen)
                {
                    return new ValueTask<RuleResult>(RuleResult.Failure($"Length must be at most {maxLen} characters."));
                }
                return new ValueTask<RuleResult>(RuleResult.Success());
            }

            if (value is System.Collections.ICollection collection)
            {
                if (collection.Count > maxLen)
                {
                    return new ValueTask<RuleResult>(RuleResult.Failure($"List must contain at most {maxLen} items."));
                }
                return new ValueTask<RuleResult>(RuleResult.Success());
            }

            return new ValueTask<RuleResult>(RuleResult.Failure("MaxLength applies only to strings or collections."));
        }
    }
}
