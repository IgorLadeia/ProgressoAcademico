using ProgressoAcademico.Models;

namespace ProgressoAcademico.Application.Interfaces.Repositories;

public interface IConteudoHomeRepository
{
    Task<ConteudoHome?> ObterAsync();
    Task<ConteudoHome?> ObterParaAtualizacaoAsync();
    Task SalvarAlteracoesAsync();
}
