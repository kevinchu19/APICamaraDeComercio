namespace APICamaraDeComercio.Models.Response.VEP
{
    public class VEPDTO
    {
        public Int64 numeroVEP { get; set; }
        public DateTime fecha { get; set; }
        public string medioDePago { get; set; }
        public decimal importe { get; set; }
        public string estado { get; set; }
        public string generador { get; set; }

        public List<VEPComprobanteDTO?> comprobantes { get; set; }
    }
}
