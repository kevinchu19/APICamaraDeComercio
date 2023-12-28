using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Models.Response.Billetera;
using APICamaraDeComercio.Models.Response.Login;
using Microsoft.Data.SqlClient;
using NuGet.Protocol.Plugins;

namespace APICamaraDeComercio.Repositories
{
    public class LoginRepository:RepositoryBase
    {
        public LoginRepository(IConfiguration configuration) : base(configuration)
        { 
        
        }

        public async Task<string?> LoginWithJwt(string usuario, string password) {
            
            string query = $"SELECT * FROM USR_WSTUSH WHERE USR_WSTUSH_CODIGO = '{usuario}'";
            
            using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("DefaultConnectionString")))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
    
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows)
                        {
                            return "El usuario no existe.";
                                
                        }
                    }
                    
                }

            }

            query = $"SELECT * FROM USR_WSTUSH WHERE USR_WSTUSH_CODIGO = '{usuario}' AND USR_WSTUSH_PWD256 = convert(varbinary(max),'0x{password}',1)";

            using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("DefaultConnectionString")))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows)
                        {
                            return "Contraseña incorrecta.";
                        }
                    }

                }
            }

            return null;
        }

        public async Task<LoginResponse> GetLoggedUserData(string userid)
        {
            LoginResponse response = new LoginResponse();

            response = await ExecuteStoredProcedure<LoginResponse>("ALM_GetLoggedUserForAPI",
                                                                         new Dictionary<string, object>{
                                                                                { "@UserId", userid} });
           

            return response;
        }
    }
}
