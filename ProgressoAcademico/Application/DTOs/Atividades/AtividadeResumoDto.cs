namespace ProgressoAcademico.Application.DTOs.Atividades;

public class AtividadeResumoDto
{
    public int AtividadeId { get; set; }
    public int SolicitacaoProgressaoId { get; set; }
    public int SubTipoAtividadeId { get; set; }
    public string TipoAtividade { get; set; } = string.Empty;
    public string SubtipoAtividade { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataTermino { get; set; }
    public decimal Quantidade { get; set; }
    public decimal PontuacaoCalculada { get; set; }
    public string Status { get; set; } = string.Empty;
    public string OrigemCadastro { get; set; } = string.Empty;
    public string? ParecerAvaliacao { get; set; }
    public DateTime? DataAvaliacao { get; set; }
    public IReadOnlyList<AtividadeAvaliacaoRevisorDto> AvaliacoesRevisores { get; set; } = Array.Empty<AtividadeAvaliacaoRevisorDto>();
}
