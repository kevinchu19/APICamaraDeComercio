using System.ComponentModel.DataAnnotations;

namespace APICamaraDeComercio.Models.SeguimientoFacturacion
{
    public class ComprobanteParams
    {
        [MaxLength(6)]    
        public string codfor { get; set; }
        public int nrofor { get; set; }
    }
}
