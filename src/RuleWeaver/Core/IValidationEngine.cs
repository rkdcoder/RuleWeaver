namespace RuleWeaver.Core
{
    public interface IValidationEngine
    {
        List<ValidationErrorDetail> Validate(object model);
    }
}
