using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyAPI.Authentication
{
    public class AddSwaggerAuthHeaderParams : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = AuthenticationKeys.AUTH_HEADER_PARAM_APIKEY,
                Description = "API authenication key assigned to every user.",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema() { Type = "String" },
                Required = false,
                Example = new OpenApiString("Gilligan'sBox(4,2x)(4,2)") // TODO: this is the real password, use only while debugging!!!
                //Example = new OpenApiString("my-user-password")
            });

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = AuthenticationKeys.AUTH_HEADER_PARAM_USERNAME,
                Description = "API authenication user name assigned to every user.",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema() { Type = "String" },
                Required = false,
                Example = new OpenApiString("boss-hog") // TODO: this is the real username, use only while debugging!!!
                //Example = new OpenApiString("my_user_name")
            });
        }
    }
}
