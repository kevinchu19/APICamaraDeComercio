using APICamaraDeComercio.Models.Clientes;
using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Models.Response.Pdf;
using APICamaraDeComercio.Models.Usuario;
using Microsoft.Data.SqlClient;

namespace APICamaraDeComercio.Repositories
{
    public class UsuarioRepository : RepositoryBase
    {
        public UsuarioRepository (IConfiguration configuration) : base(configuration)
        {
        }


        public async Task<RecoverPasswordDTO?> RecoverPassword(string useriId, string newPassword)
        {
            return await ExecuteStoredProcedure<RecoverPasswordDTO?>("Alm_PostPasswordRecoverForAPI",
                                                                           new Dictionary<string, object>{
                                                                                { "@Userid", useriId },
                                                                                { "@NewPassword", newPassword}
                                                                           });

        }

        public async Task<RecoverPasswordDTO?> ChangePassword(string useriId, string newPassword)
        {
            return await ExecuteStoredProcedure<RecoverPasswordDTO?>("Alm_PostPasswordChangeForAPI",
                                                                           new Dictionary<string, object>{
                                                                                { "@Userid", useriId },
                                                                                { "@NewPassword", newPassword}
                                                                           });

        }


        public async Task<TerminosYCondicionesDTO?> UpdateTerminosYCondiciones(string userId, bool result)
        {
            return await ExecuteStoredProcedure<TerminosYCondicionesDTO?>("Alm_PutTerminosYCondicionesForAPI",
                                                                        new Dictionary<string, object>{
                                                                                { "@Userid", userId },
                                                                                { "@Result", result?"S":"N" }    
                                                                        });

        }

    }
}
