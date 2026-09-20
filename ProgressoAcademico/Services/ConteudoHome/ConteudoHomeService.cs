using ProgressoAcademico.Application.DTOs.ConteudoHome;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models.ViewModels.ConteudoHome;

namespace ProgressoAcademico.Services.ConteudoHome;

public class ConteudoHomeService : IConteudoHomeService
{
    private readonly IConteudoHomeRepository _repository;

    public ConteudoHomeService(IConteudoHomeRepository repository)
    {
        _repository = repository;
    }

    public async Task<ConteudoHomeDto> ObterAsync()
    {
        var conteudo = await _repository.ObterAsync() ?? CriarConteudoPadrao();
        return MapearDto(conteudo);
    }

    public async Task<ConteudoHomeEdicaoViewModel?> ObterParaEdicaoAsync()
    {
        var conteudo = await _repository.ObterAsync();

        if (conteudo == null)
            return null;

        return new ConteudoHomeEdicaoViewModel
        {
            TituloPrincipal = conteudo.TituloPrincipal,
            ResumoProjeto = conteudo.ResumoProjeto,
            TituloDestaque = conteudo.TituloDestaque,
            ItensDestaque = conteudo.ItensDestaque,
            AvisoEscopo = conteudo.AvisoEscopo,
            DescricaoProfessor = conteudo.DescricaoProfessor,
            RecursosProfessor = conteudo.RecursosProfessor,
            DescricaoAdministrador = conteudo.DescricaoAdministrador,
            RecursosAdministrador = conteudo.RecursosAdministrador
        };
    }

    public async Task<bool> AtualizarAsync(ConteudoHomeEdicaoViewModel model, int? usuarioId)
    {
        var conteudo = await _repository.ObterParaAtualizacaoAsync();

        if (conteudo == null)
            return false;

        conteudo.TituloPrincipal = model.TituloPrincipal.Trim();
        conteudo.ResumoProjeto = model.ResumoProjeto.Trim();
        conteudo.TituloDestaque = model.TituloDestaque.Trim();
        conteudo.ItensDestaque = NormalizarLista(model.ItensDestaque);
        conteudo.AvisoEscopo = model.AvisoEscopo.Trim();
        conteudo.DescricaoProfessor = model.DescricaoProfessor.Trim();
        conteudo.RecursosProfessor = NormalizarLista(model.RecursosProfessor);
        conteudo.DescricaoAdministrador = model.DescricaoAdministrador.Trim();
        conteudo.RecursosAdministrador = NormalizarLista(model.RecursosAdministrador);
        conteudo.DataAtualizacao = DateTime.UtcNow;
        conteudo.AtualizadoPorUsuarioId = usuarioId;

        await _repository.SalvarAlteracoesAsync();
        return true;
    }

    private static ConteudoHomeDto MapearDto(ProgressoAcademico.Models.ConteudoHome conteudo)
    {
        return new ConteudoHomeDto
        {
            TituloPrincipal = conteudo.TituloPrincipal,
            ResumoProjeto = conteudo.ResumoProjeto,
            TituloDestaque = conteudo.TituloDestaque,
            ItensDestaque = SepararLista(conteudo.ItensDestaque),
            AvisoEscopo = conteudo.AvisoEscopo,
            DescricaoProfessor = conteudo.DescricaoProfessor,
            RecursosProfessor = SepararLista(conteudo.RecursosProfessor),
            DescricaoAdministrador = conteudo.DescricaoAdministrador,
            RecursosAdministrador = SepararLista(conteudo.RecursosAdministrador),
            DataAtualizacao = conteudo.DataAtualizacao
        };
    }

    private static string NormalizarLista(string valor)
    {
        return string.Join(Environment.NewLine, SepararLista(valor));
    }

    private static IReadOnlyCollection<string> SepararLista(string valor)
    {
        return valor
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .ToArray();
    }

    private static ProgressoAcademico.Models.ConteudoHome CriarConteudoPadrao()
    {
        return new ProgressoAcademico.Models.ConteudoHome
        {
            ConteudoHomeId = 1,
            TituloPrincipal = "Sistema integrado para apoio à progressão acadêmica docente.",
            ResumoProjeto = "Aplicação web funcional desenvolvida para organizar solicitações, atividades, documentos, revisões técnicas e decisão administrativa no processo de progressão acadêmica dos professores da Universidade Federal do ABC.",
            TituloDestaque = "Fluxo estruturado para professor, revisor e administrador.",
            ItensDestaque = "Solicitações e atividades centralizadas\nMúltiplos documentos por atividade\nRevisão técnica por professores avaliadores\nDecisão administrativa com parecer registrado",
            AvisoEscopo = "Sistema acadêmico funcional de apoio. Não substitui normas, sistemas oficiais ou atos administrativos da UFABC.",
            DescricaoProfessor = "Prepara e acompanha a própria solicitação, registra atividades e vincula documentos comprobatórios.",
            RecursosProfessor = "Cria e acompanha solicitações\nCadastra atividades de ensino, pesquisa, extensão e gestão\nAnexa múltiplos comprovantes por atividade\nConsulta histórico, status e pendências",
            DescricaoAdministrador = "Coordena a análise das solicitações, atribui revisores, consulta pareceres e registra a decisão final.",
            RecursosAdministrador = "Atribui um ou mais revisores por solicitação\nConsulta pareceres técnicos e documentos\nSolicita ajustes quando necessário\nRegistra aprovação, rejeição ou encerramento",
            DataAtualizacao = DateTime.UtcNow
        };
    }
}
