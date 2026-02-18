using RuleWeaver.Abstractions;

namespace RuleWeaver.Rules
{
    public class MinLengthRule : IValidationRule
    {
        public string Name => "MinLength";

        public bool Validate(object? value, string[] args, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (value is null) return true;

            if (args.Length == 0 || !int.TryParse(args[0], out var minLen))
            {
                errorMessage = "Invalid configuration for MinLength.";
                return false;
            }

            if (value is string str)
            {
                if (str.Length < minLen)
                {
                    errorMessage = $"Length must be at least {minLen} characters.";
                    return false;
                }
                return true;
            }

            if (value is System.Collections.ICollection collection)
            {
                if (collection.Count < minLen)
                {
                    errorMessage = $"List must contain at least {minLen} items.";
                    return false;
                }
                return true;
            }

            errorMessage = "MinLength applies only to strings or collections.";
            return false;
        }
    }
}
