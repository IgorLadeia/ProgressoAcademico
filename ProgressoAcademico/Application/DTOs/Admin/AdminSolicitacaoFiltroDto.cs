namespace ProgressoAcademico.Application.DTOs.Admin;

public class AdminSolicitacaoFiltroDto
{
    public string? Status { get; set; }
    public string? Professor { get; set; }
    public int? TipoProgressoId { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int? PercentualMinimo { get; set; }
    public bool SomentePendentes { get; set; }
}
