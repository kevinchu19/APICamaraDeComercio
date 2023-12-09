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
        
        public async Task<ActionResult<List<BilleteraDTO>>> GetBilletera(string? numeroDocumento, string? fechaDesde, string? fechaHasta, string businessUnit)
        {
            if (numeroDocumento is null)
            {
                numeroDocumento = "";
            }
            List<BilleteraDTO?> Billetera = await Repository.GetBilletera(numeroDocumento, fechaDesde, fechaHasta, businessUnit);

            if (Billetera.Count() > 0)
            {
                return Ok(Billetera);
            }
            
            return NotFound(new ComprobanteResponse(new ComprobanteDTO(numeroDocumento, "404", "Billetera inexistente", $"No se encontraron registros de billetera para el numero de documento {numeroDocumento}.", null)));

        }

        [HttpGet]
        [Route("saldo")]
        public async Task<ActionResult<SaldoBilleteraDTO>> GetSaldoBilletera(string? numeroDocumento, string businessUnit)
        {
            if (numeroDocumento is null)
            {
                numeroDocumento = "";
            }
            SaldoBilleteraDTO? Billetera = await Repository.GetSaldoBilletera(numeroDocumento, businessUnit);

            return Ok(Billetera);            

        }
    }
}