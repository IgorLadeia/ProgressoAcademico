using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.Atividades;
using ProgressoAcademico.Models.ViewModels.Atividades;
using ProgressoAcademico.Models.ViewModels.Documentos;
using ProgressoAcademico.Services.Atividades;
using ProgressoAcademico.Services.Documentos;
using ProgressoAcademico.Services.Solicitacoes;
using System.Security.Claims;

namespace ProgressoAcademico.Controllers;

[Authorize(Roles = PerfisAcesso.Professor)]
public class ProfessorAtividadesController : Controller
{
    private readonly IAtividadeService _atividadeService;
    private readonly ISolicitacaoProgressaoService _solicitacaoService;
    private readonly IDocumentoService _documentoService;

    public ProfessorAtividadesController(
        IAtividadeService atividadeService,
        ISolicitacaoProgressaoService solicitacaoService,
        IDocumentoService documentoService)
    {
        _atividadeService = atividadeService;
        _solicitacaoService = solicitacaoService;
        _documentoService = documentoService;
    }

    public async Task<IActionResult> Index(int solicitacaoId, string grupo = "Ensino")
    {
        var usuarioId = ObterUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        var solicitacao = await _solicitacaoService.ObterDetalheDoProfessorAsync(usuarioId.Value, solicitacaoId);

        if (solicitacao == null)
            return NotFound();

        var formulario = new AtividadeFormViewModel { SolicitacaoProgressaoId = solicitacaoId };
        await _atividadeService.PreencherOpcoesAsync(formulario);

        grupo = GruposAtividade.ObterValidoOuPadrao(grupo);
        formulario.SubtiposAtividade = formulario.SubtiposAtividade
            .Where(item => GruposAtividade.ItemSelecaoPertenceAoGrupo(item.Text, grupo))
            .ToList();

        var atividades = await _atividadeService.ListarPorSolicitacaoAsync(usuarioId.Value, solicitacaoId);
        return View(new AtividadesSolicitacaoViewModel
        {
            Solicitacao = solicitacao,
            Formulario = formulario,
            Atividades = atividades.Where(a => GruposAtividade.MesmoGrupo(a.TipoAtividade, grupo)).ToList(),
            Documentos = await _documentoService.ListarPorSolicitacaoAsync(usuarioId.Value, solicitacaoId),
            GrupoAtual = grupo
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(AtividadeFormViewModel model)
    {
        var usuarioId = ObterUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        if (!ModelState.IsValid)
        {
            TempData["Erro"] = "Revise os dados da atividade.";
            return RedirectToAction(nameof(Index), new { solicitacaoId = model.SolicitacaoProgressaoId, grupo = Request.Form["Grupo"].ToString() });
        }

        var resultado = await _atividadeService.CriarAsync(usuarioId.Value, model);
        if (!resultado.Sucesso)
        {
            TempData["Erro"] = resultado.Erro;
        }
        else if (model.Comprovantes.Count > 0 && resultado.AtividadeId.HasValue)
        {
            var falhas = new List<string>();
            foreach (var comprovante in model.Comprovantes.Where(c => c.Length > 0))
            {
                var upload = await _documentoService.EnviarAsync(usuarioId.Value, new DocumentoUploadViewModel
                {
                    SolicitacaoProgressaoId = model.SolicitacaoProgressaoId,
                    AtividadeId = resultado.AtividadeId.Value,
                    OrigemDocumento = OrigensDocumento.ComprovatorioProfessor,
                    Arquivo = comprovante
                });

                if (!upload.Sucesso)
                    falhas.Add($"{comprovante.FileName}: {upload.Erro}");
            }

            TempData[falhas.Count == 0 ? "Mensagem" : "Erro"] = falhas.Count == 0
                ? "Atividade e comprovantes cadastrados com sucesso."
                : $"A atividade foi cadastrada, mas alguns comprovantes nao foram anexados: {string.Join("; ", falhas)}";
        }
        else
        {
            TempData["Mensagem"] = "Atividade cadastrada. Voce ainda pode adicionar um ou mais comprovantes.";
        }

        return RedirectToAction(nameof(Index), new { solicitacaoId = model.SolicitacaoProgressaoId, grupo = Request.Form["Grupo"].ToString() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(AtividadeFormViewModel model)
    {
        var usuarioId = ObterUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        if (!ModelState.IsValid)
        {
            TempData["Erro"] = "Revise os dados da atividade antes de salvar a correcao.";
            return RedirectToAction(nameof(Index), new { solicitacaoId = model.SolicitacaoProgressaoId, grupo = Request.Form["Grupo"].ToString() });
        }

        var resultado = await _atividadeService.AtualizarAsync(usuarioId.Value, model);
        TempData[resultado.Sucesso ? "Mensagem" : "Erro"] = resultado.Sucesso
            ? "Atividade atualizada e reenviada para avaliacao."
            : resultado.Erro;

        return RedirectToAction(nameof(Index), new { solicitacaoId = model.SolicitacaoProgressaoId, grupo = Request.Form["Grupo"].ToString() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(int id, int solicitacaoId, string grupo)
    {
        var usuarioId = ObterUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        var removida = await _atividadeService.ExcluirAsync(usuarioId.Value, id);
        TempData[removida ? "Mensagem" : "Erro"] = removida
            ? "Atividade removida."
            : "Nao foi possivel remover a atividade.";

        return RedirectToAction(nameof(Index), new { solicitacaoId, grupo });
    }

    private int? ObterUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(claim?.Value, out var usuarioId) ? usuarioId : null;
    }
}
