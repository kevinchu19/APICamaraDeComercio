using APICamaraDeComercio.Models.Response.VEP;
using Azure;

namespace APICamaraDeComercio.Repositories
{
    public class VEPRepository : RepositoryBase
    {
        public VEPRepository (IConfiguration configuration) : base(configuration)
        {
        }


        public async Task<List<VEPDTO?>> GetVEPList (string numeroDocumento, string? fechaDesde, string? fechaHasta, string bussinessUnit)
        {
            List<VEPDTO?> response = new List<VEPDTO?> ();    

            response = await ExecuteStoredProcedureList<VEPDTO?>("ALM_GetVEPListForAPI",
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
                                                                                        { "@Import", comprobante.importe},
                                                                                        { "@CodigoImputacion", vep.businessUnit}
                                                                                    }));
                    }
                }

                if (vep.medioDePago == "Billetera") //Si es billetera lo paso a pagado automaticamente
                {
                    await ExecuteStoredProcedure<VEPDTO?>("Alm_PatchVEPForAPI",
                                                                              new Dictionary<string, object>{
                                                                                   { "@NroVEP", response.numeroVEP},
                                                                                   { "@NuevoEstado", "Pagado" },
                                                                                   { "@FechaPago", response.fecha},
                                                                                   { "@CodigoAutorizacion", ""},
                                                                                   { "@TipoTarjeta", ""}
                                                                              });
                    response.estado = "Pagado";
                }
            

            }
            catch (Exception ex)
            {
                response = new VEPDTO();   
                response.mensaje = ex.Message;
            }

            return response;
        }

        public async Task<VEPDTO> GetVEP(int id)
        {

            VEPDTO? response = new VEPDTO();
            response = await ExecuteStoredProcedure<VEPDTO?>("Alm_GetVEPForAPI",
                                                                               new Dictionary<string, object>{
                                                                                    { "@NroVEP", id }
                                                                               });
            
            return response;
        }

        public async Task<VEPDTO> PatchVEP(VEPDTO vep)
        {
            var response = await ExecuteStoredProcedure<VEPDTO?>("Alm_PatchVEPForAPI",
                                                                               new Dictionary<string, object>{
                                                                                   { "@NroVEP", vep.numeroVEP},
                                                                                   { "@NuevoEstado", vep.estado },
                                                                                    { "@FechaPago", vep.fechaPago},
                                                                                    { "@CodigoAutorizacion", vep.codigoAutorizacionTarjeta},
                                                                                    { "@TipoTarjeta", vep.tipoTarjeta}
                                                                               });
          
            return response;
        }
    }
}
