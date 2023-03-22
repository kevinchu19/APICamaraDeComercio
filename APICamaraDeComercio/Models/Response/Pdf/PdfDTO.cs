using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICamaraDeComercio.Models.Response.Pdf
{
    public class PdfDTO
    {
        public string? mensaje { get; set; }
        public string? fecha { get; set; }
        public string? numeroAfip { get; set; }
        public decimal? importeTotal { get; set; }
        public string? pdf { get;  set; }
       

        public PdfDTO()
        {

        }

        public PdfDTO(string? _mensaje)
        {
            mensaje = _mensaje;
        }
    }
}
