using FluentAssertions;
using Moq;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.Atividades;
using ProgressoAcademico.Services.Atividades;

namespace ProgressoAcademico.Tests.Services;

public class AtividadeServiceTests
{
    [Fact]
    public async Task CriarAsync_ComSolicitacaoEditavelESubtipoValido_DeveCadastrarAtividadeComPontuacao()
    {
        Atividade? criada = null;
        var solicitacao = TestDataFactory.Solicitacao();
        var subtipo = TestDataFactory.SubtipoAtividade(pontos: 12);

        var atividadeRepository = new Mock<IAtividadeRepository>();
        atividadeRepository.Setup(r => r.ObterSubtipoAsync(subtipo.SubtipoAtividadeId)).ReturnsAsync(subtipo);
        atividadeRepository
            .Setup(r => r.AdicionarAsync(It.IsAny<Atividade>()))
            .Callback<Atividade>(a =>
            {
                a.AtividadeId = 77;
                criada = a;
            })
            .Returns(Task.CompletedTask);

        var solicitacaoRepository = new Mock<ISolicitacaoProgressaoRepository>();
        solicitacaoRepository
            .Setup(r => r.ObterParaEdicaoPorIdEUsuarioAsync(solicitacao.SolicitacaoProgressaoId, solicitacao.UsuarioId))
            .ReturnsAsync(solicitacao);

        var service = new AtividadeService(atividadeRepository.Object, solicitacaoRepository.Object);

        var resultado = await service.CriarAsync(solicitacao.UsuarioId, new AtividadeFormViewModel
        {
            SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId,
            SubTipoAtividadeId = subtipo.SubtipoAtividadeId,
            Titulo = "  Disciplina ministrada  ",
            Descricao = "  Comprovacao manual  ",
            DataInicio = new DateTime(2026, 1, 10),
            DataTermino = new DateTime(2026, 2, 10),
            Quantidade = 3
        });

        resultado.Sucesso.Should().BeTrue();
        resultado.AtividadeId.Should().Be(77);
        criada.Should().NotBeNull();
        criada!.Titulo.Should().Be("Disciplina ministrada");
        criada.Descricao.Should().Be("Comprovacao manual");
        criada.PontuacaoCalculada.Should().Be(36);
        criada.Status.Should().Be("Declarada");
        atividadeRepository.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_ComSolicitacaoFinalizada_DeveBloquearCadastro()
    {
        var solicitacao = TestDataFactory.Solicitacao(statusNome: StatusSolicitacaoNomes.Encerrada);
        var atividadeRepository = new Mock<IAtividadeRepository>();
        var solicitacaoRepository = new Mock<ISolicitacaoProgressaoRepository>();
        solicitacaoRepository
            .Setup(r => r.ObterParaEdicaoPorIdEUsuarioAsync(solicitacao.SolicitacaoProgressaoId, solicitacao.UsuarioId))
            .ReturnsAsync(solicitacao);

        var service = new AtividadeService(atividadeRepository.Object, solicitacaoRepository.Object);

        var resultado = await service.CriarAsync(solicitacao.UsuarioId, new AtividadeFormViewModel
        {
            SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId,
            SubTipoAtividadeId = 1,
            Titulo = "Atividade",
            DataInicio = DateTime.UtcNow,
            Quantidade = 1
        });

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Contain("bloqueadas");
        atividadeRepository.Verify(r => r.AdicionarAsync(It.IsAny<Atividade>()), Times.Never);
        atividadeRepository.Verify(r => r.SalvarAlteracoesAsync(), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_ComSolicitacaoEmAjustes_DeveReabrirSolicitacaoELimparParecer()
    {
        var solicitacao = TestDataFactory.Solicitacao(statusNome: StatusSolicitacaoNomes.AjustesSolicitados);
        var statusEmProcessamento = TestDataFactory.Status(1, StatusSolicitacaoNomes.EmProcessamento);
        var subtipo = TestDataFactory.SubtipoAtividade(pontos: 10);
        var atividade = TestDataFactory.Atividade(id: 7);
        atividade.SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId;
        atividade.SolicitacaoProgressao = solicitacao;
        atividade.Status = "Rejeitada";
        atividade.ParecerAvaliacao = "Comprovante insuficiente.";
        atividade.DataAvaliacao = DateTime.UtcNow.AddDays(-1);

        var atividadeRepository = new Mock<IAtividadeRepository>();
        atividadeRepository.Setup(r => r.ObterPorIdEUsuarioAsync(atividade.AtividadeId, solicitacao.UsuarioId)).ReturnsAsync(atividade);
        atividadeRepository.Setup(r => r.ObterSubtipoAsync(subtipo.SubtipoAtividadeId)).ReturnsAsync(subtipo);

        var solicitacaoRepository = new Mock<ISolicitacaoProgressaoRepository>();
        solicitacaoRepository.Setup(r => r.ObterStatusPorNomeAsync(StatusSolicitacaoNomes.EmProcessamento)).ReturnsAsync(statusEmProcessamento);

        var service = new AtividadeService(atividadeRepository.Object, solicitacaoRepository.Object);

        var resultado = await service.AtualizarAsync(solicitacao.UsuarioId, new AtividadeFormViewModel
        {
            AtividadeId = atividade.AtividadeId,
            SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId,
            SubTipoAtividadeId = subtipo.SubtipoAtividadeId,
            Titulo = "Atividade corrigida",
            DataInicio = new DateTime(2026, 3, 1),
            Quantidade = 2
        });

        resultado.Sucesso.Should().BeTrue();
        solicitacao.StatusSolicitacaoId.Should().Be(statusEmProcessamento.StatusSolicitacaoId);
        solicitacao.DataFechamento.Should().BeNull();
        atividade.Status.Should().Be("Declarada");
        atividade.ParecerAvaliacao.Should().BeNull();
        atividade.DataAvaliacao.Should().BeNull();
        atividade.PontuacaoCalculada.Should().Be(20);
        atividadeRepository.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task ExcluirAsync_ComSolicitacaoFinalizada_DeveRetornarFalse()
    {
        var solicitacao = TestDataFactory.Solicitacao(statusNome: StatusSolicitacaoNomes.Aprovada);
        var atividade = TestDataFactory.Atividade();
        atividade.SolicitacaoProgressao = solicitacao;

        var atividadeRepository = new Mock<IAtividadeRepository>();
        atividadeRepository.Setup(r => r.ObterPorIdEUsuarioAsync(atividade.AtividadeId, solicitacao.UsuarioId)).ReturnsAsync(atividade);

        var service = new AtividadeService(atividadeRepository.Object, Mock.Of<ISolicitacaoProgressaoRepository>());

        var resultado = await service.ExcluirAsync(solicitacao.UsuarioId, atividade.AtividadeId);

        resultado.Should().BeFalse();
        atividadeRepository.Verify(r => r.Remover(It.IsAny<Atividade>()), Times.Never);
        atividadeRepository.Verify(r => r.SalvarAlteracoesAsync(), Times.Never);
    }
}
