using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.ConteudoHome;
using ProgressoAcademico.Services.ConteudoHome;
using System.Security.Claims;

namespace ProgressoAcademico.Controllers;

[Authorize(Roles = PerfisAcesso.Administrador)]
public class AdminConteudoHomeController : Controller
{
    private readonly IConteudoHomeService _service;

    public AdminConteudoHomeController(IConteudoHomeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Editar()
    {
        var model = await _service.ObterParaEdicaoAsync();
        return model == null ? NotFound() : View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(ConteudoHomeEdicaoViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var atualizado = await _service.AtualizarAsync(
            model,
            int.TryParse(usuarioId, out var id) ? id : null);

        if (!atualizado)
            return NotFound();

        TempData["Mensagem"] = "Apresentacao da Home atualizada com sucesso.";
        return RedirectToAction("Index", "AdminComunicados");
    }
}
