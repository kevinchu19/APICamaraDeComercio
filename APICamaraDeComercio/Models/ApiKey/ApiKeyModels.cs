namespace APICamaraDeComercio.Models.ApiKey
{
    public static class Constants
    {
        public const string ApiKeyHeaderName = "X-API-Key";

        public const string ApiKeyName = "ApiKey";
    }
    public interface IApiKeyValidation
    {
        bool IsValidApiKey(string userApiKey);
    }
}
