using Microsoft.AspNetCore.Mvc.Rendering;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models.ViewModels.Cadastro;
using ProgressoAcademico.Models.ViewModels.ProfessorPerfil;
using ProgressoAcademico.Models.ViewModels.ProfessorPortal;

namespace ProgressoAcademico.Services.Usuarios;

public class DadosAcademicosService : IDadosAcademicosService
{
    private readonly IDadosAcademicosRepository _dadosAcademicosRepository;

    public DadosAcademicosService(IDadosAcademicosRepository dadosAcademicosRepository)
    {
        _dadosAcademicosRepository = dadosAcademicosRepository;
    }

    public async Task PreencherOpcoesCadastroAsync(CadastroViewModel model)
    {
        var instituicoes = await _dadosAcademicosRepository.ListarInstituicoesAsync();
        var tiposVinculo = await _dadosAcademicosRepository.ListarTiposVinculoAsync();
        var niveis = await _dadosAcademicosRepository.ListarNiveisAsync();

        model.Instituicoes = instituicoes
            .Select(i => new SelectListItem($"{i.Sigla} - {i.Nome}", i.InstituicaoId.ToString()))
            .ToList();

        model.TiposVinculo = tiposVinculo
            .Select(t => new SelectListItem(t.Nome, t.TipoVinculoId.ToString()))
            .ToList();

        model.Niveis = niveis
            .Select(n => new SelectListItem($"{n.Codigo} - {n.Classe}", n.NivelId.ToString()))
            .ToList();
    }

    public async Task PreencherOpcoesPerfilAsync(ProfessorPerfilEdicaoViewModel model)
    {
        var instituicoes = await _dadosAcademicosRepository.ListarInstituicoesAsync();
        var tiposVinculo = await _dadosAcademicosRepository.ListarTiposVinculoAsync();
        var niveis = await _dadosAcademicosRepository.ListarNiveisAsync();

        model.Instituicoes = instituicoes
            .Select(i => new SelectListItem($"{i.Sigla} - {i.Nome}", i.InstituicaoId.ToString()))
            .ToList();

        model.TiposVinculo = tiposVinculo
            .Select(t => new SelectListItem(t.Nome, t.TipoVinculoId.ToString()))
            .ToList();

        model.Niveis = niveis
            .Select(n => new SelectListItem($"{n.Codigo} - {n.Classe}", n.NivelId.ToString()))
            .ToList();
    }

    public async Task PreencherOpcoesSolicitacaoAsync(CriarSolicitacaoViewModel model)
    {
        var tiposProgresso = await _dadosAcademicosRepository.ListarTiposProgressoAsync();
        var niveis = await _dadosAcademicosRepository.ListarNiveisAsync();

        model.TiposProgresso = tiposProgresso
            .Select(t => new SelectListItem(t.Nome, t.TipoProgressoId.ToString()))
            .ToList();

        model.Niveis = niveis
            .Select(n => new SelectListItem($"{n.Codigo} - {n.Classe}", n.NivelId.ToString()))
            .ToList();
    }
}
