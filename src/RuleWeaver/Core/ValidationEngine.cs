using RuleWeaver.Abstractions;

namespace RuleWeaver.Core
{
    public class ValidationEngine : IValidationEngine
    {
        private readonly IValidationCache _cache;
        private readonly Dictionary<string, IValidationRule> _rulesMap;

        public ValidationEngine(IValidationCache cache, IEnumerable<IValidationRule> rules)
        {
            _cache = cache;
            _rulesMap = rules.ToDictionary(r => r.Name, r => r, StringComparer.OrdinalIgnoreCase);
        }

        public List<ValidationErrorDetail> Validate(object model)
        {
            var errorsList = new List<ValidationErrorDetail>();
            if (model is null) return errorsList;

            var plans = _cache.GetPlanForType(model.GetType());

            if (plans.Count == 0) return errorsList;

            foreach (var propPlan in plans)
            {
                var prop = model.GetType().GetProperty(propPlan.PropertyName);
                if (prop == null) continue;

                var value = prop.GetValue(model);
                var currentPropertyMessages = new List<string>();

                foreach (var step in propPlan.Steps)
                {
                    if (_rulesMap.TryGetValue(step.RuleName, out var ruleImplementation))
                    {
                        if (!ruleImplementation.Validate(value, step.Args, out var technicalError))
                        {
                            string finalMessage = !string.IsNullOrWhiteSpace(step.CustomErrorMessage)
                                ? step.CustomErrorMessage
                                : (!string.IsNullOrWhiteSpace(technicalError) ? technicalError : $"Error in {step.RuleName}");

                            if (!currentPropertyMessages.Contains(finalMessage))
                            {
                                currentPropertyMessages.Add(finalMessage);
                            }
                        }
                    }
                    else
                    {

                    }
                }

                if (currentPropertyMessages.Count > 0)
                {
                    errorsList.Add(new ValidationErrorDetail(propPlan.PropertyName, currentPropertyMessages));
                }
            }

            return errorsList;
        }
    }
}