using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICamaraDeComercio.Models.Response.Pdf
{
    public class PdfDTO
    {
        public string? pdf { get; private set; }
        public string mensaje { get; private set; }

        public PdfDTO(string? _pdf)
        {
            pdf = _pdf;
        }

        public PdfDTO(string? _pdf, string _mensaje)
        {
            mensaje = _mensaje;
        }
    }
}
