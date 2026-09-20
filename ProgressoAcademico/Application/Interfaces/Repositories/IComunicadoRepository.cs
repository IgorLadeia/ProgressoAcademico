using ProgressoAcademico.Models;

namespace ProgressoAcademico.Application.Interfaces.Repositories;

public interface IComunicadoRepository
{
    Task<List<Comunicado>> ListarPublicadosAsync();
    Task<List<Comunicado>> ListarTodosAsync();
    Task<Comunicado?> ObterPorIdAsync(int comunicadoId);
    Task AdicionarAsync(Comunicado comunicado);
    void Remover(Comunicado comunicado);
    Task SalvarAlteracoesAsync();
}
