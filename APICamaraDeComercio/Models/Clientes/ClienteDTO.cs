namespace APICamaraDeComercio.Models.Clientes
{
    public class ClienteDTO
    {
        public string identificador { get; set; }
        public string? razonSocial { get; set; }
        public string? direccionFiscal { get; set; }
        public string? paisFiscal { get; set; }
        public string? codigoPostalFiscal { get; set; }
        public string? telefono { get; set; }
        public string? situacionImpositiva { get; set; }
        public string? tipoDeDocumento { get; set; }
        public string? numeroDeDocumento { get; set; }
        public string? jurisdiccionFiscal { get; set; }
        public string? categoria { get; set; }
        public string? condicionDePago { get; set; }
        public string? listaDePrecios { get; set; }
        public string? limiteDeCredito { get; set; }
        public string? direccionEntrega { get; set; }
        public string? paisEntrega { get; set; }
        public string? codigoPostalEntrega { get; set; }
        public string? jurisdiccionEntrega { get; set; }
        public string? observaciones { get; set; }
        public string? mail { get; set; }
        public string? inhibidoParaOperacionesDeFacturacion { get; set; }
        public string? correspondeFacturadecredito { get; set; }
        public string? codigoDeExportacion { get; set; }
        public string? administraCuentaCorriente { get; set; }
        public string? admiteAnticiposenCuentaCorriente { get; set; }
        public string? listaDePreciosVisaciones { get; set; }
        public string? circuitoFacturacionVisaciones { get; set; }
        public string? frecuenciaFacturacion { get; set; }
        public string? deBaja { get; set; }

    }
}
