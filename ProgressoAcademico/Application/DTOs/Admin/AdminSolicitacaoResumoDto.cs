namespace ProgressoAcademico.Application.DTOs.Admin;

public class AdminSolicitacaoResumoDto
{
    public int SolicitacaoProgressaoId { get; set; }
    public string Professor { get; set; } = string.Empty;
    public string EmailProfessor { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string TipoProgressao { get; set; } = string.Empty;
    public string NivelOrigem { get; set; } = string.Empty;
    public string NivelDestino { get; set; } = string.Empty;
    public bool Multiprogressao { get; set; }
    public string? Revisor { get; set; }
    public int TotalRevisores { get; set; }
    public int RevisoresPendentes { get; set; }
    public DateTime? DataAtribuicaoRevisor { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataFechamento { get; set; }
    public string? Apelido { get; set; }
    public int PercentualPreenchimento { get; set; }
    public int TotalAtividades { get; set; }
    public int TotalDocumentos { get; set; }
}
