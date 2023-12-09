namespace APICamaraDeComercio.Models.Facturacion
{
    public class FacturacionDTO
    {
        public string identificador { get; set; }
        public string numero_de_documento { get; set; }
        public string fecha_de_movimento { get; set; }
        public string? observaciones { get; set; }
        public string? email { get; set; }
        public string? numero_de_documento_generador { get; set; }

        public List<FacturacionItemsDTO> items_a_facturar { get; set; }

    }

    public class FacturacionItemsDTO
    {
        public string tipo_producto { get; set; }
        public string producto { get; set; }
        public decimal cantidad { get; set; }
        public string numero_de_certificado { get; set; }
        public string? textos { get; set; }

    }
}
