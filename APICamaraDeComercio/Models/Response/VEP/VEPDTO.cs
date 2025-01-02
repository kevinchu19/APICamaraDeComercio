using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static APICamaraDeComercio.Models.Response.VEP.AllowedMedioDePagoAttribute;

namespace APICamaraDeComercio.Models.Response.VEP
{
   
    public class VEPDTO
    {
        
        public Int32? numeroVEP { get; set; }
        public DateTime? fecha { get; set; } = null;
        
        [AllowedMedioDePago(ErrorMessage = "El medio de pago no es válido.")]
        public string? medioDePago{ get; set; }
        public decimal? importe { get; set; } = null;
        [AllowedEstado(ErrorMessage = "El estado no es válido.")]
        public string estado { get; set; }

        [MaxLength(30)]
        public string? generador { get; set; }
        [MaxLength(6)]
        public string? businessUnit { get; set; }
        public string? barCode { get; set; }
        public string? tipoDocumento { get; set; }
        public string? numeroDocumento { get; set; }
        public string? tipoDocumentoGenerador { get; set; }
        public string? numeroDocumentoGenerador { get; set; }
        public string? codigoAutorizacionTarjeta { get; set; }
        public string? tipoTarjeta { get; set; }
        public DateTime? fechaPago { get; set; } = null;
        public DateTime? fechaVencimiento { get; set; }
        public int? registrosActualizados { get; set; }
        public string? mensaje { get; set; }

        public List<VEPComprobanteDTO?> comprobantes { get; set; } = new List<VEPComprobanteDTO?>();


        public VEPDTO()
        {

        }

        public VEPDTO(string? _mensaje)
        {
            mensaje = _mensaje;
        }
    }

    public class AllowedMedioDePagoAttribute : ValidationAttribute
    {
        private readonly string[] valoresPermitidos = { "Pago Facil", "Tarjeta", "Billetera", "Interbanking" };

        public override bool IsValid(object value)
        {
            if (value == null || !valoresPermitidos.Contains(value.ToString(), StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }
        public class AllowedEstadoAttribute : ValidationAttribute
        {
            private readonly string[] valoresPermitidos = { "Pagado", "Anulado", "Pendiente de Pago" };

            public override bool IsValid(object value)
            {
                if (value == null || !valoresPermitidos.Contains(value.ToString(), StringComparer.OrdinalIgnoreCase))
                {
                    return false;
                }

                return true;
            }
        }
    }
}
