using APICamaraDeComercio.Services.Entities;
using Microsoft.Data.SqlClient;

namespace APICamaraDeComercio.Repositories
{
    public class FacturacionRepository: RepositoryBase
    {
        public FacturacionRepository(IConfiguration configuration) : base(configuration)
        {
        }
        public override async Task<string> ExecuteSqlInsertToTablaSAR(List<FieldMap> fieldMapList, object resource, object valorIdentificador, string jobName)
        {
            string errorMessage = "";

            errorMessage = await base.ExecuteSqlInsertToTablaSAR(fieldMapList, resource, valorIdentificador, jobName);

            if (errorMessage == "")
            {
                try
                {

                    Alm_InsSAR_FCRMVDFromSAR_FCRMVH((string)valorIdentificador);

                }
                catch (Exception ex)
                {

                    return ex.Message + ex.StackTrace;
                }
            }

            return errorMessage;
        }
        private async Task Alm_InsSAR_FCRMVDFromSAR_FCRMVH(string identificador)
        {
            using (SqlConnection sql = new SqlConnection(Configuration.GetConnectionString("DefaultConnectionString")))
            {
                using (SqlCommand cmd = new SqlCommand("Alm_InsSAR_FCRMVDFromSAR_FCRMVH", sql))
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
