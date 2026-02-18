using RuleWeaver.Abstractions;

namespace RuleWeaver.Rules
{
    public class MaxLengthRule : IValidationRule
    {
        public string Name => "MaxLength";

        public bool Validate(object? value, string[] args, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (value is null) return true;

            if (args.Length == 0 || !int.TryParse(args[0], out var maxLen))
            {
                errorMessage = "Invalid configuration for MaxLength.";
                return false;
            }

            if (value is string str)
            {
                if (str.Length > maxLen)
                {
                    errorMessage = $"Length must be at most {maxLen} characters.";
                    return false;
                }
                return true;
            }

            if (value is System.Collections.ICollection collection)
            {
                if (collection.Count > maxLen)
                {
                    errorMessage = $"List must contain at most {maxLen} items.";
                    return false;
                }
                return true;
            }

            errorMessage = "MaxLength applies only to strings or collections.";
            return false;
        }
    }
}
