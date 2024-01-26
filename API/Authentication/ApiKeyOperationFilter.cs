using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace SeaSlugAPI.Authentication
{
    public class ApiKeyOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attributes = GetServiceFilterAttributes(context);

            if (attributes.Any())
            {
                operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            }
                        },
                        new List<string>()
                    }
                }
            };
            }
        }

        private IEnumerable<ServiceFilterAttribute> GetServiceFilterAttributes(OperationFilterContext context)
        {
            var actionAttributes = context.MethodInfo.GetCustomAttributes<ServiceFilterAttribute>(true)
                                   .Where(attr => attr.ServiceType == typeof(ApiKeyAuthFilter));
            var controllerAttributes = context.MethodInfo.DeclaringType?.GetCustomAttributes<ServiceFilterAttribute>(true)
                                       .Where(attr => attr.ServiceType == typeof(ApiKeyAuthFilter))
                                       ?? Enumerable.Empty<ServiceFilterAttribute>();

            return actionAttributes.Concat(controllerAttributes);
        }
    }
}
