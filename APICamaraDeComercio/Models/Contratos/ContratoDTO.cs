namespace APICamaraDeComercio.Models.Contratos
{
    public class ContratoDTO
    {
        public string identificador { get; set; }
        public string tipoDocumento { get; set; }
        public string numeroDocumento { get; set; }
        public string tipoDocumentoFiscal { get; set; }
        public string numeroDocumentoFiscal { get; set; }
        public string fechaInicioFacturacion { get; set; }
        public string fechaFinFacturacion { get; set; }
        public string listaDePrecios { get; set; }
        public string carrera { get; set; }
        public List<ContratoItemsDTO> items { get; set; }

    }

    public class ContratoItemsDTO
    {
        public string tipoDeProducto { get; set; }
        public string codigoDeProducto { get; set; }
        public decimal cantidad { get; set; }
        public string? textoAdicional { get; set; }
        public string fechaVigenciaDesde { get; set; }
        public string fechaVigenciaHasta { get; set; }
        

    }
}
