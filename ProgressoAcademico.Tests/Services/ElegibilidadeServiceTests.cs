using FluentAssertions;
using Moq;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Services.Elegibilidade;

namespace ProgressoAcademico.Tests.Services;

public class ElegibilidadeServiceTests
{
    [Fact]
    public async Task AvaliarAsync_ComRequisitosBasicosAtendidos_DeveRetornarElegivel()
    {
        var solicitacao = TestDataFactory.Solicitacao(
            comAtividade: true,
            comDocumentoComprovatorio: true,
            comDocumentoOficial: false,
            dataUltimoProgresso: DateTime.UtcNow.AddMonths(-30));
        PrepararSolicitacaoElegivel(solicitacao);

        var repository = new Mock<ISolicitacaoProgressaoRepository>();
        repository
            .Setup(r => r.ObterDetalhePorIdEUsuarioAsync(solicitacao.SolicitacaoProgressaoId, solicitacao.UsuarioId))
            .ReturnsAsync(solicitacao);

        var service = new ElegibilidadeService(repository.Object);

        var resultado = await service.AvaliarAsync(solicitacao.UsuarioId, solicitacao.SolicitacaoProgressaoId);

        resultado.Should().NotBeNull();
        resultado!.Status.Should().Be("Elegivel");
        resultado.PercentualElegibilidade.Should().Be(100);
        resultado.RequisitosAtendidos.Should().Be(resultado.TotalRequisitos);
        resultado.Pendencias.Should().BeEmpty();
        resultado.Requisitos.Where(r => r.Obrigatorio).Should().OnlyContain(r => r.Atendido);
        resultado.Requisitos.Single(r => r.Nome == "Documento oficial UFABC").Obrigatorio.Should().BeFalse();
        resultado.Requisitos.Single(r => r.Nome == "Documento oficial UFABC").Atendido.Should().BeFalse();
    }

    [Fact]
    public async Task AvaliarAsync_ComIntersticioAusenteENivelInvalido_DeveRetornarNaoElegivel()
    {
        var solicitacao = TestDataFactory.Solicitacao(
            comObservacao: false,
            nivelOrigemOrdem: 2,
            nivelDestinoOrdem: 1,
            dataUltimoProgresso: null);
        solicitacao.Usuario.DataUltimoProgresso = null;

        var repository = new Mock<ISolicitacaoProgressaoRepository>();
        repository
            .Setup(r => r.ObterDetalhePorIdEUsuarioAsync(solicitacao.SolicitacaoProgressaoId, solicitacao.UsuarioId))
            .ReturnsAsync(solicitacao);

        var service = new ElegibilidadeService(repository.Object);

        var resultado = await service.AvaliarAsync(solicitacao.UsuarioId, solicitacao.SolicitacaoProgressaoId);

        resultado.Should().NotBeNull();
        resultado!.Status.Should().Be("NaoElegivel");
        resultado.Pendencias.Should().Contain(p => p.Contains("intersticio", StringComparison.OrdinalIgnoreCase));
        resultado.Pendencias.Should().Contain(p => p.Contains("nivel destino", StringComparison.OrdinalIgnoreCase)
            || p.Contains("nivel de origem", StringComparison.OrdinalIgnoreCase));
    }

    private static void PrepararSolicitacaoElegivel(ProgressoAcademico.Models.SolicitacaoProgressao solicitacao)
    {
        solicitacao.Usuario.VinculosInstitucionais.Add(new ProgressoAcademico.Models.VinculoInstitucional
        {
            UsuarioId = solicitacao.UsuarioId,
            InstituicaoId = 1,
            TipoVinculoId = 1,
            NivelId = 1,
            DataIngressoInstituicao = DateTime.UtcNow.AddYears(-5),
            Ativo = true
        });

        solicitacao.Atividades.Clear();
        solicitacao.Atividades.Add(CriarAtividade("Ensino", 40));
        solicitacao.Atividades.Add(CriarAtividade("Pesquisa", 50));
        solicitacao.Atividades.Add(CriarAtividade("Extensao", 20));
        solicitacao.Atividades.Add(CriarAtividade("Gestao", 20));
    }

    private static ProgressoAcademico.Models.Atividade CriarAtividade(string grupo, decimal pontos)
    {
        return new ProgressoAcademico.Models.Atividade
        {
            TipoAtividade = new ProgressoAcademico.Models.TipoAtividade { Nome = grupo },
            Titulo = grupo,
            DataInicio = DateTime.UtcNow.AddMonths(-3),
            Quantidade = 1,
            PontuacaoCalculada = pontos,
            Status = "Declarada"
        };
    }
}
