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

        public async Task<VEPDTO?> PostVEP (VEPDTO vep)
        {
            VEPDTO? response = new VEPDTO();

            try
            {

                response = await ExecuteStoredProcedure<VEPDTO?>("Alm_PostVEPHeaderForAPI",
                                                                               new Dictionary<string, object>{
                                                                                    { "@Fchmov", vep.fecha },
                                                                                    { "@Medpag", vep.medioDePago },
                                                                                    { "@Import", vep.importe},
                                                                                    { "@Estado", vep.estado},
                                                                                    { "@Nrodoc", vep.numeroDocumento},
                                                                                    { "@BusinessUnit", vep.businessUnit}
                                                                               });
                if (response != null)
                {
                    foreach (VEPComprobanteDTO comprobante in vep.comprobantes)
                    {
                        response.comprobantes.Add(await ExecuteStoredProcedure<VEPComprobanteDTO?>("Alm_PostVEPDocumentForAPI",
                                                                                    new Dictionary<string, object>{
                                                                                        { "@Nrovep", response.numeroVEP },
                                                                                        { "@Codemp", "CAC01" },
                                                                                        { "@Modfor", "VT"},
                                                                                        { "@Codfor", comprobante.comprobante.Substring(0,6)},
                                                                                        { "@Nrofor", Int64.Parse(comprobante.comprobante.Substring(7,8))},
                                                                                        { "@Import", comprobante.importe}
                                                                                    }));
                    }
                }
            

            }
            catch (Exception ex)
            {
                response = new VEPDTO();   
                response.mensaje = ex.Message;
            }

            return response;
        }
    }
}
