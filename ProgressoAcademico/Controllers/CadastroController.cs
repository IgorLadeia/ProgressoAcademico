using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressoAcademico.Models.ViewModels.Cadastro;
using ProgressoAcademico.Services.Usuarios;

namespace ProgressoAcademico.Controllers;

public class CadastroController : Controller
{
    private readonly IUsuarioService _usuarioService;
    private readonly IDadosAcademicosService _dadosAcademicosService;

    public CadastroController(IUsuarioService usuarioService, IDadosAcademicosService dadosAcademicosService)
    {
        _usuarioService = usuarioService;
        _dadosAcademicosService = dadosAcademicosService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = new CadastroViewModel();
        await _dadosAcademicosService.PreencherOpcoesCadastroAsync(model);
        return View("Index", model);
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CadastroViewModel model)
    {
        ValidarComplemento(model);

        if (!ModelState.IsValid)
        {
            await _dadosAcademicosService.PreencherOpcoesCadastroAsync(model);
            return View("Index", model);
        }

        var usuarioCriado = await _usuarioService.CriarProfessorCadastroAsync(model);

        if (usuarioCriado == null)
        {
            ModelState.AddModelError(nameof(model.Email), "Ja existe um usuario cadastrado com este email.");
            await _dadosAcademicosService.PreencherOpcoesCadastroAsync(model);
            return View("Index", model);
        }

        TempData["Mensagem"] = "Cadastro criado com sucesso. Entre com seu email e senha.";
        return RedirectToAction("Login", "Account");
    }

    [AllowAnonymous]
    [HttpGet("/Cadastro")]
    public Task<IActionResult> Cadastro()
    {
        return Index();
    }

    [AllowAnonymous]
    [HttpPost("/Cadastro")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Cadastro(CadastroViewModel model)
    {
        return Index(model);
    }

    private void ValidarComplemento(CadastroViewModel model)
    {
        if (model.ModoCadastro != CadastroModos.Completo)
            return;

        if (!model.InstituicaoId.HasValue)
            ModelState.AddModelError(nameof(model.InstituicaoId), "Selecione a instituicao.");

        if (!model.TipoVinculoId.HasValue)
            ModelState.AddModelError(nameof(model.TipoVinculoId), "Selecione o tipo de vinculo.");

        if (!model.NivelId.HasValue)
            ModelState.AddModelError(nameof(model.NivelId), "Selecione o nivel atual.");

        if (!model.DataIngressoInstituicao.HasValue)
            ModelState.AddModelError(nameof(model.DataIngressoInstituicao), "Informe a data de ingresso.");
    }
}
