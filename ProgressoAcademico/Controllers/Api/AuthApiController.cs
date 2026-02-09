using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgressoAcademico.Context;
using ProgressoAcademico.ViewModels;

namespace ProgressoAcademico.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase // 👈 ControllerBase, não Controller
    {
        private readonly ProgressoAcademicoDbContext _context;

        public AuthController(ProgressoAcademicoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Autentica um usuário e valida email e senha
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = _context.Usuarios
                .Include(u => u.UsuarioPerfil)
                .Include(u => u.VinculoInstitucional)
                .FirstOrDefault(u =>
                    u.Email == model.Email &&
                    u.Ativo &&
                    u.DataExclusao == null
                );

            if (usuario == null)
            {
                return Unauthorized(new
                {
                    mensagem = "Usuário não encontrado ou inativo"
                });
            }

            bool senhaValida = BCrypt.Net.BCrypt.Verify(
                model.Senha,
                usuario.SenhaHash
            );

            if (!senhaValida)
            {
                return Unauthorized(new
                {
                    mensagem = "Senha inválida"
                });
            }

            // 🚀 Por enquanto retorno simples (depois entra JWT)
            return Ok(new
            {
                mensagem = "Login realizado com sucesso",
                usuario = new
                {
                    usuario.UsuarioId,
                    usuario.Nome,
                    usuario.Email
                }
            });
        }
    }
}
