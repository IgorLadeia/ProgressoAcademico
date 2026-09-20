using ProgressoAcademico.Application.DTOs.Elegibilidade;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Models;
using System.Globalization;
using System.Text;

namespace ProgressoAcademico.Services.Elegibilidade;

public class ElegibilidadeService : IElegibilidadeService
{
    private const int IntersticioMinimoMeses = 24;
    private const decimal PontosMinimosEnsino = 40;
    private const decimal PontosMinimosPesquisa = 50;
    private const decimal PontosMinimosExtensao = 20;
    private const decimal PontosMinimosGestao = 20;

    private readonly ISolicitacaoProgressaoRepository _solicitacaoRepository;

    public ElegibilidadeService(ISolicitacaoProgressaoRepository solicitacaoRepository)
    {
        _solicitacaoRepository = solicitacaoRepository;
    }

    public async Task<ElegibilidadeResultadoDto?> AvaliarAsync(int usuarioId, int solicitacaoProgressaoId)
    {
        var solicitacao = await _solicitacaoRepository.ObterDetalhePorIdEUsuarioAsync(solicitacaoProgressaoId, usuarioId);

        if (solicitacao == null)
            return null;

        var requisitos = new List<RequisitoElegibilidadeDto>
        {
            AvaliarIntersticio(solicitacao),
            AvaliarClasseNivel(solicitacao),
            AvaliarDadosPessoais(solicitacao),
            AvaliarGrupoAtividade(solicitacao, "Ensino", PontosMinimosEnsino),
            AvaliarGrupoAtividade(solicitacao, "Pesquisa", PontosMinimosPesquisa),
            AvaliarGrupoAtividade(solicitacao, "Extensao", PontosMinimosExtensao),
            AvaliarGrupoAtividade(solicitacao, "Gestao", PontosMinimosGestao),
            AvaliarDocumentosComprobatorios(solicitacao),
            AvaliarDocumentoOficial(solicitacao),
            AvaliarPromocao(solicitacao)
        };

        var requisitosObrigatorios = requisitos.Where(r => r.Obrigatorio).ToList();
        var totalRequisitos = requisitosObrigatorios.Count;
        var requisitosAtendidos = requisitosObrigatorios.Count(r => r.Atendido);
        var pendencias = requisitos
            .Where(r => r.Obrigatorio && !r.Atendido)
            .Select(r => r.Mensagem)
            .ToList();

        return new ElegibilidadeResultadoDto
        {
            SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId,
            PercentualElegibilidade = CalcularPercentualElegibilidade(requisitosAtendidos, totalRequisitos),
            RequisitosAtendidos = requisitosAtendidos,
            TotalRequisitos = totalRequisitos,
            Status = DefinirStatus(requisitos),
            Requisitos = requisitos,
            Pendencias = pendencias
        };
    }

    private static RequisitoElegibilidadeDto AvaliarIntersticio(SolicitacaoProgressao solicitacao)
    {
        if (!solicitacao.Usuario.DataUltimoProgresso.HasValue)
        {
            return Pendente(
                "Intersticio",
                "Informe a data da ultima progressao para calcular o intersticio minimo.",
                0,
                IntersticioMinimoMeses,
                "meses",
                "Cadastrar/modificar dados",
                "ProfessorPerfil",
                "Editar");
        }

        var meses = ((DateTime.UtcNow.Year - solicitacao.Usuario.DataUltimoProgresso.Value.Year) * 12)
            + DateTime.UtcNow.Month
            - solicitacao.Usuario.DataUltimoProgresso.Value.Month;

        return meses >= IntersticioMinimoMeses
            ? Atendido("Intersticio", $"Intersticio: precisa de {IntersticioMinimoMeses} meses e voce tem {meses}.", meses, IntersticioMinimoMeses, "meses")
            : Pendente("Intersticio", $"Intersticio: precisa de {IntersticioMinimoMeses} meses e voce tem {meses}.", meses, IntersticioMinimoMeses, "meses");
    }

    private static RequisitoElegibilidadeDto AvaliarClasseNivel(SolicitacaoProgressao solicitacao)
    {
        return solicitacao.NivelDestino.Ordem > solicitacao.NivelOrigem.Ordem
            ? Atendido("Classe e nivel", "Classe/nivel destino compativel com o avanco solicitado.", solicitacao.NivelDestino.Ordem, solicitacao.NivelOrigem.Ordem + 1, "ordem")
            : Pendente("Classe e nivel", "O nivel destino precisa ser posterior ao nivel de origem.", solicitacao.NivelDestino.Ordem, solicitacao.NivelOrigem.Ordem + 1, "ordem");
    }

    private static RequisitoElegibilidadeDto AvaliarDadosPessoais(SolicitacaoProgressao solicitacao)
    {
        var vinculoAtivo = solicitacao.Usuario.VinculosInstitucionais.FirstOrDefault(v => v.Ativo);
        var camposPreenchidos = 0;

        if (!string.IsNullOrWhiteSpace(solicitacao.Usuario.Nome))
            camposPreenchidos++;

        if (!string.IsNullOrWhiteSpace(solicitacao.Usuario.Email))
            camposPreenchidos++;

        if (solicitacao.Usuario.DataUltimoProgresso.HasValue)
            camposPreenchidos++;

        if (vinculoAtivo?.InstituicaoId > 0)
            camposPreenchidos++;

        if (vinculoAtivo?.TipoVinculoId > 0)
            camposPreenchidos++;

        if (vinculoAtivo?.NivelId > 0)
            camposPreenchidos++;

        if (vinculoAtivo?.DataIngressoInstituicao != default)
            camposPreenchidos++;

        return camposPreenchidos == 7
            ? Atendido("Informacoes pessoais e funcionais", "Dados pessoais, vinculo, nivel atual e datas funcionais preenchidos.", camposPreenchidos, 7, "campos")
            : Pendente(
                "Informacoes pessoais e funcionais",
                $"Dados pessoais/funcionais incompletos. Preenchidos {camposPreenchidos} de 7 campos obrigatorios.",
                camposPreenchidos,
                7,
                "campos",
                "Cadastrar/modificar dados",
                "ProfessorPerfil",
                "Editar");
    }

