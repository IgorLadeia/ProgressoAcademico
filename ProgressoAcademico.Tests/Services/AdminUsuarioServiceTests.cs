using FluentAssertions;
using Moq;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;
using ProgressoAcademico.Services.Admin;

namespace ProgressoAcademico.Tests.Services;

public class AdminUsuarioServiceTests
{
    [Fact]
    public async Task AlterarNivelAcessoAsync_AdminNaoPodeReduzirProprioNivel()
    {
        var repository = new Mock<IAdminUsuarioRepository>();
        var service = new AdminUsuarioService(repository.Object);

        var resultado = await service.AlterarNivelAcessoAsync(
            usuarioId: 1,
            adminId: 1,
            nivelAcesso: PerfisAcesso.Professor);

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Contain("proprio nivel");
        repository.Verify(r => r.ObterAsync(It.IsAny<int>()), Times.Never);
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Never);
    }

    [Fact]
    public async Task AlterarNivelAcessoAsync_RevisorMantemPerfilProfessorComFuncaoDeRevisao()
    {
        var usuario = TestDataFactory.Professor(usuarioId: 20);
        var repository = new Mock<IAdminUsuarioRepository>();
        repository.Setup(r => r.ObterAsync(usuario.UsuarioId)).ReturnsAsync(usuario);
        var service = new AdminUsuarioService(repository.Object);

        var resultado = await service.AlterarNivelAcessoAsync(
            usuario.UsuarioId,
            adminId: 1,
            nivelAcesso: PerfisAcesso.Revisor);

        resultado.Sucesso.Should().BeTrue();
        usuario.PerfilAcesso.Should().Be(PerfisAcesso.Professor);
        usuario.PodeRevisar.Should().BeTrue();
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task AlterarNivelAcessoAsync_NaoPermiteAlterarOutroAdministrador()
    {
        var outroAdmin = TestDataFactory.Professor(usuarioId: 30, perfil: PerfisAcesso.Administrador);
        var repository = new Mock<IAdminUsuarioRepository>();
        repository.Setup(r => r.ObterAsync(outroAdmin.UsuarioId)).ReturnsAsync(outroAdmin);
        var service = new AdminUsuarioService(repository.Object);

        var resultado = await service.AlterarNivelAcessoAsync(
            outroAdmin.UsuarioId,
            adminId: 1,
            nivelAcesso: PerfisAcesso.Professor);

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Contain("outro administrador");
        outroAdmin.PerfilAcesso.Should().Be(PerfisAcesso.Administrador);
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Never);
    }
}
