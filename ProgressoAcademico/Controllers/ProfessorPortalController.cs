using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressoAcademico.Application.DTOs.Solicitacoes;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.ProfessorPortal;
using ProgressoAcademico.Services.Solicitacoes;
using ProgressoAcademico.Services.Usuarios;
using System.Security.Claims;

namespace ProgressoAcademico.Controllers;

[Authorize(Roles = PerfisAcesso.Professor)]
public class ProfessorPortalController : Controller
{
    private readonly ISolicitacaoProgressaoService _solicitacaoService;
    private readonly IDadosAcademicosService _dadosAcademicosService;

    public ProfessorPortalController(
        ISolicitacaoProgressaoService solicitacaoService,
        IDadosAcademicosService dadosAcademicosService)
    {
        _solicitacaoService = solicitacaoService;
        _dadosAcademicosService = dadosAcademicosService;
    }

    [HttpGet("/Professor")]
    public IActionResult Professor()
    {
        return RedirectToAction(nameof(Dashboard));
    }

    public async Task<IActionResult> Dashboard()
    {
        var usuarioId = ObterUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        var dashboard = await _solicitacaoService.ObterDashboardProfessorAsync(usuarioId.Value);

        return View(new ProfessorDashboardViewModel
        {
            NomeProfessor = User.Identity?.Name ?? "Professor",
            TotalSolicitacoes = dashboard.TotalSolicitacoes,
            EmProcessamento = dashboard.EmProcessamento,
            AjustesSolicitados = dashboard.AjustesSolicitados,
            Aprovadas = dashboard.Aprovadas,
            Negadas = dashboard.Negadas,
            Encerradas = dashboard.Encerradas,
            SolicitacoesRecentes = dashboard.SolicitacoesRecentes
        });
    }

    public async Task<IActionResult> Solicitacoes()
    {
        var usuarioId = ObterUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        var solicitacoes = await _solicitacaoService.ListarPorUsuarioAsync(usuarioId.Value);
        return View(solicitacoes);
    }

    public async Task<IActionResult> Detalhes(int id)
    {
        var usuarioId = ObterUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        var solicitacao = await _solicitacaoService.ObterDetalheDoProfessorAsync(usuarioId.Value, id);

        if (solicitacao == null)
            return NotFound();

        return View(solicitacao);
    }

    [HttpGet]
    public async Task<IActionResult> Criar()
    {
        var model = new CriarSolicitacaoViewModel();
        await _dadosAcademicosService.PreencherOpcoesSolicitacaoAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(CriarSolicitacaoViewModel model)
    {
        var usuarioId = ObterUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        if (!ModelState.IsValid)
        {
            await _dadosAcademicosService.PreencherOpcoesSolicitacaoAsync(model);
            return View(model);
        }

        var resultado = await _solicitacaoService.CriarAsync(usuarioId.Value, new CriarSolicitacaoDto
        {
            TipoProgressoId = model.TipoProgressoId!.Value,
            NivelOrigemId = model.NivelOrigemId!.Value,
            NivelDestinoId = model.NivelDestinoId!.Value,
            Apelido = model.Apelido,
            Observacao = model.Observacao
        });

        if (!resultado.Sucesso)
        {
            ModelState.AddModelError(string.Empty, resultado.Erro ?? "Nao foi possivel criar a solicitacao.");
            await _dadosAcademicosService.PreencherOpcoesSolicitacaoAsync(model);
            return View(model);
        }

        TempData["Mensagem"] = "Solicitacao criada com sucesso.";
        return RedirectToAction(nameof(Detalhes), new { id = resultado.SolicitacaoProgressaoId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submeter(int id)
    {
        var usuarioId = ObterUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        var resultado = await _solicitacaoService.SubmeterAsync(usuarioId.Value, id);

        if (!resultado.Sucesso)
        {
            TempData["Erro"] = resultado.Erro ?? "Nao foi possivel submeter a solicitacao.";
            return RedirectToAction(nameof(Detalhes), new { id });
        }

        TempData["Mensagem"] = "Solicitacao enviada para analise.";
        return RedirectToAction(nameof(Detalhes), new { id });
    }

    private int? ObterUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(claim?.Value, out var usuarioId) ? usuarioId : null;
    }
}
