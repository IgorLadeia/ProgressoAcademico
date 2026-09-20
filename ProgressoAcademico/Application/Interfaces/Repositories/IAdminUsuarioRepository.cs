using ProgressoAcademico.Models;

namespace ProgressoAcademico.Application.Interfaces.Repositories;

public interface IAdminUsuarioRepository
{
    Task<IReadOnlyList<Usuario>> ListarAsync();
    Task<Usuario?> ObterAsync(int usuarioId);
    Task SalvarAlteracoesAsync();
}
