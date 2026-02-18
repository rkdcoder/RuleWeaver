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
                var currentPropertyViolations = new List<ValidationFailure>();

                foreach (var step in propPlan.Steps)
                {
                    if (step.RuleName.Equals("Nested", StringComparison.OrdinalIgnoreCase))
                    {
                        if (value != null)
                        {
                            if (value is System.Collections.IEnumerable collection && value is not string)
                            {
                                int index = 0;
                                foreach (var item in collection)
                                {
                                    var itemErrors = await ValidateAsync(item);

                                    foreach (var itemError in itemErrors)
                                    {
                                        var newPropertyName = $"{propPlan.PropertyName}[{index}].{itemError.Property}";

                                        errorsList.Add(new ValidationErrorDetail(newPropertyName, itemError.Violations));
                                    }
                                    index++;
                                }
                            }
                            else
                            {
                                var childErrors = await ValidateAsync(value);

                                foreach (var childError in childErrors)
                                {
                                    var newPropertyName = $"{propPlan.PropertyName}.{childError.Property}";

                                    errorsList.Add(new ValidationErrorDetail(newPropertyName, childError.Violations));
                                }
                            }
                        }
                        continue;
                    }

                    if (_rulesMap.TryGetValue(step.RuleName, out var ruleImplementation))
                    {
                        var result = await ruleImplementation.ValidateAsync(value, step.Args);

                        if (!result.IsValid)
                        {
                            string finalMessage = !string.IsNullOrWhiteSpace(step.CustomErrorMessage)
                                       ? step.CustomErrorMessage
                                       : (!string.IsNullOrWhiteSpace(result.ErrorMessage) ? result.ErrorMessage : $"Error in {step.RuleName}");

                            var failure = new ValidationFailure(step.RuleName, finalMessage);
                            if (!currentPropertyViolations.Any(f => f.Rule == failure.Rule && f.Message == failure.Message))
                            {
                                currentPropertyViolations.Add(failure);
                            }
                        }
                    }
                }

                if (currentPropertyViolations.Count > 0)
                {
                    errorsList.Add(new ValidationErrorDetail(propPlan.PropertyName, currentPropertyViolations));
                }
            }

            return errorsList;
        }
    }
}