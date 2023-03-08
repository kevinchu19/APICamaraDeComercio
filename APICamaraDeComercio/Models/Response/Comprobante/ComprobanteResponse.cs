using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICamaraDeComercio.Models.Response
{
    public class ComprobanteResponse : BaseResponse<ComprobanteDTO>
    {
        public ComprobanteResponse(ComprobanteDTO response):base(response)
        {

        }
    }
}
