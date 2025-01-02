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


        public async Task<List<DeudaDTO?>> GetDeuda (string numeroDocumento, string businessUnit, string ?fechaDesde, string? fechaHasta, string tipoDocumento)
        {
            return await ExecuteStoredProcedureList<DeudaDTO?>("ALM_GetDeudaForAPI",
                                                                           new Dictionary<string, object>{
                                                                                { "@NumeroDocumentoGenerador", numeroDocumento },
                                                                                { "@CodigoImputacion", businessUnit},
                                                                                { "@FechaDesde", fechaDesde is null ? DBNull.Value : fechaDesde},
                                                                                { "@FechaHasta", fechaHasta is null ? DBNull.Value : fechaHasta},
                                                                                { "@TipoDocumento", tipoDocumento}
                                                                           });

        }
       
    }
}
