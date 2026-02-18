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

        public async Task<List<ValidationErrorDetail>> ValidateAsync(object model)
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

                var currentPropertyErrors = new List<ValidationFailure>();

                foreach (var step in propPlan.Steps)
                {
                    if (_rulesMap.TryGetValue(step.RuleName, out var ruleImplementation))
                    {
                        var result = await ruleImplementation.ValidateAsync(value, step.Args);

                        if (!result.IsValid)
                        {
                            string finalMessage = !string.IsNullOrWhiteSpace(step.CustomErrorMessage)
                                ? step.CustomErrorMessage
                                : (!string.IsNullOrWhiteSpace(result.ErrorMessage) ? result.ErrorMessage : $"Error in {step.RuleName}");

                            var failure = new ValidationFailure(step.RuleName, finalMessage);

                            if (!currentPropertyErrors.Any(f => f.Rule == failure.Rule && f.Message == failure.Message))
                            {
                                currentPropertyErrors.Add(failure);
                            }
                        }
                    }
                }

                if (currentPropertyErrors.Count > 0)
                {
                    errorsList.Add(new ValidationErrorDetail(propPlan.PropertyName, currentPropertyErrors));
                }
            }

            return errorsList;
        }
    }
}