using APICamaraDeComercio.Models.Facturacion;
using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Models.Response.Deuda;
using APICamaraDeComercio.Repositories;
using APICamaraDeComercio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Reflection;
using System.Security.Claims;

namespace APICamaraDeComercio.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class DeudaController : ControllerBase
    {

        private readonly ILogger<FacturacionController> Logger;

        public DeudaController(ILogger<FacturacionController> logger, DeudaRepository repository, IConfiguration configuration)
        {
            Logger = logger;
            Repository = repository;
            Configuration = configuration;
        }

        public DeudaRepository Repository { get; }
        public IConfiguration Configuration { get; }

     

        [HttpGet]
        
        public async Task<ActionResult<List<DeudaDTO>>> GetDeuda(string? fechaDesde, string? fechaHasta)
        {

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                string? numeroDocumento = claims.FirstOrDefault(c => c.Type == "numeroDocumento").Value;
                string businessUnit = claims.FirstOrDefault(c => c.Type == "businessUnit").Value;


                if (numeroDocumento is null)
                {
                    numeroDocumento = "";
                }
                List<DeudaDTO?> Deuda = await Repository.GetDeuda(numeroDocumento, businessUnit, fechaDesde, fechaHasta);

                if (Deuda.Count() > 0)
                {
                    return Ok(Deuda);
                }

                return NotFound(new ComprobanteResponse(new ComprobanteDTO(numeroDocumento, "404", "Deuda inexistente", $"No se encontró Deuda con el numero de documento {numeroDocumento}.", null)));
            }
            return Unauthorized();

        }
    }
}