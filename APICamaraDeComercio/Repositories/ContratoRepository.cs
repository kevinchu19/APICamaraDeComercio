using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Services.Entities;
using Microsoft.Data.SqlClient;

namespace APICamaraDeComercio.Repositories
{
    public class ContratoRepository: RepositoryBase
    {
        public ContratoRepository(IConfiguration configuration) : base(configuration)
        {
        }

        
        public override async Task<string> ExecuteSqlInsertToTablaSAR(List<FieldMap> fieldMapList, object resource, object valorIdentificador, string jobName)
        {
            string errorMessage = "";

            errorMessage = await base.ExecuteSqlInsertToTablaSAR(fieldMapList, resource, valorIdentificador, jobName);

            if (errorMessage=="")
            {
                try
                {

                    Alm_InsSAR_CVMCTDySAR_CVMCTVFromSAR_CVMCTI((string)valorIdentificador);

                }
                catch (Exception ex)
                {

                    return ex.Message + ex.StackTrace;
                }
            }

            return errorMessage;
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
                                                                                        codigocomprobante = ((string)reader[$"{table}_CODEMP"] + "|" +
                                                                                                            (string)reader[$"{table}_CODCON"] +"|" +
                                                                                                            (string)reader[$"{table}_NROCON"] + "|" +
                                                                                                            (string)reader[$"{table}_NROEXT"])
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


        private async Task Alm_InsSAR_CVMCTDySAR_CVMCTVFromSAR_CVMCTI(string identificador)
        {
            using (SqlConnection sql = new SqlConnection(Configuration.GetConnectionString("DefaultConnectionString")))
            {
                using (SqlCommand cmd = new SqlCommand("Alm_InsSAR_CVMCTDySAR_CVMCTVFromSAR_CVMCTI", sql))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Identificador", identificador));

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    
                }
            }
        }
    }
}
