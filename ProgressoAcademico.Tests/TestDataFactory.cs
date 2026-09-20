using ProgressoAcademico.Models;

namespace ProgressoAcademico.Tests;

internal static class TestDataFactory
{
    public static Usuario Professor(
        int usuarioId = 10,
        string perfil = PerfisAcesso.Professor,
        bool ativo = true,
        DateTime? dataUltimoProgresso = null)
    {
        return new Usuario
        {
            UsuarioId = usuarioId,
            Nome = "Professor Teste",
            Email = $"professor{usuarioId}@ufabc.edu.br",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Professor@123456"),
            PerfilAcesso = perfil,
            PodeRevisar = perfil == PerfisAcesso.Revisor,
            Ativo = ativo,
            DataCriacao = DateTime.UtcNow.AddYears(-1),
            DataUltimoProgresso = dataUltimoProgresso ?? DateTime.UtcNow.AddMonths(-25)
        };
    }

    public static StatusSolicitacao Status(int id, string nome)
    {
        return new StatusSolicitacao
        {
            StatusSolicitacaoId = id,
            Nome = nome
        };
    }

    public static TipoProgresso TipoProgressao(int id = 1, string nome = "Progressao funcional")
    {
        return new TipoProgresso
        {
            TipoProgressoId = id,
            Nome = nome,
            Descricao = "Tipo de progressao usado em testes."
        };
    }

    public static Nivel Nivel(int id, string codigo, int ordem)
    {
        return new Nivel
        {
            NivelId = id,
            Codigo = codigo,
            Classe = "Classe Teste",
            Ordem = ordem,
            Ativo = true
        };
    }

    public static TipoAtividade TipoAtividade(int id = 1)
    {
        return new TipoAtividade
        {
            TipoAtividadeId = id,
            Nome = "Ensino"
        };
    }

    public static SubTipoAtividade SubtipoAtividade(int id = 1, int pontos = 10)
    {
        var tipo = TipoAtividade();
        return new SubTipoAtividade
        {
            SubtipoAtividadeId = id,
            TipoAtividadeId = tipo.TipoAtividadeId,
            TipoAtividade = tipo,
            Nome = "Disciplina ministrada",
            Pontos = pontos
        };
    }

    public static TipoDocumento TipoDocumento(int id = 1)
    {
        return new TipoDocumento
        {
            TipoDocumentoId = id,
            Tipo = "Comprovante academico"
        };
    }

    public static Documento Documento(
        int id = 1,
        string origem = OrigensDocumento.ComprovatorioProfessor)
    {
        var tipoDocumento = TipoDocumento();
        return new Documento
        {
            DocumentoId = id,
            TipoDocumentoId = tipoDocumento.TipoDocumentoId,
            TipoDocumento = tipoDocumento,
            NomeArquivo = "comprovante.pdf",
            ContentType = "application/pdf",
            TamanhoBytes = 128,
            DataUpload = DateTime.UtcNow.AddDays(-1),
            OrigemDocumento = origem,
            HashSha256 = new string('a', 64),
            Arquivo = new byte[] { 1, 2, 3 }
        };
    }

    public static Atividade Atividade(int id = 1)
    {
        var tipoAtividade = TipoAtividade();
        var subtipo = SubtipoAtividade();
        return new Atividade
        {
            AtividadeId = id,
            TipoAtividadeId = tipoAtividade.TipoAtividadeId,
            TipoAtividade = tipoAtividade,
            SubTipoAtividadeId = subtipo.SubtipoAtividadeId,
            SubTipoAtividade = subtipo,
            Titulo = "Aula ministrada",
            DataInicio = DateTime.UtcNow.AddMonths(-4),
            DataTermino = DateTime.UtcNow.AddMonths(-3),
            Quantidade = 2,
            PontuacaoCalculada = 20,
            Status = "Declarada"
        };
    }

    public static SolicitacaoProgressao Solicitacao(
        int id = 50,
        int usuarioId = 10,
        string statusNome = StatusSolicitacaoNomes.EmProcessamento,
        bool comObservacao = true,
        bool comAtividade = false,
        bool comDocumentoComprovatorio = false,
        bool comDocumentoOficial = false,
        int nivelOrigemOrdem = 1,
        int nivelDestinoOrdem = 2,
        DateTime? dataUltimoProgresso = null)
    {
        var status = Status(1, statusNome);
        var tipoProgressao = TipoProgressao();
        var nivelOrigem = Nivel(1, "A1", nivelOrigemOrdem);
        var nivelDestino = Nivel(2, "A2", nivelDestinoOrdem);

        var solicitacao = new SolicitacaoProgressao
        {
            SolicitacaoProgressaoId = id,
            UsuarioId = usuarioId,
            Usuario = Professor(usuarioId, dataUltimoProgresso: dataUltimoProgresso),
            StatusSolicitacaoId = status.StatusSolicitacaoId,
            StatusSolicitacao = status,
            TipoProgressoId = tipoProgressao.TipoProgressoId,
            TipoProgresso = tipoProgressao,
            NivelOrigemId = nivelOrigem.NivelId,
            NivelOrigem = nivelOrigem,
            NivelDestinoId = nivelDestino.NivelId,
            NivelDestino = nivelDestino,
            DataCriacao = DateTime.UtcNow.AddDays(-10),
            Observacao = comObservacao ? "Solicitacao de teste" : null,
            Atividades = new List<Atividade>(),
            Documentos = new List<Documento>()
        };

        if (StatusSolicitacaoNomes.EhFinal(statusNome))
            solicitacao.DataFechamento = DateTime.UtcNow.AddDays(-1);

        if (comAtividade)
        {
            var atividade = Atividade();
            atividade.SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId;
            atividade.SolicitacaoProgressao = solicitacao;
            solicitacao.Atividades.Add(atividade);
        }

        if (comDocumentoComprovatorio)
        {
            var documento = Documento(origem: OrigensDocumento.ComprovatorioProfessor);
            documento.SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId;
            documento.SolicitacaoProgressao = solicitacao;
            solicitacao.Documentos.Add(documento);
        }

        if (comDocumentoOficial)
        {
            var documento = Documento(id: 2, origem: OrigensDocumento.DocumentoOficialUfabc);
            documento.SolicitacaoProgressaoId = solicitacao.SolicitacaoProgressaoId;
            documento.SolicitacaoProgressao = solicitacao;
            solicitacao.Documentos.Add(documento);
        }

        return solicitacao;
    }
}
