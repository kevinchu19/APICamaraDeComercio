using APICamaraDeComercio.Models.Response;
using Microsoft.Data.SqlClient;

namespace APICamaraDeComercio.Repositories
{
    public class ComprobanteRepository: RepositoryBase
    {
        public ComprobanteRepository(IConfiguration configuration) : base(configuration)
        {}

        public async Task<string> GetPdfPath(string codigoFormulario, int numeroFormulario)
        {

            string pdfPath = "";

            string query = $"SELECT SAR_FCRMVH_PATPDF PATPDF FROM SAR_FCRMVH WHERE " +
                $"ISNULL(SAR_FCRMVH_CODFOR,SAR_FCRMVH_CODFVT) = '{codigoFormulario}'" +
                $"AND ISNULL(SAR_FCRMVH_NROFOR,SAR_FCRMVH_NROFVT) = {numeroFormulario}";

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
                                    if (reader["PATPDF"] is not System.DBNull)
                                    {
                                        pdfPath = (string)reader["PATPDF"];
                                    }
                                    
                                }
                            }
                            else
                            {
                                return "";
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        return "";
                    }

                    return pdfPath;
                }


            }
        }

    }
}
