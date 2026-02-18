using RuleWeaver.Abstractions;

namespace RuleWeaver.Rules
{
    public class MaxValueRule : IValidationRule
    {
        public string Name => "MaxValue";

        public bool Validate(object? value, string[] args, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (value is null) return true;

            if (args.Length == 0 || !double.TryParse(args[0], out var maxVal))
            {
                errorMessage = "Invalid configuration for MaxValue.";
                return false;
            }

            if (double.TryParse(value.ToString(), out var currentVal))
            {
                if (currentVal > maxVal)
                {
                    errorMessage = $"Value must be at most {maxVal}.";
                    return false;
                }
                return true;
            }

            errorMessage = "MaxValue applies only to numeric types.";
            return false;
        }
    }
}
