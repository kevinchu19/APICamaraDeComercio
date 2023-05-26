using APICamaraDeComercio.Models.Clientes;
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
    public class ClienteController : ControllerBase
    {

        private readonly ILogger<FacturacionController> Logger;

        public ClienteController(ILogger<FacturacionController> logger, ClienteRepository repository, IConfiguration configuration)
        {
            Logger = logger;
            Repository = repository;
            Configuration = configuration;
        }

        public ClienteRepository Repository { get; }
        public IConfiguration Configuration { get; }

        [HttpPost]
        public async Task<ActionResult<ComprobanteResponse>> PostCliente([FromBody] ClienteDTO comprobante)
        {

            FieldMapper mapping = new FieldMapper();
            if (!mapping.LoadMappingFile(AppDomain.CurrentDomain.BaseDirectory + @"\Services\FieldMapFiles\Cliente.json"))
            { return BadRequest(new ComprobanteDTO((string?)comprobante.GetType()
                                                           .GetProperty("identificador")
                                                           .GetValue(comprobante), "400", "Error de configuracion", "No se encontro el archivo de configuracion del endpoint", null)); };

            string errorMessage = await Repository.ExecuteSqlInsertToTablaSAR(mapping.fieldMap, 
                                                                              comprobante, 
                                                                              comprobante.identificador,
                                                                              Configuration["Clientes:JobName"]);
            if (errorMessage!= "")
            {
                return BadRequest(new ComprobanteResponse(new ComprobanteDTO(comprobante.identificador, "400", "Bad Request",errorMessage, null)));
            };
            
            
            
            return Ok(new ComprobanteResponse(new ComprobanteDTO(comprobante.identificador, "200", "OK", errorMessage, null)));
        }

        [HttpGet]
        [Route("{identificador}")]
        public async Task<ActionResult<ComprobanteResponse>> GetClienteTransaccion(string identificador)
        {
            ComprobanteResponse respuesta = await Repository.GetTransaccion(identificador,"SAR_VTMCLH");

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

        [HttpGet]
        
        public async Task<ActionResult<ClienteDTO>> GetCliente(string numeroDocumento)
        {
            ClienteDTO? cliente = await Repository.GetCliente(numeroDocumento);

            if (cliente is not null)
            {
                return Ok(cliente);
            }
            
            return NotFound(new ComprobanteResponse(new ComprobanteDTO(numeroDocumento, "404", "Cliente inexistente", $"No se encontró cliente con el numero de documento {numeroDocumento}.", null)));

        }
    }
}