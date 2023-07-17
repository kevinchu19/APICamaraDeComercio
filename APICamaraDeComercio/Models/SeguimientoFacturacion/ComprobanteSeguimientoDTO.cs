namespace APICamaraDeComercio.Models.SeguimientoFacturacion
{
    public class ComprobanteSeguimientoDTO
    {
        public string fechaFormulario { get; set; }
        public string codigoFormulario { get; set; }
        public int numeroFormulario { get; set; }
        public string numeroAfip { get; set; }
        public decimal importeTotal { get; set; }
        public List<ComprobanteSeguimientoDTO> comprobantesAsociados { get; set; }
    }
}
