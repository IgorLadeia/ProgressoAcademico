using ProgressoAcademico.Models.DTOs.Auth;
using ProgressoAcademico.Models.ViewModels.Login;

namespace ProgressoAcademico.Services.Auth;

public interface IAuthService
{
    Task<LoginResponseDto?> AutenticarAsync(LoginViewModel model);
}
