namespace ProgressoAcademico.Application.DTOs.Solicitacoes;

public class SolicitacaoResumoDto
{
    public int SolicitacaoProgressaoId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string TipoProgressao { get; set; } = string.Empty;
    public string NivelOrigem { get; set; } = string.Empty;
    public string NivelDestino { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime DataUltimaMovimentacao { get; set; }
    public DateTime? DataFechamento { get; set; }
    public string? Apelido { get; set; }
    public int PercentualPreenchimento { get; set; }
    public bool PodeEditar { get; set; }
    public bool PodeSubmeter { get; set; }
    public int TotalDocumentos { get; set; }
    public int TotalAtividades { get; set; }
}
