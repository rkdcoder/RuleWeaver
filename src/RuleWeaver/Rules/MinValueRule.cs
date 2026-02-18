using RuleWeaver.Abstractions;

namespace RuleWeaver.Rules
{
    public class MinValueRule : IValidationRule
    {
        public string Name => "MinValue";

        public bool Validate(object? value, string[] args, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (value is null) return true;

            if (args.Length == 0 || !double.TryParse(args[0], out var minVal))
            {
                errorMessage = "Invalid configuration for MinValue.";
                return false;
            }

            if (double.TryParse(value.ToString(), out var currentVal))
            {
                if (currentVal < minVal)
                {
                    errorMessage = $"Value must be at least {minVal}.";
                    return false;
                }
                return true;
            }

            errorMessage = "MinValue applies only to numeric types.";
            return false;
        }
    }
}
