using APICamaraDeComercio.Models.ApiKey;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using APICamaraDeComercio.Models.Response;
using System.Net;

namespace APICamaraDeComercio.Services.ApiKey
{
    public class ApiKeyAuthFilter : IAuthorizationFilter
    {
        private readonly IApiKeyValidation _apiKeyValidation;

        public ApiKeyAuthFilter(IApiKeyValidation apiKeyValidation)
        {
            _apiKeyValidation = apiKeyValidation;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string userApiKey = context.HttpContext.Request.Headers[Constants.ApiKeyHeaderName].ToString();

            if (string.IsNullOrWhiteSpace(userApiKey))
            {
                ComprobanteResponse response = new ComprobanteResponse(new ComprobanteDTO("",
               "400",
               "Bad Request",
               $"Debe incluir el header {Constants.ApiKeyHeaderName}",
               null));

                context.Result = new ObjectResult(response);
                context.HttpContext.Response.StatusCode =
                            (int)HttpStatusCode.BadRequest;
                return;
            }

            if (!_apiKeyValidation.IsValidApiKey(userApiKey))
            
            {
                ComprobanteResponse response = new ComprobanteResponse(new ComprobanteDTO("",
               "401",
               "Unauthorized",
               $"ApiKey no autorizada a operar con el endpoint",
               null));

                context.Result = new ObjectResult(response);
                context.HttpContext.Response.StatusCode =
                            (int)HttpStatusCode.Unauthorized;
            }
        }
    }
}
