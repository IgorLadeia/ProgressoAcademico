using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.ProfessorPerfil;
using ProgressoAcademico.Services.Usuarios;

namespace ProgressoAcademico.Tests.Services;

public class UsuarioPerfilServiceTests
{
    [Fact]
    public async Task AtualizarPerfilProfessorAsync_ComPngValido_DeveCriarPerfilEGuardarFoto()
    {
        var usuario = TestDataFactory.Professor();
        var repository = CriarRepository(usuario);
        var service = new UsuarioService(repository.Object);
        var png = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 1, 2, 3 };

        var resultado = await service.AtualizarPerfilProfessorAsync(usuario.UsuarioId, new ProfessorPerfilEdicaoViewModel
        {
            NomeSocial = "  Professora Camila  ",
            Foto = CriarArquivo(png, "perfil.png", "image/png")
        });

        resultado.Sucesso.Should().BeTrue();
        usuario.UsuarioPerfil.Should().NotBeNull();
        usuario.UsuarioPerfil!.Apelido.Should().Be("Professora Camila");
        usuario.UsuarioPerfil.FotoPerfilArquivo.Should().Equal(png);
        usuario.UsuarioPerfil.FotoPerfilContentType.Should().Be("image/png");
        usuario.UsuarioPerfil.FotoPerfilAtualizadaEm.Should().NotBeNull();
        repository.Verify(r => r.AdicionarPerfilAsync(usuario.UsuarioPerfil), Times.Once);
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarPerfilProfessorAsync_ComArquivoDisfarcado_DeveRejeitarFoto()
    {
        var usuario = TestDataFactory.Professor();
        var repository = CriarRepository(usuario);
        var service = new UsuarioService(repository.Object);

        var resultado = await service.AtualizarPerfilProfessorAsync(usuario.UsuarioId, new ProfessorPerfilEdicaoViewModel
        {
            NomeSocial = "Professor Teste",
            Foto = CriarArquivo([1, 2, 3, 4], "perfil.png", "image/png")
        });

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Contain("PNG ou JPEG");
        repository.Verify(r => r.AdicionarPerfilAsync(It.IsAny<UsuarioPerfil>()), Times.Never);
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Never);
    }

    [Fact]
    public async Task AtualizarPerfilProfessorAsync_RemovendoFoto_DeveManterFallbackDoNomeCivil()
    {
        var usuario = TestDataFactory.Professor();
        usuario.UsuarioPerfil = new UsuarioPerfil
        {
            UsuarioId = usuario.UsuarioId,
            Apelido = "Nome social anterior",
            FotoPerfilArquivo = [0xFF, 0xD8, 0xFF],
            FotoPerfilContentType = "image/jpeg",
            FotoPerfilAtualizadaEm = DateTime.UtcNow
        };
        var repository = CriarRepository(usuario);
        var service = new UsuarioService(repository.Object);

        var resultado = await service.AtualizarPerfilProfessorAsync(usuario.UsuarioId, new ProfessorPerfilEdicaoViewModel
        {
            NomeSocial = "   ",
            RemoverFoto = true
        });
        var perfil = await service.ObterPerfilProfessorAsync(usuario.UsuarioId);

        resultado.Sucesso.Should().BeTrue();
        usuario.UsuarioPerfil.Apelido.Should().BeNull();
        usuario.UsuarioPerfil.FotoPerfilArquivo.Should().BeNull();
        perfil!.NomeExibicao.Should().Be(usuario.Nome);
        perfil.PossuiFoto.Should().BeFalse();
    }

    private static Mock<IUsuarioRepository> CriarRepository(Usuario usuario)
    {
        var repository = new Mock<IUsuarioRepository>();
        repository
            .Setup(r => r.ObterAtivoPorIdComPerfilAsync(usuario.UsuarioId))
            .ReturnsAsync(usuario);
        repository
            .Setup(r => r.ObterResumoPerfilProfessorAsync(usuario.UsuarioId))
            .ReturnsAsync(() => new ProgressoAcademico.Application.DTOs.Usuarios.ProfessorPerfilDto
            {
                UsuarioId = usuario.UsuarioId,
                NomeCompleto = usuario.Nome,
                NomeExibicao = string.IsNullOrWhiteSpace(usuario.UsuarioPerfil?.Apelido)
                    ? usuario.Nome
                    : usuario.UsuarioPerfil.Apelido,
                Email = usuario.Email,
                NomeSocial = usuario.UsuarioPerfil?.Apelido,
                PossuiFoto = usuario.UsuarioPerfil?.FotoPerfilArquivo?.Length > 0
            });
        return repository;
    }

    private static IFormFile CriarArquivo(byte[] bytes, string nome, string contentType)
    {
        return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Foto", nome)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }
}
