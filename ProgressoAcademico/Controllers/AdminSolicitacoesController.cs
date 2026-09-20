using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressoAcademico.Application.DTOs.Admin;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.Admin;
using ProgressoAcademico.Services.Admin;
using System.Security.Claims;

namespace ProgressoAcademico.Controllers;

[Authorize(Roles = PerfisAcesso.Administrador)]
public class AdminSolicitacoesController : Controller
{
    private readonly IAdminSolicitacaoService _adminSolicitacaoService;

    public AdminSolicitacoesController(IAdminSolicitacaoService adminSolicitacaoService)
    {
        _adminSolicitacaoService = adminSolicitacaoService;
    }

    [HttpGet("/Admin")]
    public IActionResult Admin()
    {
        return RedirectToAction(nameof(Dashboard));
    }

    public async Task<IActionResult> Dashboard()
    {
        var dashboard = await _adminSolicitacaoService.ObterDashboardAsync();
        return View(dashboard);
    }

    public async Task<IActionResult> Index(AdminSolicitacaoFiltroViewModel filtro)
    {
        await _adminSolicitacaoService.PreencherFiltrosAsync(filtro);

        var solicitacoes = await _adminSolicitacaoService.ListarAsync(new AdminSolicitacaoFiltroDto
        {
            Status = filtro.Status,
            Professor = filtro.Professor,
            TipoProgressoId = filtro.TipoProgressoId,
            DataInicio = filtro.DataInicio,
            DataFim = filtro.DataFim,
            PercentualMinimo = filtro.PercentualMinimo,
            SomentePendentes = filtro.SomentePendentes
        });

        return View(new AdminSolicitacoesIndexViewModel
        {
            Filtro = filtro,
            Solicitacoes = solicitacoes
        });
    }

    public async Task<IActionResult> Detalhes(int id)
    {
        var detalhe = await _adminSolicitacaoService.ObterDetalheAsync(id);

        if (detalhe == null)
            return NotFound();

        return View(detalhe);
    }

    [HttpGet]
    public async Task<IActionResult> Corrigir(int id)
    {
        var model = await _adminSolicitacaoService.ObterParaCorrecaoAsync(id);

        if (model == null)
            return NotFound();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> AtribuirRevisor(int id)
    {
        var model = await _adminSolicitacaoService.ObterParaAtribuicaoAsync(id);

        if (model == null)
            return NotFound();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AtribuirRevisor(AdminAtribuirRevisorViewModel model)
    {
        var adminId = ObterUsuarioId();

        if (adminId == null)
            return Unauthorized();

        if (!ModelState.IsValid)
        {
            await _adminSolicitacaoService.PreencherAtribuicaoAsync(model);
            return View(model);
        }

        var resultado = await _adminSolicitacaoService.AtribuirRevisorAsync(model, adminId.Value);

        if (!resultado.Sucesso)
        {
            ModelState.AddModelError(string.Empty, resultado.Erro ?? "Nao foi possivel atribuir a solicitacao.");
            await _adminSolicitacaoService.PreencherAtribuicaoAsync(model);
            return View(model);
        }

        TempData["Mensagem"] = "Solicitacao atribuida ao revisor.";
        return RedirectToAction(nameof(Detalhes), new { id = model.SolicitacaoProgressaoId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Corrigir(AdminCorrigirSolicitacaoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await _adminSolicitacaoService.PreencherCorrecaoAsync(model);
            return View(model);
        }

        var resultado = await _adminSolicitacaoService.CorrigirAsync(model);

        if (!resultado.Sucesso)
        {
            ModelState.AddModelError(string.Empty, resultado.Erro ?? "Nao foi possivel corrigir a solicitacao.");
            await _adminSolicitacaoService.PreencherCorrecaoAsync(model);
            return View(model);
        }

        TempData["Mensagem"] = "Solicitacao corrigida com sucesso.";
        return RedirectToAction(nameof(Detalhes), new { id = model.SolicitacaoProgressaoId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AvaliarAtividade(AdminAvaliarAtividadeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Erro"] = "Selecione o resultado e informe o parecer da atividade.";
            return RedirectToAction(nameof(Detalhes), new { id = model.SolicitacaoProgressaoId });
        }

        var resultado = await _adminSolicitacaoService.AvaliarAtividadeAsync(model);
        TempData[resultado.Sucesso ? "Mensagem" : "Erro"] = resultado.Sucesso
            ? "Avaliação da atividade registrada."
            : resultado.Erro;
        return RedirectToAction(nameof(Detalhes), new { id = model.SolicitacaoProgressaoId });
    }

    public async Task<IActionResult> DownloadDocumento(int id)
    {
        var documento = await _adminSolicitacaoService.ObterDocumentoAsync(id);
        return documento == null
            ? NotFound()
            : File(documento.Arquivo, documento.ContentType, documento.NomeArquivo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Aprovar(AdminDecisaoSolicitacaoViewModel model)
    {
        return DecidirAsync(model, "aprovar");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Reprovar(AdminDecisaoSolicitacaoViewModel model)
    {
        return DecidirAsync(model, "reprovar");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> SolicitarAjustes(AdminDecisaoSolicitacaoViewModel model)
    {
        return DecidirAsync(model, "ajustes");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Encerrar(AdminDecisaoSolicitacaoViewModel model)
    {
        return DecidirAsync(model, "encerrar");
    }

    private async Task<IActionResult> DecidirAsync(AdminDecisaoSolicitacaoViewModel model, string acao)
    {
        var adminId = ObterUsuarioId();

        if (adminId == null)
            return Unauthorized();

        if (!ModelState.IsValid)
        {
            TempData["Erro"] = "Informe o parecer administrativo.";
            return RedirectToAction(nameof(Detalhes), new { id = model.SolicitacaoProgressaoId });
        }

        var resultado = acao switch
        {
            "aprovar" => await _adminSolicitacaoService.AprovarAsync(model.SolicitacaoProgressaoId, adminId.Value, model.Parecer),
            "reprovar" => await _adminSolicitacaoService.ReprovarAsync(model.SolicitacaoProgressaoId, adminId.Value, model.Parecer),
            "ajustes" => await _adminSolicitacaoService.SolicitarAjustesAsync(model.SolicitacaoProgressaoId, adminId.Value, model.Parecer),
            _ => await _adminSolicitacaoService.EncerrarAsync(model.SolicitacaoProgressaoId, adminId.Value, model.Parecer)
        };

        TempData[resultado.Sucesso ? "Mensagem" : "Erro"] = resultado.Sucesso
            ? "Decisao administrativa registrada."
            : resultado.Erro;

        return RedirectToAction(nameof(Detalhes), new { id = model.SolicitacaoProgressaoId });
    }

    private int? ObterUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(claim?.Value, out var usuarioId) ? usuarioId : null;
    }
}
