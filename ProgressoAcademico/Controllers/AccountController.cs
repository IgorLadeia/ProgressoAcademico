using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressoAcademico.Models.DTOs.Auth;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.Login;
using ProgressoAcademico.Services.Auth;
using System.Security.Claims;

namespace ProgressoAcademico.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var resultado = await _authService.AutenticarAsync(model);

        if (resultado == null)
        {
            ModelState.AddModelError(string.Empty, "Email ou senha invalidos.");
            return View(model);
        }

        var claims = UsuarioClaimsFactory.CriarClaims(new Usuario
        {
            UsuarioId = resultado.Usuario.UsuarioId,
            Nome = resultado.Usuario.Nome,
            Email = resultado.Usuario.Email,
            PerfilAcesso = resultado.Usuario.PerfilAcesso,
            PodeRevisar = resultado.Usuario.PodeRevisar
        }, resultado.Usuario.Nome);

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = false });

        TempData["Mensagem"] = "Login validado com sucesso.";
        return resultado.Usuario.PerfilAcesso switch
        {
            PerfisAcesso.Administrador => RedirectToAction("Dashboard", "AdminSolicitacoes"),
            _ => RedirectToAction("Dashboard", "ProfessorPortal")
        };
    }

    [AllowAnonymous]
    [HttpPost("api/account/login")]
    public async Task<IActionResult> LoginApi([FromBody] LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        LoginResponseDto? resultado = await _authService.AutenticarAsync(model);

        if (resultado == null)
            return Unauthorized(new { mensagem = "Email ou senha invalidos." });

        return Ok(resultado);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}
