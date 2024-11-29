using APICamaraDeComercio.Models.Response.VEP;
using APICamaraDeComercio.Models.Usuario;

namespace APICamaraDeComercio.Models.Response.Usuario
{
    public class ValidateUsuarioResponse : BaseResponse<ValidateUsuarioDTO>
    {

            public ValidateUsuarioResponse(ValidateUsuarioDTO response) : base(response)
            {

            }
       
    }
}
