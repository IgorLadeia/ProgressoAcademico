using FluentAssertions;
using Moq;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.DTOs.Auth;
using ProgressoAcademico.Models.ViewModels.Login;
using ProgressoAcademico.Services.Auth;

namespace ProgressoAcademico.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task AutenticarAsync_ComSenhaValidaEPerfilValido_DeveRetornarTokenEUsuario()
    {
        var usuario = TestDataFactory.Professor();
        usuario.Email = "professor@ufabc.edu.br";
        usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword("Professor@123456");
        usuario.UsuarioPerfil = new UsuarioPerfil
        {
            UsuarioId = usuario.UsuarioId,
            Apelido = "Nome Social"
        };

        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository
            .Setup(r => r.ObterPorEmailAsync(usuario.Email, true))
            .ReturnsAsync(usuario);

        var expiracao = DateTime.UtcNow.AddHours(2);
        var jwtTokenService = new Mock<IJwtTokenService>();
        jwtTokenService
            .Setup(s => s.GerarToken(usuario))
            .Returns(new JwtTokenResult { Token = "jwt-token-teste", ExpiraEmUtc = expiracao });

        var service = new AuthService(usuarioRepository.Object, jwtTokenService.Object);

        var resultado = await service.AutenticarAsync(new LoginViewModel
        {
            Email = usuario.Email,
            Senha = "Professor@123456"
        });

        resultado.Should().NotBeNull();
        resultado!.Token.Should().Be("jwt-token-teste");
        resultado.Usuario.Email.Should().Be(usuario.Email);
        resultado.Usuario.Nome.Should().Be("Nome Social");
        resultado.Usuario.PerfilAcesso.Should().Be(PerfisAcesso.Professor);
        resultado.ExpiraEmUtc.Should().Be(expiracao);
        jwtTokenService.Verify(s => s.GerarToken(usuario), Times.Once);
    }

    [Fact]
    public async Task AutenticarAsync_ComSenhaInvalida_NaoDeveGerarToken()
    {
        var usuario = TestDataFactory.Professor();
        usuario.Email = "professor@ufabc.edu.br";
        usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword("Professor@123456");

        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository
            .Setup(r => r.ObterPorEmailAsync(usuario.Email, true))
            .ReturnsAsync(usuario);

        var jwtTokenService = new Mock<IJwtTokenService>();
        var service = new AuthService(usuarioRepository.Object, jwtTokenService.Object);

        var resultado = await service.AutenticarAsync(new LoginViewModel
        {
            Email = usuario.Email,
            Senha = "senha-errada"
        });

        resultado.Should().BeNull();
        jwtTokenService.Verify(s => s.GerarToken(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task AutenticarAsync_ComUsuarioInativo_NaoDeveAutenticar()
    {
        var usuario = TestDataFactory.Professor(ativo: false);

        var usuarioRepository = new Mock<IUsuarioRepository>();
        usuarioRepository
            .Setup(r => r.ObterPorEmailAsync(usuario.Email, true))
            .ReturnsAsync(usuario);

        var jwtTokenService = new Mock<IJwtTokenService>();
        var service = new AuthService(usuarioRepository.Object, jwtTokenService.Object);

        var resultado = await service.AutenticarAsync(new LoginViewModel
        {
            Email = usuario.Email,
            Senha = "Professor@123456"
        });

        resultado.Should().BeNull();
        jwtTokenService.Verify(s => s.GerarToken(It.IsAny<Usuario>()), Times.Never);
    }
}
