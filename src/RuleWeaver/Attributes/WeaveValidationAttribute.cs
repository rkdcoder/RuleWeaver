using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using RuleWeaver.Core;

namespace RuleWeaver.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class WeaveValidationAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var engine = context.HttpContext.RequestServices.GetService<IValidationEngine>();

            if (engine is not null)
            {
                foreach (var argument in context.ActionArguments.Values)
                {
                    if (argument is null) continue;

                    var type = argument.GetType();
                    if (type.IsClass && type != typeof(string))
                    {
                        var validationDetails = await engine.ValidateAsync(argument);

                        if (validationDetails.Count > 0)
                        {
                            context.Result = new BadRequestObjectResult(new
                            {
                                Message = "Validation errors occurred.",
                                Errors = validationDetails
                            });
                            return;
                        }
                    }
                }
            }

            await next();
        }
    }
}
