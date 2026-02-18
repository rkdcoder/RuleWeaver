namespace RuleWeaver.Core
{
    public class ValidationErrorDetail
    {
        public string Property { get; }

        public List<ValidationFailure> Errors { get; }

        public ValidationErrorDetail(string property, List<ValidationFailure> errors)
        {
            Property = property;
            Errors = errors;
        }
    }
}
