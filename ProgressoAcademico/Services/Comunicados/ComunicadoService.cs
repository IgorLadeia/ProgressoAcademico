using ProgressoAcademico.Application.DTOs.Comunicados;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.Comunicados;

namespace ProgressoAcademico.Services.Comunicados;

public class ComunicadoService : IComunicadoService
{
    private readonly IComunicadoRepository _comunicadoRepository;

    public ComunicadoService(IComunicadoRepository comunicadoRepository)
    {
        _comunicadoRepository = comunicadoRepository;
    }

    public async Task<List<ComunicadoResumoDto>> ListarPublicadosAsync()
    {
        var comunicados = await _comunicadoRepository.ListarPublicadosAsync();
        return comunicados.Select(Mapear).ToList();
    }

    public async Task<List<ComunicadoResumoDto>> ListarTodosAsync()
    {
        var comunicados = await _comunicadoRepository.ListarTodosAsync();
        return comunicados.Select(Mapear).ToList();
    }

    public async Task<ComunicadoFormViewModel?> ObterParaEdicaoAsync(int comunicadoId)
    {
        var comunicado = await _comunicadoRepository.ObterPorIdAsync(comunicadoId);

        if (comunicado == null)
            return null;

        return new ComunicadoFormViewModel
        {
            ComunicadoId = comunicado.ComunicadoId,
            Titulo = comunicado.Titulo,
            Resumo = comunicado.Resumo,
            Conteudo = comunicado.Conteudo,
            Categoria = comunicado.Categoria,
            Publicado = comunicado.Publicado
        };
    }

    public async Task CriarAsync(ComunicadoFormViewModel model, int? usuarioId)
    {
        var agora = DateTime.UtcNow;
        var comunicado = new Comunicado
        {
            Titulo = model.Titulo.Trim(),
            Resumo = model.Resumo.Trim(),
            Conteudo = model.Conteudo.Trim(),
            Categoria = model.Categoria.Trim(),
            Publicado = model.Publicado,
            DataPublicacao = agora,
            DataCriacao = agora,
            CriadoPorUsuarioId = usuarioId
        };

        await _comunicadoRepository.AdicionarAsync(comunicado);
        await _comunicadoRepository.SalvarAlteracoesAsync();
    }

    public async Task<bool> AtualizarAsync(ComunicadoFormViewModel model)
    {
        if (!model.ComunicadoId.HasValue)
            return false;

        var comunicado = await _comunicadoRepository.ObterPorIdAsync(model.ComunicadoId.Value);

        if (comunicado == null)
            return false;

        comunicado.Titulo = model.Titulo.Trim();
        comunicado.Resumo = model.Resumo.Trim();
        comunicado.Conteudo = model.Conteudo.Trim();
        comunicado.Categoria = model.Categoria.Trim();
        comunicado.Publicado = model.Publicado;
        comunicado.DataAtualizacao = DateTime.UtcNow;

        await _comunicadoRepository.SalvarAlteracoesAsync();
        return true;
    }

    public async Task<bool> ExcluirAsync(int comunicadoId)
    {
        var comunicado = await _comunicadoRepository.ObterPorIdAsync(comunicadoId);

        if (comunicado == null)
            return false;

        _comunicadoRepository.Remover(comunicado);
        await _comunicadoRepository.SalvarAlteracoesAsync();

        return true;
    }

    private static ComunicadoResumoDto Mapear(Comunicado comunicado)
    {
        return new ComunicadoResumoDto
        {
            ComunicadoId = comunicado.ComunicadoId,
            Titulo = comunicado.Titulo,
            Resumo = comunicado.Resumo,
            Conteudo = comunicado.Conteudo,
            Categoria = comunicado.Categoria,
            DataPublicacao = comunicado.DataPublicacao,
            Publicado = comunicado.Publicado
        };
    }
}
