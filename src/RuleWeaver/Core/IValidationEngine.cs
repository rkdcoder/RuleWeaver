namespace RuleWeaver.Core
{
    public interface IValidationEngine
    {
        Task<List<ValidationErrorDetail>> ValidateAsync(object model);
    }
}
