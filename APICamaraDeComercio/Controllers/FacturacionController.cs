using APICamaraDeComercio.Models.Facturacion;
using APICamaraDeComercio.Models.Response;
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
    public class FacturacionController : ControllerBase
    {

        private readonly ILogger<FacturacionController> Logger;

        public FacturacionController(ILogger<FacturacionController> logger, FacturacionRepository repository, IConfiguration configuration)
        {
            Logger = logger;
            Repository = repository;
            Configuration = configuration;
        }

        public FacturacionRepository Repository { get; }
        public IConfiguration Configuration { get; }

        [HttpPost]
        public async Task<ActionResult<ComprobanteResponse>> PostFacturacion([FromBody] FacturacionDTO comprobante)
        {

            FieldMapper mapping = new FieldMapper();
            if (!mapping.LoadMappingFile(AppDomain.CurrentDomain.BaseDirectory + @"\Services\FieldMapFiles\Facturacion.json"))
            { return BadRequest(new ComprobanteDTO((string?)comprobante.GetType()
                .GetProperty("identificador")
                .GetValue(comprobante), "400", "Error de configuracion", "No se encontro el archivo de configuracion del endpoing", null)); };

            string errorMessage = await Repository.ExecuteSqlInsertToTablaSAR(mapping.fieldMap,
                                                                              comprobante,
                                                                              comprobante.identificador,
                                                                              Configuration["Facturacion:JobName"]);
            if (errorMessage!= "")
            {
                return BadRequest(new ComprobanteResponse(new ComprobanteDTO(comprobante.identificador, "400", "Bad Request",errorMessage, null)));
            };
            
            
            
            return Ok(new ComprobanteResponse(new ComprobanteDTO(comprobante.identificador, "200", "OK", errorMessage, null)));
        }

        [HttpGet]
        [Route("{identificador}")]
        public async Task<ActionResult<ComprobanteResponse>> GetFacturacion(string identificador)
        {
            ComprobanteResponse respuesta = await Repository.GetTransaccion(identificador,"SAR_FCRMVH");

            switch (respuesta.response.status)
            {
                case "404":
                    return NotFound(respuesta);
                    break;
                default:
                    return Ok(respuesta);
                    break;
            }

        }
    }
}