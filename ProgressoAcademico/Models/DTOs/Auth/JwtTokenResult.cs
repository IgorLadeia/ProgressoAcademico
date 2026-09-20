namespace ProgressoAcademico.Models.DTOs.Auth;

public class JwtTokenResult
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEmUtc { get; set; }
}
