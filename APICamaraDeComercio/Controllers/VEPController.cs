using APICamaraDeComercio.Models.Facturacion;
using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Models.Response.VEP;
using APICamaraDeComercio.Repositories;
using APICamaraDeComercio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Reflection;

namespace APICamaraDeComercio.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class VEPController : ControllerBase
    {

        private readonly ILogger<FacturacionController> Logger;

        public VEPController(ILogger<FacturacionController> logger, VEPRepository repository, IConfiguration configuration)
        {
            Logger = logger;
            Repository = repository;
            Configuration = configuration;
        }

        public VEPRepository Repository { get; }
        public IConfiguration Configuration { get; }

     

        [HttpGet]
        
        public async Task<ActionResult<List<VEPDTO>>> GetVEP(string? numeroDocumento, string? fechaDesde, string? fechaHasta, string businessUnit)
        {
            if (numeroDocumento is null)
            {
                numeroDocumento = "";
            }
            List<VEPDTO?> VEP = await Repository.GetVEP(numeroDocumento, fechaDesde, fechaHasta, businessUnit);

            if (VEP.Count() > 0)
            {
                return Ok(VEP);
            }
            
            return NotFound(new ComprobanteResponse(new ComprobanteDTO(numeroDocumento, "404", "VEP inexistentes", $"No se encontraron VEP para el numero de documento {numeroDocumento}.", null)));

        }

        [HttpPost]
        public async Task<ActionResult<VEPResponse>> PostVEP([FromBody] VEPDTO vep)
        {
            VEPDTO response = new VEPDTO();
            string ErrorMessage = "";

            if (vep.comprobantes.Count()>0)
            {
                if (vep.importe != vep.comprobantes.Select(v=> v.importe).Sum())
                {
                    return BadRequest(new VEPResponse(new VEPDTO("La suma de importe de los comprobantes debe coincidir con el importe del VEP.")));
                }
            }


            response = await Repository.PostVEP(vep);

            if (response.mensaje != null)
            {
                return BadRequest(new VEPResponse(response));
            }

            return Ok(new VEPResponse(response));    

        }
    }
}