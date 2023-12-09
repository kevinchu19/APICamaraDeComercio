using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Models.Response.VEP;
using APICamaraDeComercio.Models.Response.Pdf;
using Microsoft.Data.SqlClient;
using APICamaraDeComercio.Models.SeguimientoFacturacion;
using Azure;

namespace APICamaraDeComercio.Repositories
{
    public class VEPRepository : RepositoryBase
    {
        public VEPRepository (IConfiguration configuration) : base(configuration)
        {
        }


        public async Task<List<VEPDTO?>> GetVEP (string numeroDocumento, string? fechaDesde, string? fechaHasta, string bussinessUnit)
        {
            List<VEPDTO?> response = new List<VEPDTO?> ();    

            response = await ExecuteStoredProcedureList<VEPDTO?>("ALM_GetVEPForAPI",
                                                                           new Dictionary<string, object>{
                                                                                { "@NumeroDocumento", numeroDocumento },
                                                                                { "@FechaDesde", fechaDesde is null ? DBNull.Value : fechaDesde},
                                                                                { "@FechaHasta", fechaHasta is null ? DBNull.Value : fechaHasta},
                                                                                { "@CodigoImputacion", bussinessUnit}
                                                                           });
            foreach (VEPDTO? item in response)
            {
                item.comprobantes = await ExecuteStoredProcedureList<VEPComprobanteDTO?>("ALM_GetComprobantesVEPForAPI",
                      new Dictionary<string, object>{
                            { "@NumeroVEP", item.numeroVEP},
                            { "@NumeroDocumento", numeroDocumento}
                         });
            }

            return response;
        }
       
    }
}
