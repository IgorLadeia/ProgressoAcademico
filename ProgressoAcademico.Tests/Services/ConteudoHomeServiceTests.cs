using FluentAssertions;
using Moq;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.ConteudoHome;
using ProgressoAcademico.Services.ConteudoHome;

namespace ProgressoAcademico.Tests.Services;

public class ConteudoHomeServiceTests
{
    [Fact]
    public async Task ObterAsync_DeveConverterListasEmItensSeparados()
    {
        var repository = new Mock<IConteudoHomeRepository>();
        repository.Setup(r => r.ObterAsync()).ReturnsAsync(CriarConteudo());
        var service = new ConteudoHomeService(repository.Object);

        var resultado = await service.ObterAsync();

        resultado.ItensDestaque.Should().BeEquivalentTo("Item um", "Item dois");
        resultado.RecursosProfessor.Should().BeEquivalentTo("Recurso A", "Recurso B");
        resultado.RecursosAdministrador.Should().ContainSingle("Recurso C");
    }

    [Fact]
    public async Task AtualizarAsync_DeveNormalizarConteudoERegistrarAdministrador()
    {
        var conteudo = CriarConteudo();
        var repository = new Mock<IConteudoHomeRepository>();
        repository.Setup(r => r.ObterParaAtualizacaoAsync()).ReturnsAsync(conteudo);
        var service = new ConteudoHomeService(repository.Object);

        var resultado = await service.AtualizarAsync(new ConteudoHomeEdicaoViewModel
        {
            TituloPrincipal = "  Novo titulo  ",
            ResumoProjeto = "  Novo resumo  ",
            TituloDestaque = "  Novo destaque  ",
            ItensDestaque = " Primeiro \n\n Segundo ",
            AvisoEscopo = "  Novo aviso  ",
            DescricaoProfessor = "  Professor  ",
            RecursosProfessor = " Um \n Dois ",
            DescricaoAdministrador = "  Administrador  ",
            RecursosAdministrador = " Tres \n Quatro "
        }, usuarioId: 12);

        resultado.Should().BeTrue();
        conteudo.TituloPrincipal.Should().Be("Novo titulo");
        conteudo.ItensDestaque.Should().Be($"Primeiro{Environment.NewLine}Segundo");
        conteudo.AtualizadoPorUsuarioId.Should().Be(12);
        conteudo.DataAtualizacao.Should().NotBe(default);
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    private static ConteudoHome CriarConteudo()
    {
        return new ConteudoHome
        {
            ConteudoHomeId = 1,
            TituloPrincipal = "Titulo",
            ResumoProjeto = "Resumo",
            TituloDestaque = "Destaque",
            ItensDestaque = "Item um\nItem dois",
            AvisoEscopo = "Aviso",
            DescricaoProfessor = "Professor",
            RecursosProfessor = "Recurso A\nRecurso B",
            DescricaoAdministrador = "Administrador",
            RecursosAdministrador = "Recurso C"
        };
    }
}
