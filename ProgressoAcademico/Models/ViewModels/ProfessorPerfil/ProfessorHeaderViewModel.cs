namespace ProgressoAcademico.Models.ViewModels.ProfessorPerfil;

public class ProfessorHeaderViewModel
{
    public string NomeExibicao { get; init; } = "Usuario";
    public string PerfilAcesso { get; init; } = "Professor";
    public string Inicial { get; init; } = "U";
    public bool PossuiFoto { get; init; }
    public string? FotoPerfilUrl { get; init; }
    public string AvatarFallbackUrl { get; init; } = "/images/avatars/avatar-1.svg";
}
