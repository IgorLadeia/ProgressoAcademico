using ProgressoAcademico.Application.DTOs.Usuarios;
using ProgressoAcademico.Models.ViewModels.Cadastro;
using ProgressoAcademico.Models.ViewModels.ChangePassword;
using ProgressoAcademico.Models.ViewModels.Register;
using ProgressoAcademico.Models.ViewModels.ProfessorPerfil;

namespace ProgressoAcademico.Services.Usuarios;

public interface IUsuarioService
{
    Task<CriarUsuarioResponseDto?> CriarProfessorAsync(RegisterViewModel dto);
    Task<CriarUsuarioResponseDto?> CriarProfessorCadastroAsync(CadastroViewModel dto);
    Task<bool> AlterarSenhaAsync(int usuarioId, ChangePasswordViewModel dto);
    Task<bool> AnonimizarUsuarioAsync(int usuarioId);
    Task<ProfessorPerfilDto?> ObterPerfilProfessorAsync(int usuarioId);
    Task<FotoPerfilDto?> ObterFotoPerfilAsync(int usuarioId);
    Task<AtualizarPerfilResultadoDto> AtualizarPerfilProfessorAsync(int usuarioId, ProfessorPerfilEdicaoViewModel model);
}
