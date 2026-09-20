using FluentAssertions;
using Moq;
using ProgressoAcademico.Application.DTOs.Solicitacoes;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;
using ProgressoAcademico.Services.Solicitacoes;

namespace ProgressoAcademico.Tests.Services;

public class SolicitacaoProgressaoServiceTests
{
    [Fact]
    public async Task CriarAsync_QuandoProfessorJaPossuiSolicitacaoAberta_DeveBloquearNovaSolicitacao()
    {
        var repository = new Mock<ISolicitacaoProgressaoRepository>();
        repository.Setup(r => r.ExisteSolicitacaoAbertaAsync(10)).ReturnsAsync(true);

        var service = new SolicitacaoProgressaoService(repository.Object);

        var resultado = await service.CriarAsync(10, new CriarSolicitacaoDto
        {
            TipoProgressoId = 1,
            NivelOrigemId = 1,
            NivelDestinoId = 2
        });

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Contain("em aberto");
        repository.Verify(r => r.AdicionarAsync(It.IsAny<SolicitacaoProgressao>()), Times.Never);
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_ComDadosValidos_DeveCriarSolicitacaoEmProcessamento()
    {
        SolicitacaoProgressao? criada = null;
        var repository = new Mock<ISolicitacaoProgressaoRepository>();
        repository.Setup(r => r.ExisteSolicitacaoAbertaAsync(10)).ReturnsAsync(false);
        repository.Setup(r => r.TipoProgressoExisteAsync(1)).ReturnsAsync(true);
        repository.Setup(r => r.NivelExisteAsync(1)).ReturnsAsync(true);
        repository.Setup(r => r.NivelExisteAsync(2)).ReturnsAsync(true);
        repository
            .Setup(r => r.ObterStatusPorNomeAsync(StatusSolicitacaoNomes.EmProcessamento))
            .ReturnsAsync(TestDataFactory.Status(1, StatusSolicitacaoNomes.EmProcessamento));
        repository
            .Setup(r => r.AdicionarAsync(It.IsAny<SolicitacaoProgressao>()))
            .Callback<SolicitacaoProgressao>(s =>
            {
                s.SolicitacaoProgressaoId = 99;
                criada = s;
            })
            .Returns(Task.CompletedTask);

        var service = new SolicitacaoProgressaoService(repository.Object);

        var resultado = await service.CriarAsync(10, new CriarSolicitacaoDto
        {
            TipoProgressoId = 1,
            NivelOrigemId = 1,
            NivelDestinoId = 2,
            Apelido = "  Pedido 2026  ",
            Observacao = "  Observacao inicial  "
        });

        resultado.Sucesso.Should().BeTrue();
        resultado.SolicitacaoProgressaoId.Should().Be(99);
        criada.Should().NotBeNull();
        criada!.UsuarioId.Should().Be(10);
        criada.StatusSolicitacaoId.Should().Be(1);
        criada.Apelido.Should().Be("Pedido 2026");
        criada.Observacao.Should().Be("Observacao inicial");
        criada.ParecerFinal.Should().BeNull();
        criada.DataFechamento.Should().BeNull();
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterDetalheDoProfessorAsync_ComSolicitacaoEncerrada_DeveRetornarSomenteLeitura()
    {
        var solicitacao = TestDataFactory.Solicitacao(
            statusNome: StatusSolicitacaoNomes.Aprovada,
            comAtividade: true,
            comDocumentoComprovatorio: true);

        var repository = new Mock<ISolicitacaoProgressaoRepository>();
        repository
            .Setup(r => r.ObterDetalhePorIdEUsuarioAsync(solicitacao.SolicitacaoProgressaoId, solicitacao.UsuarioId))
            .ReturnsAsync(solicitacao);

        var service = new SolicitacaoProgressaoService(repository.Object);

        var detalhe = await service.ObterDetalheDoProfessorAsync(solicitacao.UsuarioId, solicitacao.SolicitacaoProgressaoId);

        detalhe.Should().NotBeNull();
        detalhe!.PodeEditar.Should().BeFalse();
        detalhe.PercentualPreenchimento.Should().Be(100);
        detalhe.TotalAtividades.Should().Be(1);
        detalhe.TotalDocumentos.Should().Be(1);
    }

    [Fact]
    public async Task SubmeterAsync_ComRascunhoValido_DeveEnviarParaAtribuicao()
    {
        var solicitacao = TestDataFactory.Solicitacao(
            statusNome: StatusSolicitacaoNomes.EmProcessamento,
            comAtividade: true);
        var statusAguardando = TestDataFactory.Status(2, StatusSolicitacaoNomes.AguardandoAtribuicao);

        var repository = new Mock<ISolicitacaoProgressaoRepository>();
        repository
            .Setup(r => r.ObterParaSubmissaoPorIdEUsuarioAsync(solicitacao.SolicitacaoProgressaoId, solicitacao.UsuarioId))
            .ReturnsAsync(solicitacao);
        repository
            .Setup(r => r.ObterStatusPorNomeAsync(StatusSolicitacaoNomes.AguardandoAtribuicao))
            .ReturnsAsync(statusAguardando);

        var service = new SolicitacaoProgressaoService(repository.Object);

        var resultado = await service.SubmeterAsync(solicitacao.UsuarioId, solicitacao.SolicitacaoProgressaoId);

        resultado.Sucesso.Should().BeTrue();
        solicitacao.StatusSolicitacaoId.Should().Be(statusAguardando.StatusSolicitacaoId);
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task SubmeterAsync_ComAjustesERevisores_DeveReenviarParaRevisao()
    {
        var solicitacao = TestDataFactory.Solicitacao(
            statusNome: StatusSolicitacaoNomes.AjustesSolicitados,
            comAtividade: true);
        var revisor = TestDataFactory.Professor(usuarioId: 40, perfil: PerfisAcesso.Revisor);
        var statusRevisao = TestDataFactory.Status(3, StatusSolicitacaoNomes.EmRevisao);
        solicitacao.Revisores.Add(new SolicitacaoRevisor
        {
            SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId,
            SolicitacaoProgressao = solicitacao,
            RevisorUsuarioId = revisor.UsuarioId,
            RevisorUsuario = revisor,
            DataAtribuicao = DateTime.UtcNow.AddDays(-2),
            StatusRevisao = StatusRevisaoNomes.AjustesSolicitados,
            Parecer = "Corrigir comprovante."
        });

        var repository = new Mock<ISolicitacaoProgressaoRepository>();
        repository
            .Setup(r => r.ObterParaSubmissaoPorIdEUsuarioAsync(solicitacao.SolicitacaoProgressaoId, solicitacao.UsuarioId))
            .ReturnsAsync(solicitacao);
        repository
            .Setup(r => r.ObterStatusPorNomeAsync(StatusSolicitacaoNomes.EmRevisao))
            .ReturnsAsync(statusRevisao);

        var service = new SolicitacaoProgressaoService(repository.Object);

        var resultado = await service.SubmeterAsync(solicitacao.UsuarioId, solicitacao.SolicitacaoProgressaoId);

        resultado.Sucesso.Should().BeTrue();
        solicitacao.StatusSolicitacaoId.Should().Be(statusRevisao.StatusSolicitacaoId);
        solicitacao.Revisores.Should().OnlyContain(r => r.StatusRevisao == StatusRevisaoNomes.EmRevisao);
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }
}
