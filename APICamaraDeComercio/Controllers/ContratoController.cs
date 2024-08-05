
using APICamaraDeComercio.Models.Contratos;
using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Repositories;
using APICamaraDeComercio.Services;
using APICamaraDeComercio.Services.ApiKey;
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
    public class ContratoController : ControllerBase
    {

        private readonly ILogger<ContratoController> Logger;

        public ContratoController(ILogger<ContratoController> logger, ContratoRepository repository, IConfiguration configuration)
        {
            Logger = logger;
            Repository = repository;
            Configuration = configuration;
        }

        public ContratoRepository Repository { get; }
        public IConfiguration Configuration { get; }

        [HttpPost]
        public async Task<ActionResult<ComprobanteResponse>> PostContrato([FromBody] ContratoDTO comprobante)
        {

            FieldMapper mapping = new FieldMapper();
            if (!mapping.LoadMappingFile(AppDomain.CurrentDomain.BaseDirectory + @"\Services\FieldMapFiles\Contrato.json"))
            { return BadRequest(new ComprobanteDTO((string?)comprobante.GetType()
                .GetProperty("identificador")
                .GetValue(comprobante), "400", "Error de configuracion", "No se encontro el archivo de configuracion del endpoint", null)); };

            string errorMessage = await Repository.ExecuteSqlInsertToTablaSAR(mapping.fieldMap,
                                                                              comprobante,
                                                                              comprobante.identificador,
                                                                              Configuration["Contrato:JobName"]);
            if (errorMessage!= "")
            {
                return BadRequest(new ComprobanteResponse(new ComprobanteDTO(comprobante.identificador, "400", "Bad Request",errorMessage, null)));
            };
            
            
            
            return Ok(new ComprobanteResponse(new ComprobanteDTO(comprobante.identificador, "200", "OK", errorMessage, null)));
        }

        [HttpPost]
        [Route("baja")]
        public async Task<ActionResult<ComprobanteResponse>> PostBajaContrato([FromBody] ContratoBajaDTO comprobante)
        {

            FieldMapper mapping = new FieldMapper();
            if (!mapping.LoadMappingFile(AppDomain.CurrentDomain.BaseDirectory + @"\Services\FieldMapFiles\BajaContrato.json"))
            {
                return BadRequest(new ComprobanteDTO((string?)comprobante.GetType()
                .GetProperty("identificador")
                .GetValue(comprobante), "400", "Error de configuracion", "No se encontro el archivo de configuracion del endpoint", null));
            };

            string errorMessage = await Repository.ExecuteSqlInsertToTablaSAR(mapping.fieldMap,
                                                                              comprobante,
                                                                              comprobante.identificador,
                                                                              Configuration["Contrato:JobName"]);
            if (errorMessage != "")
            {
                return BadRequest(new ComprobanteResponse(new ComprobanteDTO(comprobante.identificador, "400", "Bad Request", errorMessage, null)));
            };



            return Ok(new ComprobanteResponse(new ComprobanteDTO(comprobante.identificador, "200", "OK", errorMessage, null)));
        }


        [HttpGet]
        [Route("transaccion/{identificador}")]
        public async Task<ActionResult<ComprobanteResponse>> GetContrato(string identificador)
        {
            ComprobanteResponse respuesta = await Repository.GetTransaccion(identificador,"SAR_CVMCTH");

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