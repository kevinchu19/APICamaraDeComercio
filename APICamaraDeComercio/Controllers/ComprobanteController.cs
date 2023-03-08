using APICamaraDeComercio.Models.Facturacion;
using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Models.Response.Pdf;
using APICamaraDeComercio.Repositories;
using APICamaraDeComercio.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Reflection;
using System.IO;

namespace APICamaraDeComercio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprobanteController : ControllerBase
    {

        private readonly ILogger<ComprobanteController> Logger;

        
        public ComprobanteRepository Repository { get; }

        public ComprobanteController(ILogger<ComprobanteController> logger,  ComprobanteRepository repository)
        {
            Logger = logger;
            Repository = repository;
        }

        [HttpGet]
        [Route("file")]
        public async Task<ActionResult<PdfResponse>> GetFiles(string codigoComprobante, string numeroComprobante)
        {
            int numeroFormulario = 0;

            bool isNumber = Int32.TryParse(numeroComprobante, out numeroFormulario);

            if (!isNumber)
            {
                return BadRequest(new PdfResponse(new PdfDTO(null, "El parámetro numeroComprobante debe ser numérico")));
            }

            string path = await Repository.GetPdfPath(codigoComprobante, numeroFormulario );

            if (path =="" )
            {
                return NotFound(new PdfResponse(new PdfDTO(null, "El comprobante solicitado no tiene pdf generado")));
            }

            try
            {
                byte[] bytes = await System.IO.File.ReadAllBytesAsync(path);
                string file = Convert.ToBase64String(bytes);
                return Ok(new PdfResponse(new PdfDTO(file)));
            }
            catch (Exception ex)
            {

                return NotFound(new PdfResponse(new PdfDTO(null, "No se encontro el archivo pdf asociado al comprobante solicitado")));
            }
        }

    }
}