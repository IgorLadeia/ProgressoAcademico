using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.Documentos;
using ProgressoAcademico.Services.Documentos;
using ProgressoAcademico.Services.Solicitacoes;
using System.Security.Claims;

namespace ProgressoAcademico.Controllers;

[Authorize(Roles = PerfisAcesso.Professor)]
public class ProfessorDocumentosController : Controller
{
    private readonly IDocumentoService _documentoService;
    private readonly ISolicitacaoProgressaoService _solicitacaoService;

    public ProfessorDocumentosController(
        IDocumentoService documentoService,
        ISolicitacaoProgressaoService solicitacaoService)
    {
        _documentoService = documentoService;
        _solicitacaoService = solicitacaoService;
    }

    public async Task<IActionResult> Index(int solicitacaoId)
    {
        var usuarioId = ObterUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        var solicitacao = await _solicitacaoService.ObterDetalheDoProfessorAsync(usuarioId.Value, solicitacaoId);

        if (solicitacao == null)
            return NotFound();

        var upload = new DocumentoUploadViewModel { SolicitacaoProgressaoId = solicitacaoId };
        await _documentoService.PreencherOpcoesUploadAsync(upload);

        return View(new DocumentosSolicitacaoViewModel
        {
            Solicitacao = solicitacao,
            Upload = upload,
            Documentos = await _documentoService.ListarPorSolicitacaoAsync(usuarioId.Value, solicitacaoId)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(DocumentoUploadViewModel model)
    {
        var usuarioId = ObterUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        var arquivos = model.Arquivos.Where(a => a.Length > 0).ToList();
        if (model.Arquivo is { Length: > 0 })
            arquivos.Add(model.Arquivo);

        if (!ModelState.IsValid || arquivos.Count == 0)
        {
            TempData["Erro"] = arquivos.Count == 0 ? "Selecione ao menos um arquivo PDF." : "Revise os dados do upload.";
            if (model.AtividadeId.HasValue)
                return RedirectToAction("Index", "ProfessorAtividades", new { solicitacaoId = model.SolicitacaoProgressaoId, grupo = model.RetornoGrupo });

            return RedirectToAction(nameof(Index), new { solicitacaoId = model.SolicitacaoProgressaoId });
        }

        var falhas = new List<string>();
        foreach (var arquivo in arquivos)
        {
            model.Arquivo = arquivo;
            var resultado = await _documentoService.EnviarAsync(usuarioId.Value, model);
            if (!resultado.Sucesso)
                falhas.Add($"{arquivo.FileName}: {resultado.Erro}");
        }

        TempData[falhas.Count == 0 ? "Mensagem" : "Erro"] =
            falhas.Count == 0 ? "Documento(s) anexado(s) com sucesso." : string.Join("; ", falhas);

        if (model.AtividadeId.HasValue)
            return RedirectToAction("Index", "ProfessorAtividades", new { solicitacaoId = model.SolicitacaoProgressaoId, grupo = model.RetornoGrupo });

        return RedirectToAction(nameof(Index), new { solicitacaoId = model.SolicitacaoProgressaoId });
    }

    public async Task<IActionResult> Download(int id)
    {
        var usuarioId = ObterUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        var documento = await _documentoService.ObterDownloadAsync(usuarioId.Value, id);

        if (documento == null)
            return NotFound();

        return File(documento.Arquivo, documento.ContentType, documento.NomeArquivo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(int id, int solicitacaoId, string? retornoGrupo)
    {
        var usuarioId = ObterUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        var removido = await _documentoService.ExcluirAsync(usuarioId.Value, id);
        TempData[removido ? "Mensagem" : "Erro"] = removido
            ? "Documento removido."
            : "Nao foi possivel remover o documento.";

        if (!string.IsNullOrWhiteSpace(retornoGrupo))
            return RedirectToAction("Index", "ProfessorAtividades", new { solicitacaoId, grupo = retornoGrupo });

        return RedirectToAction(nameof(Index), new { solicitacaoId });
    }

    private int? ObterUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(claim?.Value, out var usuarioId) ? usuarioId : null;
    }
}
