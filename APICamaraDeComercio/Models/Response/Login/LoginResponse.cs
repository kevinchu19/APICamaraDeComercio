namespace APICamaraDeComercio.Models.Response.Login
{
    public class LoginResponse
    {
        public string? token { get; set; }
        public DateTime? expirationDate { get; set; }

        public string? mensaje { get; set; }
    }
}
