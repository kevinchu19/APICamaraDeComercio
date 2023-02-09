using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Services;
using Microsoft.AspNetCore.Mvc;

namespace APICamaraDeComercio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturacionController : ControllerBase
    {

        private readonly ILogger<FacturacionController> _logger;

        public FacturacionController(ILogger<FacturacionController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse>> PostFacturacion(object comprobante)
        {
            FieldMapper mapping = new FieldMapper();
            mapping.LoadMappingFile(AppDomain.CurrentDomain.BaseDirectory + @"\Services\FieldMapFiles\FacturacionController.json");
            return Ok(new BaseResponse());
        }
    }
}