namespace RuleWeaver.Core
{
    public class ValidationErrorDetail
    {
        public string Property { get; set; } = string.Empty;
        public List<string> Messages { get; set; } = new();

        public ValidationErrorDetail(string property, List<string> messages)
        {
            Property = property;
            Messages = messages;
        }
    }
}
