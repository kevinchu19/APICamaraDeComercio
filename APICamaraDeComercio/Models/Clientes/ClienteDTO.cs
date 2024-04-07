using System.ComponentModel.DataAnnotations;

namespace APICamaraDeComercio.Models.Clientes
{
    public class ClienteDTO
    {
        [Key]
        public string identificador { get; set; }
        [MaxLength(255)]
        public string? razonSocial { get; set; }
        [MaxLength(60)]
        public string? nombre { get; set; }
        [MaxLength(60)]
        public string? apellido { get; set; }

        [MaxLength(255)]
        public string? direccionFiscal { get; set; }
        [MaxLength(10)]
        public string? direccionFiscalNumero { get; set; }
        [MaxLength(10)]
        public string? direccionFiscalPiso { get; set; }
        [MaxLength(10)]
        public string? direccionFiscalDepto { get; set; }
        [MaxLength(1)]
        public string? tipoPersona{ get; set; }
        [MaxLength(5)]
        public string? paisFiscal { get; set; }
        [MaxLength(20)]
        public string? codigoPostalFiscal { get; set; }
        [Phone]
        public string? telefono { get; set; }
        [MaxLength(2)]
        public string? situacionImpositiva { get; set; }
        [MaxLength(6)]
        public string? tipoDeDocumento { get; set; }
        [MaxLength(11)]
        public string? numeroDeDocumento { get; set; }
        [MaxLength(6)]
        public string? tipoDeDocumento1 { get; set; }
        [MaxLength(11)]
        public string? numeroDeDocumento1 { get; set; }
        [MaxLength(6)]
        public string? tipoDeDocumento2 { get; set; }
        [MaxLength(11)]
        public string? numeroDeDocumento2 { get; set; }
        [MaxLength(6)]
        public string? tipoDeDocumento3 { get; set; }
        [MaxLength(11)]
        public string? numeroDeDocumento3 { get; set; }
        [MaxLength(6)]
        public string? tipoDeDocumento4 { get; set; }
        [MaxLength(11)]
        public string? numeroDeDocumento4 { get; set; }
        [MaxLength(3)]
        public string? jurisdiccionFiscal { get; set; }
        [MaxLength(6)]
        public string? categoria { get; set; }
        [MaxLength(6)]
        public string? condicionDePago { get; set; }
        [MaxLength(6)]
        public string? listaDePrecios { get; set; }
        
        [MaxLength(6)]
        public string? limiteDeCredito { get; set; }
        [MaxLength(120)]
        public string? direccionEntrega { get; set; }
        [MaxLength(6)]
        public string? paisEntrega { get; set; }
        [MaxLength(20)]
        public string? codigoPostalEntrega { get; set; }

        [MaxLength(3)]
        public string? jurisdiccionEntrega { get; set; }

        public string? observaciones { get; set; }
        [EmailAddress]
        public string? mail { get; set; }
        [MaxLength(1)]
        public string? inhibidoParaOperacionesDeFacturacion { get; set; }
        [MaxLength(1)]
        public string? correspondeFacturadecredito { get; set; }
        [MaxLength(1)]
        public string? codigoDeExportacion { get; set; }
        [MaxLength(1)]
        public string? administraCuentaCorriente { get; set; }
        [MaxLength(1)]
        public string? admiteAnticiposenCuentaCorriente { get; set; }
        [MaxLength(6)]
        public string? listaDePreciosVisaciones { get; set; }
        [MaxLength(6)]
        public string? circuitoFacturacionVisaciones { get; set; }
        [MaxLength(6)]
        public string? frecuenciaFacturacion { get; set; }
        [MaxLength(1)]
        public string? deBaja { get; set; }

    }
}
