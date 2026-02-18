using Microsoft.Extensions.Configuration;

namespace RuleWeaver.Core
{
    public class ValidationCache : IValidationCache
    {
        private readonly IConfiguration _configuration;

        // Cache thread-safe
        private readonly Dictionary<Type, List<PropertyValidationPlan>> _cache = new();

        public ValidationCache(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<PropertyValidationPlan> GetPlanForType(Type type)
        {
            if (_cache.TryGetValue(type, out var plan))
            {
                return plan;
            }

            var newPlan = BuildPlan(type);
            _cache[type] = newPlan;
            return newPlan;
        }

        private List<PropertyValidationPlan> BuildPlan(Type type)
        {
            var plans = new List<PropertyValidationPlan>();
            var typeName = type.Name;
            var typeConfig = _configuration.GetSection($"RuleWeaver:{typeName}");

            if (!typeConfig.Exists()) return plans;

            foreach (var prop in type.GetProperties())
            {
                var rulesArraySection = typeConfig.GetSection(prop.Name);
                if (!rulesArraySection.Exists()) continue;

                var propPlan = new PropertyValidationPlan(prop.Name);

                foreach (var ruleSection in rulesArraySection.GetChildren())
                {
                    var ruleName = ruleSection["RuleName"];
                    var ruleParameter = ruleSection["RuleParameter"];
                    var customErrorMessage = ruleSection["RuleErrorMessage"];

                    if (string.IsNullOrWhiteSpace(ruleName)) continue;

                    var args = ruleParameter != null ? new[] { ruleParameter } : Array.Empty<string>();

                    propPlan.Steps.Add(new RuleExecutionStep(ruleName, args, customErrorMessage));
                }

                if (propPlan.Steps.Any())
                {
                    plans.Add(propPlan);
                }
            }

            return plans;
        }
    }
}
