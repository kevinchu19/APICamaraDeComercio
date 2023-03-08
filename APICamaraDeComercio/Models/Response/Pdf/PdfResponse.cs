using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICamaraDeComercio.Models.Response.Pdf
{
    public class PdfResponse:BaseResponse<PdfDTO>
    {
        public PdfResponse(PdfDTO response):base(response)
        {

        }
    }
}
