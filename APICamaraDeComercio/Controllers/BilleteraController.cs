using APICamaraDeComercio.Models.Facturacion;
using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Models.Response.Billetera;
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
    public class BilleteraController : ControllerBase
    {

        private readonly ILogger<FacturacionController> Logger;

        public BilleteraController(ILogger<FacturacionController> logger, BilleteraRepository repository, IConfiguration configuration)
        {
            Logger = logger;
            Repository = repository;
            Configuration = configuration;
        }

        public BilleteraRepository Repository { get; }
        public IConfiguration Configuration { get; }

     

        [HttpGet]
        
        public async Task<ActionResult<List<BilleteraDTO>>> GetBilletera(string? fechaDesde, string? fechaHasta)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                string? numeroDocumento = claims.FirstOrDefault(c => c.Type == "numeroDocumento").Value;
                string? tipoDocumento = claims.FirstOrDefault(c => c.Type == "tipoDocumento").Value;
                string businessUnit = claims.FirstOrDefault(c => c.Type == "businessUnit").Value;

                if (numeroDocumento is null)
                {
                    numeroDocumento = "";
                }
                List<BilleteraDTO?> Billetera = await Repository.GetBilletera(numeroDocumento, fechaDesde, fechaHasta, businessUnit, tipoDocumento);

                if (Billetera.Count() > 0)
                {
                    return Ok(Billetera);
                }

                return NotFound(new ComprobanteResponse(new ComprobanteDTO(numeroDocumento, "404", "Billetera inexistente", $"No se encontraron registros de billetera para el documento tipo {tipoDocumento} - número {numeroDocumento}.", null)));
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("saldo")]
        public async Task<ActionResult<SaldoBilleteraDTO>> GetSaldoBilletera()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                string? numeroDocumento = claims.FirstOrDefault(c => c.Type == "numeroDocumento").Value;
                string? tipoDocumento = claims.FirstOrDefault(c => c.Type == "tipoDocumento").Value;
                string businessUnit = claims.FirstOrDefault(c => c.Type == "businessUnit").Value;

                if (numeroDocumento is null)
                {
                    numeroDocumento = "";
                }
                SaldoBilleteraDTO? Billetera = await Repository.GetSaldoBilletera(numeroDocumento, businessUnit, tipoDocumento);

                return Ok(Billetera);
            }
            return Unauthorized();

        }

        [HttpGet]
        [Route("montossugeridos")]

        public async Task<ActionResult<List<decimal>>> GetMontosSugeridosBilletera()
        {
            return Ok(await Repository.GetMontosSugeridosBilletera());
        }
    }
}