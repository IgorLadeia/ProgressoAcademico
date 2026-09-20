namespace ProgressoAcademico.Models.DTOs.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEmUtc { get; set; }
    public UsuarioAutenticadoDto Usuario { get; set; } = new();
}
