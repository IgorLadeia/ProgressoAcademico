using FluentAssertions;
using Moq;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.Admin;
using ProgressoAcademico.Services.Revisor;

namespace ProgressoAcademico.Tests.Services;

public class RevisorSolicitacaoServiceTests
{
    [Fact]
    public async Task EncaminharParaAdmAsync_ComParecer_DeveRegistrarParecerTecnicoEStatus()
    {
        var solicitacao = TestDataFactory.Solicitacao(statusNome: StatusSolicitacaoNomes.EmRevisao);
        var atribuicao = new SolicitacaoRevisor
        {
            SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId,
            SolicitacaoProgressao = solicitacao,
            RevisorUsuarioId = 44,
            RevisorUsuario = TestDataFactory.Professor(usuarioId: 44, perfil: PerfisAcesso.Revisor),
            DataAtribuicao = DateTime.UtcNow.AddDays(-1),
            StatusRevisao = StatusRevisaoNomes.EmRevisao
        };
        solicitacao.Revisores.Add(atribuicao);
        var statusDecisaoFinal = TestDataFactory.Status(9, StatusSolicitacaoNomes.AguardandoDecisaoFinal);

        var repository = new Mock<IRevisorSolicitacaoRepository>();
        repository
            .Setup(r => r.ObterAtribuicaoAsync(solicitacao.SolicitacaoProgressaoId, 44))
            .ReturnsAsync(atribuicao);
        repository
            .Setup(r => r.ObterStatusPorNomeAsync(StatusSolicitacaoNomes.AguardandoDecisaoFinal))
            .ReturnsAsync(statusDecisaoFinal);

        var service = new RevisorSolicitacaoService(repository.Object);

        var resultado = await service.EncaminharParaAdmAsync(
            solicitacao.SolicitacaoProgressaoId,
            revisorUsuarioId: 44,
            parecer: "  Atividades conferidas e aptas para decisao final.  ");

        resultado.Sucesso.Should().BeTrue();
        atribuicao.Parecer.Should().Be("Atividades conferidas e aptas para decisao final.");
        atribuicao.DataParecer.Should().NotBeNull();
        atribuicao.StatusRevisao.Should().Be(StatusRevisaoNomes.EncaminhadaAoAdm);
        solicitacao.StatusSolicitacaoId.Should().Be(statusDecisaoFinal.StatusSolicitacaoId);
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task AvaliarAtividadeAsync_ForaDaAtribuicao_DeveBloquearRegistro()
    {
        var repository = new Mock<IRevisorSolicitacaoRepository>();
        var service = new RevisorSolicitacaoService(repository.Object);

        var resultado = await service.AvaliarAtividadeAsync(new AdminAvaliarAtividadeViewModel
        {
            SolicitacaoProgressaoId = 10,
            AtividadeId = 7,
            Resultado = "Aceita",
            Parecer = "Comprovante aceito."
        }, revisorUsuarioId: 44);

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Contain("atribuidas");
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Never);
    }

    [Fact]
    public async Task AvaliarAtividadeAsync_ComAtribuicao_DeveRegistrarParecerDoRevisorSemAlterarDecisaoFinal()
    {
        var atividade = new Atividade { AtividadeId = 7, SolicitacaoProgressaoId = 10, Status = "Pendente" };
        var atribuicao = new SolicitacaoRevisor
        {
            SolicitacaoRevisorId = 12,
            SolicitacaoProgressaoId = atividade.SolicitacaoProgressaoId,
            RevisorUsuarioId = 44,
            DataAtribuicao = DateTime.UtcNow.AddDays(-1),
            StatusRevisao = StatusRevisaoNomes.EmRevisao
        };

        AtividadeAvaliacaoRevisor? avaliacaoCriada = null;
        var repository = new Mock<IRevisorSolicitacaoRepository>();
        repository
            .Setup(r => r.ObterAtividadeAsync(atividade.AtividadeId, atividade.SolicitacaoProgressaoId, 44))
            .ReturnsAsync(atividade);
        repository
            .Setup(r => r.ObterAtribuicaoAsync(atividade.SolicitacaoProgressaoId, 44))
            .ReturnsAsync(atribuicao);
        repository
            .Setup(r => r.ObterAvaliacaoAtividadeAsync(atividade.AtividadeId, atribuicao.SolicitacaoRevisorId))
            .ReturnsAsync((AtividadeAvaliacaoRevisor?)null);
        repository
            .Setup(r => r.AdicionarAvaliacaoAtividadeAsync(It.IsAny<AtividadeAvaliacaoRevisor>()))
            .Callback<AtividadeAvaliacaoRevisor>(a => avaliacaoCriada = a)
            .Returns(Task.CompletedTask);

        var service = new RevisorSolicitacaoService(repository.Object);

        var resultado = await service.AvaliarAtividadeAsync(new AdminAvaliarAtividadeViewModel
        {
            SolicitacaoProgressaoId = atividade.SolicitacaoProgressaoId,
            AtividadeId = atividade.AtividadeId,
            Resultado = "Aceita",
            Parecer = "  Comprovante suficiente para a atividade.  "
        }, revisorUsuarioId: 44);

        resultado.Sucesso.Should().BeTrue();
        atividade.Status.Should().Be("Pendente");
        atividade.ParecerAvaliacao.Should().BeNull();
        avaliacaoCriada.Should().NotBeNull();
        avaliacaoCriada!.Resultado.Should().Be("Aceita");
        avaliacaoCriada.Parecer.Should().Be("Comprovante suficiente para a atividade.");
        avaliacaoCriada.AtividadeId.Should().Be(atividade.AtividadeId);
        avaliacaoCriada.SolicitacaoRevisorId.Should().Be(atribuicao.SolicitacaoRevisorId);
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }
}
