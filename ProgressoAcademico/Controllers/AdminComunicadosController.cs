using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.Comunicados;
using ProgressoAcademico.Services.Comunicados;
using System.Security.Claims;

namespace ProgressoAcademico.Controllers;

[Authorize(Roles = PerfisAcesso.Administrador)]
public class AdminComunicadosController : Controller
{
    private readonly IComunicadoService _comunicadoService;

    public AdminComunicadosController(IComunicadoService comunicadoService)
    {
        _comunicadoService = comunicadoService;
    }

    [HttpGet("/AdminComunicados")]
    public async Task<IActionResult> Index()
    {
        ViewData["ActivePage"] = "Admin";
        var comunicados = await _comunicadoService.ListarTodosAsync();
        return View(comunicados);
    }

    [HttpGet]
    public IActionResult Criar()
    {
        ViewData["ActivePage"] = "Admin";
        return View(new ComunicadoFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(ComunicadoFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _comunicadoService.CriarAsync(model, int.TryParse(usuarioId, out var id) ? id : null);

        TempData["Mensagem"] = "Comunicado publicado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var model = await _comunicadoService.ObterParaEdicaoAsync(id);

        if (model == null)
            return NotFound();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(ComunicadoFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (!await _comunicadoService.AtualizarAsync(model))
            return NotFound();

        TempData["Mensagem"] = "Conteudo da Home atualizado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(int id)
    {
        await _comunicadoService.ExcluirAsync(id);
        TempData["Mensagem"] = "Comunicado removido.";
        return RedirectToAction(nameof(Index));
    }
}
