using Microsoft.AspNetCore.Mvc;
using ProgressoAcademico.Models.ViewModels.ProfessorPerfil;
using ProgressoAcademico.Services.Usuarios;
using System.Security.Claims;

namespace ProgressoAcademico.ViewComponents;

public class ProfessorHeaderViewComponent : ViewComponent
{
    private readonly IUsuarioService _usuarioService;

    public ProfessorHeaderViewComponent(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var claim = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        var perfilAcesso = UserClaimsPrincipal.FindFirstValue(ClaimTypes.Role) ?? "Professor";
        var perfil = int.TryParse(claim, out var usuarioId)
            ? await _usuarioService.ObterPerfilProfessorAsync(usuarioId)
            : null;
        var nomeExibicao = perfil?.NomeExibicao ?? UserClaimsPrincipal.Identity?.Name ?? "Usuario";

        return View(new ProfessorHeaderViewModel
        {
            NomeExibicao = nomeExibicao,
            PerfilAcesso = perfilAcesso,
            Inicial = nomeExibicao.Length > 0 ? nomeExibicao[..1].ToUpperInvariant() : "U",
            PossuiFoto = perfil?.PossuiFoto == true,
            FotoPerfilUrl = perfil?.FotoPerfilUrl,
            AvatarFallbackUrl = AvatarFallbackUrl(usuarioId)
        });
    }

    private static string AvatarFallbackUrl(int usuarioId)
    {
        var indice = Math.Abs(usuarioId % 4) + 1;
        return $"/images/avatars/avatar-{indice}.svg";
    }
}
