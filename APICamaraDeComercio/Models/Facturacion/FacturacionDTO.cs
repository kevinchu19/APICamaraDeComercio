using System.ComponentModel.DataAnnotations;

namespace APICamaraDeComercio.Models.Facturacion
{
    public class FacturacionDTO
    {
        [Key]
        public string identificador { get; set; }
        [Required]
        [MaxLength(11)]
        public string numero_de_documento { get; set; }
        public string fecha_de_movimento { get; set; }
        public string? observaciones { get; set; }
        [EmailAddress]
        public string? email { get; set; }
        [MaxLength(11)]
        public string? numero_de_documento_generador { get; set; }
        [MaxLength(6)]
        public string? lista_de_precios { get; set; } = null;

        public string? usuarioApi { get; set; } = "";

        public List<FacturacionItemsDTO> items_a_facturar { get; set; }

    }

    public class FacturacionItemsDTO
    {
        [MaxLength(6)]
        public string tipo_producto { get; set; }
        [MaxLength(30)]
        public string producto { get; set; }
        public decimal cantidad { get; set; }
        public string numero_de_certificado { get; set; }
        public string? textos { get; set; }

    }
}
