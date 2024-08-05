using Microsoft.AspNetCore.Mvc;

namespace APICamaraDeComercio.Services.ApiKey
{
    public class ApiKeyAttribute : ServiceFilterAttribute
    {
        public ApiKeyAttribute()
            : base(typeof(ApiKeyAuthFilter))
        {
        }
    }
}
