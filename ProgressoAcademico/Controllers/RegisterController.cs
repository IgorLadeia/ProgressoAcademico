using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressoAcademico.Models.ViewModels.Register;
using ProgressoAcademico.Services.Usuarios;

namespace ProgressoAcademico.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public RegisterController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [AllowAnonymous]
        [HttpPost("registro-completo")]
        public async Task<IActionResult> CriarUsuarioCompleto([FromBody] RegisterViewModel dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuarioCriado = await _usuarioService.CriarProfessorAsync(dto);

            if (usuarioCriado == null)
                return BadRequest("Email ja cadastrado.");

            return Ok(new
            {
                mensagem = "Usuario criado com sucesso.",
                usuarioId = usuarioCriado.UsuarioId,
                perfilAcesso = usuarioCriado.PerfilAcesso
            });
        }
    }
}
