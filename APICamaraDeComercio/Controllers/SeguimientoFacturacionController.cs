using APICamaraDeComercio.Models.Clientes;
using APICamaraDeComercio.Models.Facturacion;
using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Models.SeguimientoFacturacion;
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
    public class SeguimientoFacturacionController : ControllerBase
    {

        private readonly ILogger<SeguimientoFacturacionController> Logger;

        public SeguimientoFacturacionController(ILogger<SeguimientoFacturacionController> logger, SeguimientoFacturacionRepository repository, IConfiguration configuration)
        {
            Logger = logger;
            Repository = repository;
            Configuration = configuration;
        }

        public SeguimientoFacturacionRepository Repository { get; }
        public IConfiguration Configuration { get; }

        [HttpPost]
        public async Task<ActionResult<List<ComprobanteSeguimientoDTO>>> Post([FromBody] List<ComprobanteParams> listaComprobantes, DateTime? fchmov, string? circuitoOrigen)
        {

            var response = await Repository.GetComprobantesPrincipales(listaComprobantes, fchmov, circuitoOrigen);
            
            return Ok(response);
        }
    }
}