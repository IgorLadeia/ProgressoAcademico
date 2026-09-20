namespace ProgressoAcademico.Application.DTOs.Solicitacoes;

public class SubmeterSolicitacaoResultadoDto
{
    public bool Sucesso { get; private set; }
    public string? Erro { get; private set; }

    public static SubmeterSolicitacaoResultadoDto Ok()
    {
        return new SubmeterSolicitacaoResultadoDto { Sucesso = true };
    }

    public static SubmeterSolicitacaoResultadoDto Falha(string erro)
    {
        return new SubmeterSolicitacaoResultadoDto { Sucesso = false, Erro = erro };
    }
}
