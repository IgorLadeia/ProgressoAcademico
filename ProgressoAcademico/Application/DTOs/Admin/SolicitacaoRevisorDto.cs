namespace ProgressoAcademico.Application.DTOs.Admin;

public class SolicitacaoRevisorDto
{
    public int SolicitacaoRevisorId { get; set; }
    public int RevisorUsuarioId { get; set; }
    public string Revisor { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string StatusRevisao { get; set; } = string.Empty;
    public string? Parecer { get; set; }
    public DateTime DataAtribuicao { get; set; }
    public DateTime? DataParecer { get; set; }
}
