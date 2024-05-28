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
using System.Security.Claims;

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

        public async Task<ActionResult<List<VEPDTO>>> GetVEP(string? fechaDesde, string? fechaHasta)
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
                List<VEPDTO?> VEP = await Repository.GetVEPList(numeroDocumento, fechaDesde, fechaHasta, businessUnit);

                if (VEP.Count() > 0)
                {
                    return Ok(VEP);
                }

                return NotFound(new ComprobanteResponse(new ComprobanteDTO(numeroDocumento, "404", "VEP inexistentes", $"No se encontraron VEP para el numero de documento {numeroDocumento}.", null)));
            }
            return Unauthorized();
        }

        [HttpPost]
        public async Task<ActionResult<VEPResponse>> PostVEP([FromBody] VEPDTO vep)
        {
            VEPDTO response = new VEPDTO();
            string ErrorMessage = "";

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                string? numeroDocumento = claims.FirstOrDefault(c => c.Type == "numeroDocumento").Value;
                string businessUnit = claims.FirstOrDefault(c => c.Type == "businessUnit").Value;


                if (vep.numeroDocumento != numeroDocumento)
                {
                    return BadRequest(new VEPResponse(new VEPDTO("El numero de documento asignado al VEP no coincide con las credenciales del usuario.")));
                }

                if (vep.comprobantes.Count() > 0)
                {
                    if (vep.importe != vep.comprobantes.Select(v => v.importe).Sum())
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
            return Unauthorized();
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<ActionResult<VEPResponse>> PatchVEP([FromBody] VEPDTO vep, int id)
        {
            VEPDTO response = new VEPDTO();
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            vep.numeroVEP = id;

            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                string? numeroDocumentoToken = claims.FirstOrDefault(c => c.Type == "numeroDocumento").Value;
                VEPDTO? vepRecuperado = await Repository.GetVEP(id);
                if (vepRecuperado is null)
                {
                    return BadRequest(new VEPResponse(new VEPDTO("El VEP no existe.")));
                }

                if (numeroDocumentoToken != vepRecuperado.numeroDocumento)
                {
                    return BadRequest(new VEPResponse(new VEPDTO("El numero de documento del VEP a actualizar no coincide con el de las credenciales del usuario.")));
                }
            }

            response = await Repository.PatchVEP(vep);

            return Ok(new VEPResponse(response));
        }
    }
}