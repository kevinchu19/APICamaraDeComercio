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
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;

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
        [Authorize]
        [HttpPut]
        [Route("password/change")]
        public async Task<ActionResult<RecoverPasswordResponse>> ChangePassword( [FromBody] RecoverPasswordDTO payload)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {   
                IEnumerable < Claim > claims = identity.Claims;
                string id = claims.FirstOrDefault(c => c.Type == "id").Value;
               
                RecoverPasswordDTO response = await Repository.ChangePassword(id, payload.newPassword);

                response.newPassword = null;

                if (response.registrosActualizados > 0)
                {
                    return Ok(new RecoverPasswordResponse(response));
                }
                else
                {
                    return BadRequest(new RecoverPasswordResponse(response));
                }
            }
            return Unauthorized();
        }

        [Authorize]
        [HttpPut]
        [Route("terminosycondiciones")]
        public async Task<ActionResult<TerminosYCondicionesResponse>> TerminosYCondiciones([FromBody] TerminosYCondicionesDTO payload)
        {

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                string id = claims.FirstOrDefault(c => c.Type == "id").Value;
                TerminosYCondicionesDTO response = await Repository.UpdateTerminosYCondiciones(id, payload.terminosYCondiciones);
                return Ok(new TerminosYCondicionesResponse(response));
            }
            return Unauthorized();
        }

    }
}