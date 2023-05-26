using APICamaraDeComercio.Models.Response;
using Microsoft.Data.SqlClient;
using NuGet.Protocol.Plugins;

namespace APICamaraDeComercio.Repositories
{
    public class LoginRepository:RepositoryBase
    {
        public LoginRepository(IConfiguration configuration) : base(configuration)
        { }

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

            query = $"SELECT * FROM USR_WSTUSH WHERE USR_WSTUSH_CODIGO = '{usuario}' AND USR_WSTUSH_PWD256 = HASHBYTES('SHA2_256','{password}')";

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
    }
}
