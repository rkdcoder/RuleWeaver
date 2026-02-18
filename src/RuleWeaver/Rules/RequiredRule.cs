using RuleWeaver.Abstractions;

namespace RuleWeaver.Rules
{
    public class RequiredRule : IValidationRule
    {
        public string Name => "Required";

        public bool Validate(object? value, string[] args, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (value is null)
            {
                errorMessage = "This field is required.";
                return false;
            }

            if (value is string str && string.IsNullOrWhiteSpace(str))
            {
                errorMessage = "This field is required.";
                return false;
            }

            return true;
        }
    }
}
