using APICamaraDeComercio.Models.Facturacion;
using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Repositories;
using APICamaraDeComercio.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Reflection;

namespace APICamaraDeComercio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturacionController : ControllerBase
    {

        private readonly ILogger<FacturacionController> Logger;

        public FacturacionController(ILogger<FacturacionController> logger, FacturacionRepository repository)
        {
            Logger = logger;
            Repository = repository;
        }

        public FacturacionRepository Repository { get; }

        [HttpPost]
        public async Task<ActionResult<BaseResponse>> PostFacturacion([FromBody] FacturacionDTO comprobante)
        {

            FieldMapper mapping = new FieldMapper();
            if (!mapping.LoadMappingFile(AppDomain.CurrentDomain.BaseDirectory + @"\Services\FieldMapFiles\Facturacion.json"))
            { return BadRequest(new ResponseDTO((string?)comprobante.GetType()
                                                           .GetProperty("identificador")
                                                           .GetValue(comprobante), "400", "Error de configuracion", "No se encontro el archivo de configuracion del endpoing", null)); };

            string errorMessage = await Repository.ExecuteSqlInsertToTablaSAR(mapping.fieldMap, comprobante, comprobante.identificador);
            if (errorMessage!= "")
            {
                return BadRequest(new BaseResponse(new ResponseDTO(comprobante.identificador, "400", "Bad Request",errorMessage, null)));
            };
            
            
            
            return Ok(new BaseResponse(new ResponseDTO(comprobante.identificador, "200", "OK", errorMessage, null)));
        }

        [HttpGet]
        [Route("{identificador}")]
        public async Task<ActionResult<BaseResponse>> GetFacturacion(string identificador)
        {
            BaseResponse respuesta = await Repository.GetTransaccion(identificador,"SAR_FCRMVH");

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