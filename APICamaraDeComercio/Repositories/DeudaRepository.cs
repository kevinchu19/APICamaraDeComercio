using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Models.Response.Deuda;
using APICamaraDeComercio.Models.Response.Pdf;
using Microsoft.Data.SqlClient;

namespace APICamaraDeComercio.Repositories
{
    public class DeudaRepository : RepositoryBase
    {
        public DeudaRepository (IConfiguration configuration) : base(configuration)
        {
        }


        public async Task<List<DeudaDTO?>> GetDeuda (string numeroDocumento, string businessUnit)
        {
            return await ExecuteStoredProcedureList<DeudaDTO?>("ALM_GetDeudaForAPI",
                                                                           new Dictionary<string, object>{
                                                                                { "@NumeroDocumentoGenerador", numeroDocumento },
                                                                                { "@CodigoImputacion", businessUnit}
                                                                           });

        }
       
    }
}
