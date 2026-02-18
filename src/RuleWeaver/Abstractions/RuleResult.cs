namespace RuleWeaver.Abstractions
{
    public readonly struct RuleResult
    {
        public bool IsValid { get; }
        public string ErrorMessage { get; }

        public RuleResult(bool isValid, string errorMessage = "")
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }

        public static RuleResult Success() => new(true);
        public static RuleResult Failure(string message) => new(false, message);
    }
}
