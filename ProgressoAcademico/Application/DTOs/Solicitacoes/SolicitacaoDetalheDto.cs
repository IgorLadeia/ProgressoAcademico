namespace ProgressoAcademico.Application.DTOs.Solicitacoes;

public class SolicitacaoDetalheDto : SolicitacaoResumoDto
{
    public string? Observacao { get; set; }
    public string? ParecerFinal { get; set; }
}
