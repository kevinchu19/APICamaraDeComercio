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
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.FlowAnalysis;
using APICamaraDeComercio.Services.ApiKey;

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
        [Authorize]
        [Route("file")]
        public async Task<ActionResult<PdfResponse>> GetFiles(string codigoComprobante, string numeroComprobante, bool pidePdf = true)
        {

            int numeroFormulario = 0;

            bool isNumber = Int32.TryParse(numeroComprobante, out numeroFormulario);

            if (!isNumber)
            {
                return BadRequest(new PdfResponse(new PdfDTO("El parámetro numeroComprobante debe ser numérico")));
            }

            PdfDTO? result = await Repository.GetPdfPath(codigoComprobante, numeroFormulario );



            if (result == null)
            {
                return NotFound(new PdfResponse(new PdfDTO("El comprobante solicitado no existe")));
            }

            if (pidePdf) {
                if (result.pdf == "")
                {
                    return NotFound(new PdfResponse(new PdfDTO("El comprobante solicitado no tiene pdf generado")));
                }

                try
                {
                    byte[] bytes = await System.IO.File.ReadAllBytesAsync(result.pdf);
                    result.pdf = Convert.ToBase64String(bytes);
                    return Ok(new PdfResponse(result));
                }
                catch (Exception ex)
                {

                    return NotFound(new PdfResponse(new PdfDTO("No se encontro el archivo pdf asociado al comprobante solicitado")));
                }
            }

            result.pdf = null;

            return Ok(new PdfResponse(result));
        }

        [HttpGet]
        [ApiKey]
        [Route("file/untoken")]
        public async Task<ActionResult<PdfResponse>> GetFilesUntoken(string codigoComprobante, string numeroComprobante, bool pidePdf = true)
        {

            return await GetFiles(codigoComprobante, numeroComprobante, pidePdf);
        }

    }
}