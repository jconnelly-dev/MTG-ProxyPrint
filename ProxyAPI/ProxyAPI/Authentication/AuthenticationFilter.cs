using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyAPI.Authentication
{
    public class AuthenticationFilter : Attribute, IAuthorizationFilter
    {
        private const string _modelErrorKey = "Unauthorized";

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context == null || context.HttpContext == null || context.HttpContext.Request == null || context.HttpContext.Request.Headers == null)
            {
                // Short-circuit HTTP request pipeline to force ASP.NET Core to exit and produce an error response.
                context.ModelState.AddModelError(_modelErrorKey, "Invalid Request.");
                context.Result = new UnauthorizedObjectResult(context.ModelState);
                return;
            }

            try
            {
                BasicApiRequestAuthentication(context.HttpContext.Request);
            }
            catch (UnauthorizedAccessException)
            {
                // Short-circuit HTTP request pipeline to force ASP.NET Core to exit and produce an error response.
                context.ModelState.AddModelError(_modelErrorKey, "Unauthorized Access.");
                context.Result = new UnauthorizedObjectResult(context.ModelState);
                return;
            }
            catch (Exception)
            {
                // Short-circuit HTTP request pipeline to force ASP.NET Core to exit and produce an error response.
                context.ModelState.AddModelError(_modelErrorKey, "Invalid Request.");
                context.Result = new UnauthorizedObjectResult(context.ModelState);
                return;
            }
        }

        private static void BasicApiRequestAuthentication(HttpRequest request)
        {
            const string RECOGNIZED_USER = "boss-hog";
            string userName = GetHeaderValue(request.Headers, AuthenticationKeys.AUTH_HEADER_PARAM_USERNAME);
            if (string.IsNullOrEmpty(userName) || string.IsNullOrWhiteSpace(userName) || userName != RECOGNIZED_USER)
            {
                throw new UnauthorizedAccessException();
            }

            const string RECOGNIZED_KEY = "Gilligan'sBox(4,2x)(4,2)";
            string apiKey = GetHeaderValue(request.Headers, AuthenticationKeys.AUTH_HEADER_PARAM_APIKEY);
            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrWhiteSpace(apiKey) || apiKey != RECOGNIZED_KEY)
            {
                throw new UnauthorizedAccessException();
            }
        }

        private static string GetHeaderValue(IHeaderDictionary headers, string key)
        {
            StringValues values = new StringValues();
            if (headers != null && !string.IsNullOrEmpty(key) && !headers.TryGetValue(key, out values))
            {
                return null;
            }

            string value = values.FirstOrDefault();
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return value.Trim();
        }
    }
}
