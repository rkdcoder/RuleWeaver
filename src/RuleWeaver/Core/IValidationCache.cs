namespace RuleWeaver.Core
{
    public interface IValidationCache
    {
        List<PropertyValidationPlan> GetPlanForType(Type type);
    }
}
