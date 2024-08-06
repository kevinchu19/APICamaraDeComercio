using System.Text.Json.Serialization;

namespace APICamaraDeComercio.Models.Response.VEP
{
    public class VEPComprobanteDTO
    {
        [JsonIgnore]
        public int nrovep { get; set; }

        public string comprobante { get; set; }
        public DateTime? fecha { get; set; }
        public string? cliente { get; set; }
        public decimal importe { get; set; }
    }
}