using APICamaraDeComercio.Models.Clientes;
using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Models.Response.Pdf;
using Microsoft.Data.SqlClient;

namespace APICamaraDeComercio.Repositories
{
    public class ClienteRepository: RepositoryBase
    {
        public ClienteRepository(IConfiguration configuration) : base(configuration)
        {
        }


        public async Task<ClienteDTO?> GetCliente (string numeroDocumento)
        {
            return await ExecuteStoredProcedure<ClienteDTO?>("ALM_GetClienteForAPI",
                                                                           new Dictionary<string, object>{
                                                                                { "@NumeroDocumento", numeroDocumento }
                                                                           });

        }
        public override async Task<ComprobanteResponse> GetTransaccion(string identificador, string table)
        {
            string query = $"SELECT * FROM {table} WHERE {table}_IDENTI = '{identificador}'";

            using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("DefaultConnectionString")))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    try
                    {
                        await connection.OpenAsync();

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync())
                                {
                                    switch ((string)reader[$"{table}_STATUS"])
                                    {
                                        case "E":
                                            return new ComprobanteResponse(new ComprobanteDTO(identificador, (string)reader[$"{table}_STATUS"], "Procesada con error", (string)reader[$"{table}_ERRMSG"], null));

                                        case "S":
                                            return new ComprobanteResponse(new ComprobanteDTO(identificador,
                                                                                    (string)reader[$"{table}_STATUS"],
                                                                                    "Procesada Exitosamente",
                                                                                    "",
                                                                                    new ComprobanteGenerado
                                                                                    {
                                                                                        codigocomprobante = (string)(reader[$"{table}_NROCTA"])
                                                                                    }));
                                        case "N":
                                            return new ComprobanteResponse(new ComprobanteDTO(identificador,
                                                                                   (string)reader[$"{table}_STATUS"],
                                                                                    "Pendiente de procesar",
                                                                                    "",
                                                                                    null));

                                        default:
                                            break;
                                    }

                                }
                            }
                            else
                            {
                                return new ComprobanteResponse(new ComprobanteDTO(identificador, "404", "Identificador Inexistente", $"El identificador {identificador} no existe.", null));
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        return new ComprobanteResponse(new ComprobanteDTO(identificador, "500", "Error de acceso", $"Error de conexion con la base de datos", null));
                    }

                    return new ComprobanteResponse(new ComprobanteDTO(identificador, "200", "", "", null));
                }


            }
        }
    }
}
