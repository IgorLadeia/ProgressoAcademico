namespace ProgressoAcademico.Application.DTOs.Solicitacoes;

public class CriarSolicitacaoResultadoDto
{
    public bool Sucesso { get; set; }
    public int? SolicitacaoProgressaoId { get; set; }
    public string? Erro { get; set; }

    public static CriarSolicitacaoResultadoDto Falha(string erro)
    {
        return new CriarSolicitacaoResultadoDto
        {
            Sucesso = false,
            Erro = erro
        };
    }

    public static CriarSolicitacaoResultadoDto Criada(int solicitacaoProgressaoId)
    {
        return new CriarSolicitacaoResultadoDto
        {
            Sucesso = true,
            SolicitacaoProgressaoId = solicitacaoProgressaoId
        };
    }
}
