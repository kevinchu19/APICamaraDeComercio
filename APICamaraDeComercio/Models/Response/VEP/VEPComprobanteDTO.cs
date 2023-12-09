namespace APICamaraDeComercio.Models.Response.VEP
{
    public class VEPComprobanteDTO
    {
        public string comprobante { get; set; }
        public DateTime? fecha { get; set; }
        public string? cliente { get; set; }
        public decimal importe { get; set; }
    }
}