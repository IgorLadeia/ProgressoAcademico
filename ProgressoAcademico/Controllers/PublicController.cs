using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressoAcademico.Models.ViewModels.Publico;
using ProgressoAcademico.Services.Comunicados;
using ProgressoAcademico.Services.ConteudoHome;

namespace ProgressoAcademico.Controllers;

[AllowAnonymous]
public class PublicController : Controller
{
    private readonly IComunicadoService _comunicadoService;
    private readonly IConteudoHomeService _conteudoHomeService;

    public PublicController(
        IComunicadoService comunicadoService,
        IConteudoHomeService conteudoHomeService)
    {
        _comunicadoService = comunicadoService;
        _conteudoHomeService = conteudoHomeService;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["ActivePage"] = "Home";
        var model = new HomePublicaViewModel
        {
            Conteudo = await _conteudoHomeService.ObterAsync(),
            Comunicados = await _comunicadoService.ListarPublicadosAsync()
        };

        return View(model);
    }

    public IActionResult Projeto()
    {
        ViewData["ActivePage"] = "Projeto";
        return View();
    }

    // Mantem compatibilidade com links antigos enquanto o conteudo publico e consolidado.
    public IActionResult Sobre()
    {
        return RedirectToAction(nameof(Projeto));
    }

    public IActionResult Contato()
    {
        return RedirectToAction(nameof(Projeto), "Public", null, "contato");
    }

    public IActionResult Informacoes()
    {
        return RedirectToAction(nameof(Index), "Public", null, "comunicados");
    }

    public IActionResult Referencias()
    {
        return RedirectToAction(nameof(Projeto), "Public", null, "referencias");
    }
}
