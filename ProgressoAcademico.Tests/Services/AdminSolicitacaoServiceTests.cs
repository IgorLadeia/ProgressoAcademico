using FluentAssertions;
using Moq;
using ProgressoAcademico.Application.DTOs.Admin;
using ProgressoAcademico.Application.DTOs.Elegibilidade;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;
using ProgressoAcademico.Models.ViewModels.Admin;
using ProgressoAcademico.Services.Admin;
using ProgressoAcademico.Services.Elegibilidade;

namespace ProgressoAcademico.Tests.Services;

public class AdminSolicitacaoServiceTests
{
    [Fact]
    public async Task AprovarAsync_SemParecer_DeveBloquearDecisao()
    {
        var repository = new Mock<IAdminSolicitacaoRepository>();
        var service = new AdminSolicitacaoService(repository.Object, Mock.Of<IElegibilidadeService>());

        var resultado = await service.AprovarAsync(10, adminId: 1, parecer: "   ");

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Contain("obrigatorio");
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Never);
    }

    [Fact]
    public async Task AprovarAsync_ComParecer_DeveRegistrarStatusFinalERevisor()
    {
        var solicitacao = TestDataFactory.Solicitacao(statusNome: StatusSolicitacaoNomes.AguardandoDecisaoFinal);
        var statusAprovada = TestDataFactory.Status(2, StatusSolicitacaoNomes.Aprovada);
        solicitacao.Usuario.VinculosInstitucionais.Add(new VinculoInstitucional
        {
            UsuarioId = solicitacao.UsuarioId,
            Usuario = solicitacao.Usuario,
            InstituicaoId = 1,
            TipoVinculoId = 1,
            NivelId = solicitacao.NivelOrigemId,
            DataIngressoInstituicao = DateTime.UtcNow.AddYears(-3),
            Ativo = true
        });

        var repository = new Mock<IAdminSolicitacaoRepository>();
        repository.Setup(r => r.ObterDetalheAsync(solicitacao.SolicitacaoProgressaoId)).ReturnsAsync(solicitacao);
        repository.Setup(r => r.ObterStatusPorNomeAsync(StatusSolicitacaoNomes.Aprovada)).ReturnsAsync(statusAprovada);

        var service = new AdminSolicitacaoService(repository.Object, Mock.Of<IElegibilidadeService>());

        var resultado = await service.AprovarAsync(solicitacao.SolicitacaoProgressaoId, adminId: 99, parecer: "  Aprovado pela banca  ");

        resultado.Sucesso.Should().BeTrue();
        solicitacao.StatusSolicitacaoId.Should().Be(statusAprovada.StatusSolicitacaoId);
        solicitacao.ParecerFinal.Should().Be("Aprovado pela banca");
        solicitacao.RevisadoPorUsuarioId.Should().Be(99);
        solicitacao.DataRevisao.Should().NotBeNull();
        solicitacao.DataFechamento.Should().NotBeNull();
        solicitacao.Usuario.DataUltimoProgresso.Should().Be(solicitacao.DataFechamento);
        solicitacao.Usuario.VinculosInstitucionais.Single().NivelId.Should().Be(solicitacao.NivelDestinoId);
        solicitacao.Usuario.VinculosInstitucionais.Single().DataUltimaProgressao.Should().Be(solicitacao.DataFechamento);
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task SolicitarAjustesAsync_ComParecer_DeveLiberarSolicitacaoParaCorrecao()
    {
        var solicitacao = TestDataFactory.Solicitacao(statusNome: StatusSolicitacaoNomes.EmRevisao);
        var statusAjustes = TestDataFactory.Status(5, StatusSolicitacaoNomes.AjustesSolicitados);

        var repository = new Mock<IAdminSolicitacaoRepository>();
        repository.Setup(r => r.ObterDetalheAsync(solicitacao.SolicitacaoProgressaoId)).ReturnsAsync(solicitacao);
        repository.Setup(r => r.ObterStatusPorNomeAsync(StatusSolicitacaoNomes.AjustesSolicitados)).ReturnsAsync(statusAjustes);

        var service = new AdminSolicitacaoService(repository.Object, Mock.Of<IElegibilidadeService>());

        var resultado = await service.SolicitarAjustesAsync(
            solicitacao.SolicitacaoProgressaoId,
            adminId: 99,
            parecer: "Corrija as atividades rejeitadas e reenvie.");

        resultado.Sucesso.Should().BeTrue();
        solicitacao.StatusSolicitacaoId.Should().Be(statusAjustes.StatusSolicitacaoId);
        solicitacao.ParecerFinal.Should().Be("Corrija as atividades rejeitadas e reenvie.");
        solicitacao.RevisadoPorUsuarioId.Should().Be(99);
        solicitacao.DataRevisao.Should().NotBeNull();
        solicitacao.DataFechamento.Should().BeNull();
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }


    [Fact]
    public async Task CorrigirAsync_ComNivelOrigemIgualDestino_DeveBloquearCorrecao()
    {
        var repository = new Mock<IAdminSolicitacaoRepository>();
        var service = new AdminSolicitacaoService(repository.Object, Mock.Of<IElegibilidadeService>());

        var resultado = await service.CorrigirAsync(new AdminCorrigirSolicitacaoViewModel
        {
            SolicitacaoProgressaoId = 10,
            TipoProgressoId = 1,
            NivelOrigemId = 2,
            NivelDestinoId = 2,
            Status = StatusSolicitacaoNomes.EmProcessamento
        });

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Contain("diferentes");
        repository.Verify(r => r.ObterDetalheAsync(It.IsAny<int>()), Times.Never);
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Never);
    }

    [Fact]
    public async Task ObterDashboardAsync_DeveConsolidarMetricasAdministrativas()
    {
        var solicitacoes = new List<SolicitacaoProgressao>
        {
            TestDataFactory.Solicitacao(id: 1, statusNome: StatusSolicitacaoNomes.EmProcessamento, comAtividade: true),
            TestDataFactory.Solicitacao(id: 2, statusNome: StatusSolicitacaoNomes.Aprovada, comDocumentoComprovatorio: true),
            TestDataFactory.Solicitacao(id: 3, statusNome: StatusSolicitacaoNomes.Negada)
        };

        var repository = new Mock<IAdminSolicitacaoRepository>();
        repository.Setup(r => r.ListarAsync(It.IsAny<AdminSolicitacaoFiltroDto>())).ReturnsAsync(solicitacoes);
        repository.Setup(r => r.ContarProfessoresAsync()).ReturnsAsync(5);
        repository.Setup(r => r.ContarRevisoresAsync()).ReturnsAsync(3);
        repository.Setup(r => r.ContarDocumentosAsync()).ReturnsAsync(3);
        repository.Setup(r => r.ContarAtividadesAsync()).ReturnsAsync(7);

        var service = new AdminSolicitacaoService(repository.Object, Mock.Of<IElegibilidadeService>());

        var dashboard = await service.ObterDashboardAsync();

        dashboard.TotalSolicitacoes.Should().Be(3);
        dashboard.EmProcessamento.Should().Be(1);
        dashboard.Aprovadas.Should().Be(1);
        dashboard.Negadas.Should().Be(1);
        dashboard.TotalProfessores.Should().Be(5);
        dashboard.TotalRevisores.Should().Be(3);
        dashboard.TotalDocumentos.Should().Be(3);
        dashboard.TotalAtividades.Should().Be(7);
        dashboard.Recentes.Should().HaveCount(3);
    }

    [Fact]
    public async Task AtribuirRevisorAsync_ComRevisorAtivo_DeveRegistrarAtribuicaoEStatusEmRevisao()
    {
        var solicitacao = TestDataFactory.Solicitacao(statusNome: StatusSolicitacaoNomes.AguardandoAtribuicao);
        var revisor = TestDataFactory.Professor(usuarioId: 25, perfil: PerfisAcesso.Revisor);
        var statusEmRevisao = TestDataFactory.Status(8, StatusSolicitacaoNomes.EmRevisao);

        var repository = new Mock<IAdminSolicitacaoRepository>();
        repository.Setup(r => r.ObterDetalheAsync(solicitacao.SolicitacaoProgressaoId)).ReturnsAsync(solicitacao);
        repository.Setup(r => r.ObterUsuariosAsync(It.IsAny<IReadOnlyCollection<int>>())).ReturnsAsync(new List<Usuario> { revisor });
        repository.Setup(r => r.ObterStatusPorNomeAsync(StatusSolicitacaoNomes.EmRevisao)).ReturnsAsync(statusEmRevisao);

        var service = new AdminSolicitacaoService(repository.Object, Mock.Of<IElegibilidadeService>());

        var resultado = await service.AtribuirRevisorAsync(new AdminAtribuirRevisorViewModel
        {
            SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId,
            RevisorUsuarioIds = new List<int> { revisor.UsuarioId }
        }, adminId: 99);

        resultado.Sucesso.Should().BeTrue();
        solicitacao.StatusSolicitacaoId.Should().Be(statusEmRevisao.StatusSolicitacaoId);
        repository.Verify(r => r.AdicionarSolicitacaoRevisorAsync(It.Is<SolicitacaoRevisor>(sr =>
            sr.SolicitacaoProgressaoId == solicitacao.SolicitacaoProgressaoId
            && sr.RevisorUsuarioId == revisor.UsuarioId
            && sr.AtribuidoPorUsuarioId == 99)), Times.Once);
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterDetalheAsync_DeveIncluirResultadoDeElegibilidade()
    {
        var solicitacao = TestDataFactory.Solicitacao(comAtividade: true);
        var elegibilidade = new ElegibilidadeResultadoDto
        {
            SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId,
            Status = "ElegivelComPendencias",
            PercentualElegibilidade = 80
        };

        var repository = new Mock<IAdminSolicitacaoRepository>();
        repository.Setup(r => r.ObterDetalheAsync(solicitacao.SolicitacaoProgressaoId)).ReturnsAsync(solicitacao);

        var elegibilidadeService = new Mock<IElegibilidadeService>();
        elegibilidadeService
            .Setup(s => s.AvaliarAsync(solicitacao.UsuarioId, solicitacao.SolicitacaoProgressaoId))
            .ReturnsAsync(elegibilidade);

        var service = new AdminSolicitacaoService(repository.Object, elegibilidadeService.Object);

        var detalhe = await service.ObterDetalheAsync(solicitacao.SolicitacaoProgressaoId);

        detalhe.Should().NotBeNull();
        detalhe!.Elegibilidade.Should().BeSameAs(elegibilidade);
    }

    [Fact]
    public async Task AvaliarAtividadeAsync_ComParecer_DeveRegistrarResultadoIndividual()
    {
        var atividade = new Atividade { AtividadeId = 7, SolicitacaoProgressaoId = 10, Status = "Declarada" };
        var repository = new Mock<IAdminSolicitacaoRepository>();
        repository.Setup(r => r.ObterAtividadeAsync(atividade.AtividadeId, atividade.SolicitacaoProgressaoId))
            .ReturnsAsync(atividade);
        var service = new AdminSolicitacaoService(repository.Object, Mock.Of<IElegibilidadeService>());

        var resultado = await service.AvaliarAtividadeAsync(new AdminAvaliarAtividadeViewModel
        {
            SolicitacaoProgressaoId = atividade.SolicitacaoProgressaoId,
            AtividadeId = atividade.AtividadeId,
            Resultado = "Rejeitada",
            Parecer = "  O comprovante não demonstra a atividade declarada.  "
        });

        resultado.Sucesso.Should().BeTrue();
        atividade.Status.Should().Be("Rejeitada");
        atividade.ParecerAvaliacao.Should().Be("O comprovante não demonstra a atividade declarada.");
        atividade.DataAvaliacao.Should().NotBeNull();
        repository.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }
}
