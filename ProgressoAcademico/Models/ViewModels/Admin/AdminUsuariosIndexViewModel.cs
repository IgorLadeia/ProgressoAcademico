using ProgressoAcademico.Application.DTOs.Admin;

namespace ProgressoAcademico.Models.ViewModels.Admin;

public class AdminUsuariosIndexViewModel
{
    public IReadOnlyList<AdminUsuarioAcessoDto> Usuarios { get; set; } = Array.Empty<AdminUsuarioAcessoDto>();
    public IReadOnlyList<string> Perfis { get; set; } = Array.Empty<string>();
}
