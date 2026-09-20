namespace ProgressoAcademico.Models;

public static class StatusSolicitacaoNomes
{
    public const string AguardandoAtribuicao = "Aguardando Atribuicao";
    public const string EmProcessamento = "Em Processamento";
    public const string EmRevisao = "Em Revisao";
    public const string AguardandoDecisaoFinal = "Aguardando Decisao Final";
    public const string AjustesSolicitados = "Ajustes Solicitados";
    public const string Aprovada = "Aprovada";
    public const string Negada = "Negada";
    public const string Encerrada = "Encerrada";

    public static bool PermiteEdicaoProfessor(string status)
    {
        return status is EmProcessamento or AjustesSolicitados;
    }

    public static bool PermiteSubmissaoProfessor(string status)
    {
        return status is EmProcessamento or AjustesSolicitados;
    }

    public static bool VisivelParaAdministracao(string status)
    {
        return status != EmProcessamento;
    }

    public static bool EhFinal(string status)
    {
        return status is Aprovada or Negada or Encerrada;
    }

    public static bool EhAberta(string status)
    {
        return status is AguardandoAtribuicao
            or EmProcessamento
            or EmRevisao
            or AguardandoDecisaoFinal
            or AjustesSolicitados;
    }
}
