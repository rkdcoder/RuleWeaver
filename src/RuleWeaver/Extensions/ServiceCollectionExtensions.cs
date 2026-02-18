using Microsoft.Extensions.DependencyInjection;
using RuleWeaver.Abstractions;
using RuleWeaver.Core;
using RuleWeaver.Rules;
using System.Reflection;

namespace RuleWeaver.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRuleWeaver(this IServiceCollection services, params Assembly[] assembliesToScan)
        {
            services.AddSingleton<IValidationCache, ValidationCache>();

            services.AddScoped<IValidationEngine, ValidationEngine>();

            services.AddSingleton<IValidationRule, RequiredRule>();
            services.AddSingleton<IValidationRule, EmailRule>();
            services.AddSingleton<IValidationRule, MinValueRule>();
            services.AddSingleton<IValidationRule, MaxValueRule>();
            services.AddSingleton<IValidationRule, MinLengthRule>();
            services.AddSingleton<IValidationRule, MaxLengthRule>();
            services.AddSingleton<IValidationRule, RegexRule>();

            if (assembliesToScan is not null && assembliesToScan.Length > 0)
            {
                var ruleType = typeof(IValidationRule);
                var internalAssembly = typeof(RequiredRule).Assembly;

                foreach (var assembly in assembliesToScan)
                {
                    var customRules = assembly.GetTypes()
                        .Where(t => ruleType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    foreach (var rule in customRules)
                    {
                        if (rule.Assembly != internalAssembly)
                        {
                            services.AddScoped(typeof(IValidationRule), rule);
                        }
                    }
                }
            }

            return services;
        }
    }
}
