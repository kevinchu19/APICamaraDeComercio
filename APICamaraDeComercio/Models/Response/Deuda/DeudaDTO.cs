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


    }
}
