using ProgressoAcademico.Models.ViewModels.Cadastro;
using ProgressoAcademico.Models.ViewModels.ProfessorPerfil;

namespace ProgressoAcademico.Services.Usuarios;

public interface IDadosAcademicosService
{
    Task PreencherOpcoesCadastroAsync(CadastroViewModel model);
    Task PreencherOpcoesPerfilAsync(ProfessorPerfilEdicaoViewModel model);
    Task PreencherOpcoesSolicitacaoAsync(ProgressoAcademico.Models.ViewModels.ProfessorPortal.CriarSolicitacaoViewModel model);
}
