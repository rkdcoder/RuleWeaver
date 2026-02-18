namespace RuleWeaver.Core
{
    public class ValidationFailure
    {
        /// <summary>
        /// The code/name of the rule (e.g. "Required", "MinLength", "UniqueEmail").
        /// </summary>
        public string Rule { get; }

        /// <summary>
        /// The human-readable error message.
        /// </summary>
        public string Message { get; }

        public ValidationFailure(string rule, string message)
        {
            Rule = rule;
            Message = message;
        }
    }
}
