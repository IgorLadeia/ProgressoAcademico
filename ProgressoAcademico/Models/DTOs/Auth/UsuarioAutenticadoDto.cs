namespace ProgressoAcademico.Models.DTOs.Auth;

public class UsuarioAutenticadoDto
{
    public int UsuarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PerfilAcesso { get; set; } = string.Empty;
    public bool PodeRevisar { get; set; }
}
