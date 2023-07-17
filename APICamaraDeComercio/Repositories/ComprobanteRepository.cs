using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Models.Response.Pdf;
using Microsoft.Data.SqlClient;

namespace APICamaraDeComercio.Repositories
{
    public class ComprobanteRepository: RepositoryBase
    {
        public ComprobanteRepository(IConfiguration configuration) : base(configuration)
        {}


        public async Task<PdfDTO?> GetPdfPath(string codigoFormulario, int numeroFormulario)
        {





            return await ExecuteStoredProcedure<PdfDTO>("ALM_GetPdfForAPI",
                                                                            new Dictionary<string, object>{
                                                                                { "@Codfor", codigoFormulario },
                                                                                { "@Nrofor", numeroFormulario}
                                                                            });

            
        }

    }
}
