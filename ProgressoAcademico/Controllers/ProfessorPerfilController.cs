using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.ProfessorPerfil;
using ProgressoAcademico.Services.Usuarios;
using System.Security.Claims;

namespace ProgressoAcademico.Controllers;

[Authorize(Roles = PerfisAcesso.Professor)]
public class ProfessorPerfilController : Controller
{
    private readonly IUsuarioService _usuarioService;
    private readonly IDadosAcademicosService _dadosAcademicosService;

    public ProfessorPerfilController(
        IUsuarioService usuarioService,
        IDadosAcademicosService dadosAcademicosService)
    {
        _usuarioService = usuarioService;
        _dadosAcademicosService = dadosAcademicosService;
    }

    [HttpGet]
    public async Task<IActionResult> Editar()
    {
        var usuarioId = ObterUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        var perfil = await _usuarioService.ObterPerfilProfessorAsync(usuarioId.Value);

        if (perfil == null)
            return NotFound();

        var model = new ProfessorPerfilEdicaoViewModel
        {
            NomeCompleto = perfil.NomeCompleto,
            Email = perfil.Email,
            NomeSocial = perfil.NomeSocial,
            PossuiFoto = perfil.PossuiFoto,
            FotoPerfilUrl = perfil.FotoPerfilUrl,
            DataUltimoProgresso = perfil.DataUltimoProgresso,
            InstituicaoId = perfil.InstituicaoId,
            TipoVinculoId = perfil.TipoVinculoId,
            NivelId = perfil.NivelId,
            DataIngressoInstituicao = perfil.DataIngressoInstituicao
        };

        await _dadosAcademicosService.PreencherOpcoesPerfilAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(2_200_000)]
    public async Task<IActionResult> Editar(ProfessorPerfilEdicaoViewModel model)
    {
        var usuarioId = ObterUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        if (!ModelState.IsValid)
        {
            await PreencherDadosSomenteLeituraAsync(usuarioId.Value, model);
            await _dadosAcademicosService.PreencherOpcoesPerfilAsync(model);
            return View(model);
        }

        var resultado = await _usuarioService.AtualizarPerfilProfessorAsync(usuarioId.Value, model);

        if (!resultado.Sucesso)
        {
            ModelState.AddModelError(string.Empty, resultado.Erro ?? "Nao foi possivel atualizar o perfil.");
            await PreencherDadosSomenteLeituraAsync(usuarioId.Value, model);
            await _dadosAcademicosService.PreencherOpcoesPerfilAsync(model);
            return View(model);
        }

        TempData["Mensagem"] = "Perfil atualizado com sucesso.";
        return RedirectToAction(nameof(Editar));
    }

    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Foto()
    {
        var usuarioId = ObterUsuarioId();

        if (usuarioId == null)
            return Unauthorized();

        var foto = await _usuarioService.ObterFotoPerfilAsync(usuarioId.Value);
        return foto == null ? NotFound() : File(foto.Arquivo, foto.ContentType);
    }

    private async Task PreencherDadosSomenteLeituraAsync(int usuarioId, ProfessorPerfilEdicaoViewModel model)
    {
        var perfil = await _usuarioService.ObterPerfilProfessorAsync(usuarioId);

        if (perfil == null)
            return;

        model.PossuiFoto = perfil.PossuiFoto;
        model.FotoPerfilUrl = perfil.FotoPerfilUrl;
        model.DataUltimoProgresso ??= perfil.DataUltimoProgresso;
        model.InstituicaoId ??= perfil.InstituicaoId;
        model.TipoVinculoId ??= perfil.TipoVinculoId;
        model.NivelId ??= perfil.NivelId;
        model.DataIngressoInstituicao ??= perfil.DataIngressoInstituicao;
    }

    private int? ObterUsuarioId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var usuarioId) ? usuarioId : null;
    }
}
