namespace RuleWeaver.Core
{
    public class ValidationErrorDetail
    {
        public string Property { get; }

        public List<ValidationFailure> Violations { get; }

        public ValidationErrorDetail(string property, List<ValidationFailure> violations)
        {
            Property = property;
            Violations = violations;
        }
    }
}