    private static RequisitoElegibilidadeDto AvaliarGrupoAtividade(
        SolicitacaoProgressao solicitacao,
        string grupo,
        decimal minimo)
    {
        var pontos = solicitacao.Atividades
            .Where(a => a.TipoAtividade != null && NormalizarTexto(a.TipoAtividade.Nome) == NormalizarTexto(grupo))
            .Sum(a => a.PontuacaoCalculada);

        var mensagem = $"{grupo}: precisa de {minimo:0} pontos e voce tem {pontos:0.##}.";

        return pontos >= minimo
            ? Atendido(grupo, mensagem, pontos, minimo, "pontos")
            : Pendente(grupo, mensagem, pontos, minimo, "pontos");
    }

    private static RequisitoElegibilidadeDto AvaliarDocumentosComprobatorios(SolicitacaoProgressao solicitacao)
    {
        return solicitacao.Documentos.Any(d => d.OrigemDocumento == OrigensDocumento.ComprovatorioProfessor)
            ? Atendido("Comprovantes", "Ha documentos comprobatorios anexados pelo professor.", 1, 1, "item")
            : Pendente("Comprovantes", "Anexe comprovantes das atividades declaradas, como PDFs ou prints exportados em PDF.", 0, 1, "item");
    }

    private static RequisitoElegibilidadeDto AvaliarDocumentoOficial(SolicitacaoProgressao solicitacao)
    {
        return solicitacao.Documentos.Any(d => d.OrigemDocumento == OrigensDocumento.DocumentoOficialUfabc)
            ? Opcional("Documento oficial UFABC", "Documento oficial anexado. A leitura automatica podera acelerar o cadastro das atividades.", true, 1, 1, "item")
            : Opcional("Documento oficial UFABC", "Documento oficial da universidade ainda nao anexado. Este envio e opcional e servira apenas como apoio para importacao futura.", false, 0, 1, "item");
    }

    private static RequisitoElegibilidadeDto AvaliarPromocao(SolicitacaoProgressao solicitacao)
    {
        if (!solicitacao.TipoProgresso.Nome.Contains("Promocao", StringComparison.OrdinalIgnoreCase)
            && !solicitacao.TipoProgresso.Nome.Contains("Promo", StringComparison.OrdinalIgnoreCase))
        {
            return Atendido("Regra de promocao", "Solicitacao nao classificada como promocao de classe.", 1, 1, "item");
        }

        return Pendente(
            "Regra de promocao",
            "Promocoes exigem conferencia de requisitos especificos, como titulacao e regras da CPPD/UFABC. Nesta etapa, marque como pendencia para revisao.",
            0,
            1,
            "item");
    }

    private static string DefinirStatus(IReadOnlyCollection<RequisitoElegibilidadeDto> requisitos)
    {
        var bloqueadores = requisitos
            .Where(r => r.Obrigatorio && !r.Atendido)
            .Any(r => r.Nome is "Intersticio" or "Classe e nivel");

        if (bloqueadores)
            return "NaoElegivel";

        return requisitos.Where(r => r.Obrigatorio).All(r => r.Atendido)
            ? "Elegivel"
            : "ElegivelComPendencias";
    }

    private static int CalcularPercentualElegibilidade(int requisitosAtendidos, int totalRequisitos)
    {
        if (totalRequisitos == 0)
            return 0;

        return (int)Math.Round(requisitosAtendidos * 100m / totalRequisitos, MidpointRounding.AwayFromZero);
    }

    private static RequisitoElegibilidadeDto Atendido(
        string nome,
        string mensagem,
        decimal valorAtual = 1,
        decimal valorMinimo = 1,
        string unidade = "")
    {
        return new RequisitoElegibilidadeDto
        {
            Nome = nome,
            Atendido = true,
            Obrigatorio = true,
            Mensagem = mensagem,
            ValorAtual = valorAtual,
            ValorMinimo = valorMinimo,
            Unidade = unidade
        };
    }

    private static RequisitoElegibilidadeDto Pendente(
        string nome,
        string mensagem,
        decimal valorAtual = 0,
        decimal valorMinimo = 1,
        string unidade = "",
        string? acaoTexto = null,
        string? acaoController = null,
        string? acaoAction = null)
    {
        return new RequisitoElegibilidadeDto
        {
            Nome = nome,
            Atendido = false,
            Obrigatorio = true,
            Mensagem = mensagem,
            ValorAtual = valorAtual,
            ValorMinimo = valorMinimo,
            Unidade = unidade,
            AcaoTexto = acaoTexto,
            AcaoController = acaoController,
            AcaoAction = acaoAction
        };
    }

    private static RequisitoElegibilidadeDto Opcional(
        string nome,
        string mensagem,
        bool atendido,
        decimal valorAtual,
        decimal valorMinimo,
        string unidade)
    {
        return new RequisitoElegibilidadeDto
        {
            Nome = nome,
            Atendido = atendido,
            Obrigatorio = false,
            Mensagem = mensagem,
            ValorAtual = valorAtual,
            ValorMinimo = valorMinimo,
            Unidade = unidade
        };
    }

    private static string NormalizarTexto(string texto)
    {
        var normalized = texto.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                builder.Append(character);
        }

        return builder
            .ToString()
            .Normalize(NormalizationForm.FormC)
            .Trim()
            .ToUpperInvariant();
    }
}
