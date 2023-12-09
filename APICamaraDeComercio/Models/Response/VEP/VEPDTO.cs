namespace APICamaraDeComercio.Models.Response.VEP
{
    public class VEPDTO
    {
        public string? mensaje { get; set; }
        public Int32? numeroVEP { get; set; }
        public DateTime? fecha { get; set; } = null;
        public string medioDePago { get; set; }
        public decimal? importe { get; set; } = null;
        public string estado { get; set; }
        public string? generador { get; set; }
        public string businessUnit { get; set; }
        public string? barCode { get; set; }
        public string numeroDocumento { get; set; }
        public List<VEPComprobanteDTO?> comprobantes { get; set; } = new List<VEPComprobanteDTO?>();

        public VEPDTO()
        {

        }

        public VEPDTO(string? _mensaje)
        {
            mensaje = _mensaje;
        }
    }
}
