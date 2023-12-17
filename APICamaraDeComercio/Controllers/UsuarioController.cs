using APICamaraDeComercio.Models.Clientes;
using APICamaraDeComercio.Models.Facturacion;
using APICamaraDeComercio.Models.Response;
using APICamaraDeComercio.Models.Response.Usuario;
using APICamaraDeComercio.Models.Usuario;
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
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {

        private readonly ILogger<UsuarioController> Logger;

        public UsuarioController(ILogger<UsuarioController> logger, UsuarioRepository repository, IConfiguration configuration)
        {
            Logger = logger;
            Repository = repository;
            Configuration = configuration;
        }

        public UsuarioRepository Repository { get; }
        public IConfiguration Configuration { get; }

        [HttpPut]
        [Route("{id}/password/recover")]
        public async Task<ActionResult<RecoverPasswordResponse>> RecoverPassword(string id,[FromBody] RecoverPasswordDTO payload)
        {

            if (payload.newPassword.Length > 120)
            {
                return BadRequest(new RecoverPasswordResponse(new RecoverPasswordDTO { registrosActualizados = 0,mensaje = "La contraseña no puede tener mas de 120 caracteres."}));
            }
            
            RecoverPasswordDTO response = await Repository.RecoverPassword(id, payload.newPassword);
            
            response.newPassword = null;
            
            if (response.registrosActualizados>0)
            {
                return Ok(new RecoverPasswordResponse(response));
            }
            else
            {
                return NotFound(new RecoverPasswordResponse(response));
            }
        }

    }
}