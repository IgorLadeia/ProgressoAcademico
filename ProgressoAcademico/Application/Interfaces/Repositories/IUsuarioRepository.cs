using ProgressoAcademico.Models;
using ProgressoAcademico.Application.DTOs.Usuarios;

namespace ProgressoAcademico.Application.Interfaces.Repositories;

public interface IUsuarioRepository
{
    Task<bool> EmailExisteAsync(string email);
    Task<Usuario?> ObterPorEmailAsync(string email, bool asNoTracking = true);
    Task<Usuario?> ObterAtivoPorIdComPerfilAsync(int usuarioId);
    Task<bool> EmailExisteParaOutroUsuarioAsync(string email, int usuarioId);
    Task<ProfessorPerfilDto?> ObterResumoPerfilProfessorAsync(int usuarioId);
    Task AdicionarAsync(Usuario usuario);
    Task AdicionarPerfilAsync(UsuarioPerfil perfil);
    Task AdicionarVinculoAsync(VinculoInstitucional vinculo);
    Task SalvarAlteracoesAsync();
}
