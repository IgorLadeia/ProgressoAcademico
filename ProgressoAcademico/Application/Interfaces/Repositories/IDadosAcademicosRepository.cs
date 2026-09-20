using ProgressoAcademico.Models;

namespace ProgressoAcademico.Application.Interfaces.Repositories;

public interface IDadosAcademicosRepository
{
    Task<List<Instituicao>> ListarInstituicoesAsync();
    Task<List<TipoVinculo>> ListarTiposVinculoAsync();
    Task<List<Nivel>> ListarNiveisAsync();
    Task<List<TipoProgresso>> ListarTiposProgressoAsync();
}
