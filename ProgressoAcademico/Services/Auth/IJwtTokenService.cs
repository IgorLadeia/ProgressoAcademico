using ProgressoAcademico.Models;
using ProgressoAcademico.Models.DTOs.Auth;

namespace ProgressoAcademico.Services.Auth;

public interface IJwtTokenService
{
    JwtTokenResult GerarToken(Usuario usuario);
}
