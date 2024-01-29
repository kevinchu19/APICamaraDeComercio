namespace APICamaraDeComercio.Models.Response.Deuda
{
    public class DeudaDTO
    {
        public string cliente { get; set; }
        public string comprobante { get; set; }
        public DateTime fechaEmision { get; set; }
        public DateTime fechaVencimiento { get; set; }
        public decimal importe { get; set; }
        public string generador { get; set; }
        public decimal? importePrimerVenc { get; set; }
        public decimal? importeSegundoVenc { get; set; }
        public decimal? importeTercerVenc { get; set; }
        public DateTime? fechaSegundoVenc { get; set; }
        public DateTime? fechaTercerVenc { get; set; }
        public string? leyendaImporte { get; set; }



    }
}
