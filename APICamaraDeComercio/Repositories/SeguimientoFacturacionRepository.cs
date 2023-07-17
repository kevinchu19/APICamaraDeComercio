using APICamaraDeComercio.Models.Clientes;
using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Models.Response.Pdf;
using APICamaraDeComercio.Models.SeguimientoFacturacion;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Data.SqlClient;
using System.Data;

namespace APICamaraDeComercio.Repositories
{
    public class SeguimientoFacturacionRepository: RepositoryBase
    {
        public SeguimientoFacturacionRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<ComprobanteSeguimientoDTO?>> GetComprobantesPrincipales (List<ComprobanteParams?> comprobantes, DateTime? fchmov, 
            string? circuitoOrigen)
        {
            
            List<ComprobanteSeguimientoDTO?> response = new List<ComprobanteSeguimientoDTO>();

            DataTable comprobantesTable = new DataTable();
            comprobantesTable.Columns.Add(new DataColumn("CODFOR", typeof(string)));
            comprobantesTable.Columns.Add(new DataColumn("NROFOR", typeof(int)));
            foreach (var item in comprobantes)
            {
                comprobantesTable.Rows.Add(new object[] { item.codfor, item.nrofor});
            }

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("@List", comprobantesTable);

            if (comprobantesTable.Rows.Count == 0)
            {
                parameters.Add("@Fchmov", fchmov);
                parameters.Add("@CircuitoOrigen", circuitoOrigen);
            }
            

            response = await ExecuteStoredProcedureList<ComprobanteSeguimientoDTO?>("Alm_GetSeguimientoFacturacionForAPI", parameters);


            foreach (ComprobanteSeguimientoDTO item in response)
            {
                item.comprobantesAsociados = await ExecuteStoredProcedureList<ComprobanteSeguimientoDTO?>("Alm_GetSeguimientoFacturacionForAPIComprobantesAsociados",
                      new Dictionary<string, object>{
                            { "@Codapl", item.codigoFormulario },
                            { "@Nroapl", item.numeroFormulario}
                         });
                //foreach (var comprobanteAsociado in item.comprobantesAsociados)
                //{
                //    List<ComprobanteParams> buscarAsociados = new List<ComprobanteParams>();
                //    buscarAsociados.Add(new ComprobanteParams { codfor = comprobanteAsociado.codigoFormulario, nrofor = comprobanteAsociado.numeroFormulario });
                //    comprobanteAsociado.comprobantesAsociados = await GetComprobantesPrincipales(buscarAsociados, null, null);
                //}

            }
            return response;
        
        }
    }
}
