using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressoAcademico.Models.ViewModels.ChangePassword;
using ProgressoAcademico.Services.Usuarios;
using System.Security.Claims;

namespace ProgressoAcademico.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost("alterar-senha")]
    public async Task<IActionResult> AlterarSenha([FromBody] ChangePasswordViewModel dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var usuarioId = ObterUsuarioId();

        if (usuarioId == null)
            return Unauthorized("Usuario nao autenticado.");

        var senhaAlterada = await _usuarioService.AlterarSenhaAsync(usuarioId.Value, dto);

        if (!senhaAlterada)
            return BadRequest("Nao foi possivel alterar a senha.");

        return Ok(new { mensagem = "Senha alterada com sucesso." });
    }

    [HttpDelete("me")]
    public async Task<IActionResult> ExcluirMinhaConta()
    {
        var usuarioId = ObterUsuarioId();

        if (usuarioId == null)
            return Unauthorized("Usuario nao autenticado.");

        var anonimizado = await _usuarioService.AnonimizarUsuarioAsync(usuarioId.Value);

        if (!anonimizado)
            return BadRequest("Nao foi possivel processar a solicitacao de exclusao.");

        return Ok(new { mensagem = "Dados pessoais anonimizados conforme diretriz LGPD do sistema." });
    }

    private int? ObterUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(claim?.Value, out var usuarioId) ? usuarioId : null;
    }
}
