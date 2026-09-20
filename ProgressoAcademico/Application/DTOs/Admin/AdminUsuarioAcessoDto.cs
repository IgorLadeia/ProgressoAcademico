namespace ProgressoAcademico.Application.DTOs.Admin;

public class AdminUsuarioAcessoDto
{
    public int UsuarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PerfilAcesso { get; set; } = string.Empty;
    public bool PodeRevisar { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataExclusao { get; set; }
    public string AvatarFallbackUrl { get; set; } = "/images/avatars/avatar-1.svg";
}
