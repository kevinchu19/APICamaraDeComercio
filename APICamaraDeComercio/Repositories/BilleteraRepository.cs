using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Models.Response.Billetera;
using APICamaraDeComercio.Models.Response.Pdf;
using Microsoft.Data.SqlClient;
using APICamaraDeComercio.Models.SeguimientoFacturacion;
using Azure;

namespace APICamaraDeComercio.Repositories
{
    public class BilleteraRepository : RepositoryBase
    {
        public BilleteraRepository (IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<decimal>> GetMontosSugeridosBilletera()
        {
            List<Decimal> response = new List<Decimal>();

            response = await ExecuteStoredProcedureArray<decimal>("ALM_GetBilleteraMontosSugeridos");

            return response;

        }
        public async Task<List<BilleteraDTO?>> GetBilletera (string numeroDocumento, string? fechaDesde, string? fechaHasta, string bussinessUnit)
        {
            List<BilleteraDTO?> response = new List<BilleteraDTO?> ();    

            response = await ExecuteStoredProcedureList<BilleteraDTO?>("ALM_GetBilleteraForAPI",
                                                                           new Dictionary<string, object>{
                                                                                { "@NumeroDocumento", numeroDocumento },
                                                                                { "@FechaDesde", fechaDesde is null ? DBNull.Value : fechaDesde},
                                                                                { "@FechaHasta", fechaHasta is null ? DBNull.Value : fechaHasta},
                                                                                { "@CodigoImputacion", bussinessUnit}
                                                                           });
            
            return response;
        }


        public async Task<SaldoBilleteraDTO?> GetSaldoBilletera(string numeroDocumento, string bussinessUnit)
        {
            

            return await ExecuteStoredProcedure<SaldoBilleteraDTO?>("ALM_GetSaldoBilleteraForAPI",
                                                                           new Dictionary<string, object>{
                                                                                { "@NumeroDocumento", numeroDocumento },
                                                                                { "@CodigoImputacion", bussinessUnit}
                                                                           });

        }
    }
}
