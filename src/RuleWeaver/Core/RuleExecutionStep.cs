namespace RuleWeaver.Core
{
    public class RuleExecutionStep
    {
        public string RuleName { get; }
        public string[] Args { get; }
        public string? CustomErrorMessage { get; }

        public RuleExecutionStep(string ruleName, string[] args, string? customErrorMessage)
        {
            RuleName = ruleName;
            Args = args;
            CustomErrorMessage = customErrorMessage;
        }
    }
}
