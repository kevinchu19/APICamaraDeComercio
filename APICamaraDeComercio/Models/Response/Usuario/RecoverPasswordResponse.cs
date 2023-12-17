using APICamaraDeComercio.Models.Response.VEP;
using APICamaraDeComercio.Models.Usuario;

namespace APICamaraDeComercio.Models.Response.Usuario
{
    public class RecoverPasswordResponse : BaseResponse<RecoverPasswordDTO>
    {

            public RecoverPasswordResponse(RecoverPasswordDTO response) : base(response)
            {

            }
       
    }
}
