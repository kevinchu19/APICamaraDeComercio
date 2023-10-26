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
    }
}