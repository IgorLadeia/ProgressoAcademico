namespace ProgressoAcademico.Application.DTOs.Atividades;

public class AtividadeAvaliacaoRevisorDto
{
    public int AtividadeAvaliacaoRevisorId { get; set; }
    public int AtividadeId { get; set; }
    public int SolicitacaoRevisorId { get; set; }
    public string Revisor { get; set; } = string.Empty;
    public string EmailRevisor { get; set; } = string.Empty;
    public string Resultado { get; set; } = string.Empty;
    public string Parecer { get; set; } = string.Empty;
    public DateTime DataAvaliacao { get; set; }
}
