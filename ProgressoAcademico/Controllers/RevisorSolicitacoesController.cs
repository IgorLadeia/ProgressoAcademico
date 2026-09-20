using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.Admin;
using ProgressoAcademico.Services.Revisor;
using System.Security.Claims;

namespace ProgressoAcademico.Controllers;

[Authorize(Roles = PerfisAcesso.Revisor)]
public class RevisorSolicitacoesController : Controller
{
    private readonly IRevisorSolicitacaoService _revisorService;

    public RevisorSolicitacoesController(IRevisorSolicitacaoService revisorService)
    {
        _revisorService = revisorService;
    }

    [HttpGet("/Revisor")]
    public IActionResult Revisor()
    {
        return RedirectToAction(nameof(Dashboard));
    }

    public async Task<IActionResult> Dashboard()
    {
        var revisorId = ObterUsuarioId();

        if (revisorId == null)
            return Unauthorized();

        return View(await _revisorService.ObterDashboardAsync(revisorId.Value));
    }

    public async Task<IActionResult> Index()
    {
        var revisorId = ObterUsuarioId();

        if (revisorId == null)
            return Unauthorized();

        return View(await _revisorService.ListarAsync(revisorId.Value));
    }

    public async Task<IActionResult> Detalhes(int id)
    {
        var revisorId = ObterUsuarioId();

        if (revisorId == null)
            return Unauthorized();

        var detalhe = await _revisorService.ObterDetalheAsync(id, revisorId.Value);

        if (detalhe == null)
            return NotFound();

        return View(detalhe);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AvaliarAtividade(AdminAvaliarAtividadeViewModel model)
    {
        var revisorId = ObterUsuarioId();

        if (revisorId == null)
            return Unauthorized();

        if (!ModelState.IsValid)
        {
            TempData["Erro"] = "Selecione o resultado e informe o parecer da atividade.";
            return RedirectToAction(nameof(Detalhes), new { id = model.SolicitacaoProgressaoId });
        }

        var resultado = await _revisorService.AvaliarAtividadeAsync(model, revisorId.Value);
        if (resultado.Sucesso)
        {
            TempData["Mensagem"] = "Avaliacao da atividade registrada.";
            TempData["AtividadeAvaliadaId"] = model.AtividadeId;
        }
        else
        {
            TempData["Erro"] = resultado.Erro;
        }

        var url = Url.Action(nameof(Detalhes), new { id = model.SolicitacaoProgressaoId }) ?? $"/RevisorSolicitacoes/Detalhes/{model.SolicitacaoProgressaoId}";
        return Redirect($"{url}#atividade-review-{model.AtividadeId}");
    }

    public async Task<IActionResult> DownloadDocumento(int id)
    {
        var revisorId = ObterUsuarioId();

        if (revisorId == null)
            return Unauthorized();

        var documento = await _revisorService.ObterDocumentoAsync(id, revisorId.Value);
        return documento == null
            ? NotFound()
            : File(documento.Arquivo, documento.ContentType, documento.NomeArquivo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DevolverProfessor(AdminDecisaoSolicitacaoViewModel model)
    {
        var revisorId = ObterUsuarioId();

        if (revisorId == null)
            return Unauthorized();

        if (!ModelState.IsValid)
        {
            TempData["Erro"] = "Informe o parecer da revisao.";
            return RedirectToAction(nameof(Detalhes), new { id = model.SolicitacaoProgressaoId });
        }

        var resultado = await _revisorService.DevolverParaAjustesAsync(model.SolicitacaoProgressaoId, revisorId.Value, model.Parecer);
        TempData[resultado.Sucesso ? "Mensagem" : "Erro"] = resultado.Sucesso
            ? "Solicitacao devolvida ao professor para ajustes."
            : resultado.Erro;

        return RedirectToAction(nameof(Detalhes), new { id = model.SolicitacaoProgressaoId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EncaminharAdm(AdminDecisaoSolicitacaoViewModel model)
    {
        var revisorId = ObterUsuarioId();

        if (revisorId == null)
            return Unauthorized();

        if (!ModelState.IsValid)
        {
            TempData["Erro"] = "Informe o parecer da revisao.";
            return RedirectToAction(nameof(Detalhes), new { id = model.SolicitacaoProgressaoId });
        }

        var resultado = await _revisorService.EncaminharParaAdmAsync(model.SolicitacaoProgressaoId, revisorId.Value, model.Parecer);
        TempData[resultado.Sucesso ? "Mensagem" : "Erro"] = resultado.Sucesso
            ? "Solicitacao encaminhada ao ADM para decisao final."
            : resultado.Erro;

        return RedirectToAction(nameof(Detalhes), new { id = model.SolicitacaoProgressaoId });
    }

    private int? ObterUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(claim?.Value, out var usuarioId) ? usuarioId : null;
    }
}
