using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.Admin;
using ProgressoAcademico.Services.Admin;
using System.Security.Claims;

namespace ProgressoAcademico.Controllers;

[Authorize(Roles = PerfisAcesso.Administrador)]
public class AdminUsuariosController : Controller
{
    private readonly IAdminUsuarioService _usuarioService;

    public AdminUsuariosController(IAdminUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    public async Task<IActionResult> Index()
    {
        var model = new AdminUsuariosIndexViewModel
        {
            Usuarios = await _usuarioService.ListarAsync(),
            Perfis = new[] { PerfisAcesso.Professor, PerfisAcesso.Administrador }
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarNivelAcesso(int usuarioId, string nivelAcesso)
    {
        var adminId = ObterUsuarioId();

        if (adminId == null)
            return Unauthorized();

        var resultado = await _usuarioService.AlterarNivelAcessoAsync(usuarioId, adminId.Value, nivelAcesso);
        TempData[resultado.Sucesso ? "Mensagem" : "Erro"] = resultado.Sucesso
            ? "Nivel de acesso atualizado."
            : resultado.Erro;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarPerfil(int usuarioId, string perfil)
    {
        var adminId = ObterUsuarioId();

        if (adminId == null)
            return Unauthorized();

        var resultado = await _usuarioService.AlterarPerfilAsync(usuarioId, adminId.Value, perfil);
        TempData[resultado.Sucesso ? "Mensagem" : "Erro"] = resultado.Sucesso
            ? "Perfil atualizado."
            : resultado.Erro;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DefinirPodeRevisar(int usuarioId, bool podeRevisar)
    {
        var adminId = ObterUsuarioId();

        if (adminId == null)
            return Unauthorized();

        var resultado = await _usuarioService.DefinirPodeRevisarAsync(usuarioId, adminId.Value, podeRevisar);
        TempData[resultado.Sucesso ? "Mensagem" : "Erro"] = resultado.Sucesso
            ? "Funcao de revisao atualizada."
            : resultado.Erro;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DefinirAtivo(int usuarioId, bool ativo)
    {
        var adminId = ObterUsuarioId();

        if (adminId == null)
            return Unauthorized();

        var resultado = await _usuarioService.DefinirAtivoAsync(usuarioId, adminId.Value, ativo);
        TempData[resultado.Sucesso ? "Mensagem" : "Erro"] = resultado.Sucesso
            ? "Acesso atualizado."
            : resultado.Erro;

        return RedirectToAction(nameof(Index));
    }

    private int? ObterUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(claim?.Value, out var usuarioId) ? usuarioId : null;
    }
}
