using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressoAcademico.Models;
using System.Security.Claims;

namespace ProgressoAcademico.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            nome = User.FindFirstValue(ClaimTypes.Name),
            email = User.FindFirstValue(ClaimTypes.Email),
            perfilAcesso = User.FindFirstValue(ClaimTypes.Role)
        });
    }

    [Authorize(Roles = PerfisAcesso.Professor)]
    [HttpGet("professor-check")]
    public IActionResult ProfessorCheck()
    {
        return Ok(new { mensagem = "Acesso de professor autorizado." });
    }

    [Authorize(Roles = PerfisAcesso.Administrador)]
    [HttpGet("admin-check")]
    public IActionResult AdminCheck()
    {
        return Ok(new { mensagem = "Acesso de administrador autorizado." });
    }
}
