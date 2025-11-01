using ContentShare.API.Middlewares;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace ContentShare.API.Swagger
{
    public sealed class HmacHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var m = context.MethodInfo;
            var hasUseHmac =
                m.GetCustomAttribute<UseHmacAttribute>() != null ||
                m.DeclaringType?.GetCustomAttribute<UseHmacAttribute>() != null;

            if (!hasUseHmac) return;

            operation.Parameters ??= new List<OpenApiParameter>();

            void Add(string name, string desc, string ex) =>
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = name,
                    In = ParameterLocation.Header,
                    Required = true,
                    Description = desc,
                    Schema = new OpenApiSchema { Type = "string" },
                    Example = new Microsoft.OpenApi.Any.OpenApiString(ex)
                });

            Add("X-HMAC-TIMESTAMP", "Unix timestamp.", "1736349652");
            Add("X-HMAC-NONCE", "Random 16-byte nonce.", "e3c1a2b6b0df49c8b6a7c3e2f9a1bcde");
            Add("X-HMAC-SIGNATURE", "HMAC-SHA256.", "a1b2c3...deadbeef");
        }
    }
}
