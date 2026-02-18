namespace RuleWeaver.Core
{
    public class PropertyValidationPlan
    {
        public string PropertyName { get; }
        public List<RuleExecutionStep> Steps { get; } = new();

        public PropertyValidationPlan(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
