using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressoAcademico.Models;
using ProgressoAcademico.Services.Elegibilidade;
using System.Security.Claims;

namespace ProgressoAcademico.Controllers;

[Authorize(Roles = PerfisAcesso.Professor)]
public class ProfessorElegibilidadeController : Controller
{
    private readonly IElegibilidadeService _elegibilidadeService;

    public ProfessorElegibilidadeController(IElegibilidadeService elegibilidadeService)
    {
        _elegibilidadeService = elegibilidadeService;
    }

    public async Task<IActionResult> Index(int solicitacaoId)
    {
        var usuarioId = ObterUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        var resultado = await _elegibilidadeService.AvaliarAsync(usuarioId.Value, solicitacaoId);

        if (resultado == null)
            return NotFound();

        return View(resultado);
    }

    private int? ObterUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(claim?.Value, out var usuarioId) ? usuarioId : null;
    }
}
