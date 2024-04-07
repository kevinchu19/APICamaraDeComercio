using System.Text.Json.Serialization;

namespace APICamaraDeComercio.Models.Response.Login
{
    public class LoginResponse
    {
        public string? token { get; set; }
        public DateTime? expirationDate { get; set; }
        public string? nombre { get; set; }
        public string? id { get; set; }
        public string? rol{ get; set; }
        public string? businessUnit { get; set; }
        public string? razonSocial { get; set; }
        public string? numeroDocumento { get; set; }
        [JsonIgnore]
        public bool calculaVencimientos { get; set; }
        [JsonIgnore]
        public bool primerAcceso { get; set; }
        [JsonIgnore]
        public bool passwordReseteada { get; set; }
        [JsonIgnore]
        public bool terminosYCondiciones { get; set; }


        public string? mensaje { get; set; }
    }
}

