namespace RuleWeaver.Abstractions
{
    public interface IValidationRule
    {
        string Name { get; }

        ValueTask<RuleResult> ValidateAsync(object? value, string[] args);
    }
}
