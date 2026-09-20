using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models;

public class SolicitacaoRevisor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SolicitacaoRevisorId { get; set; }

    [Required]
    public int SolicitacaoProgressaoId { get; set; }

    [ForeignKey(nameof(SolicitacaoProgressaoId))]
    public SolicitacaoProgressao SolicitacaoProgressao { get; set; } = null!;

    [Required]
    public int RevisorUsuarioId { get; set; }

    [ForeignKey(nameof(RevisorUsuarioId))]
    public Usuario RevisorUsuario { get; set; } = null!;

    public int? AtribuidoPorUsuarioId { get; set; }

    [ForeignKey(nameof(AtribuidoPorUsuarioId))]
    public Usuario? AtribuidoPorUsuario { get; set; }

    [Required]
    public DateTime DataAtribuicao { get; set; }

    [Required]
    [MaxLength(30)]
    public string StatusRevisao { get; set; } = StatusRevisaoNomes.EmRevisao;

    [MaxLength(2000)]
    public string? Parecer { get; set; }

    public DateTime? DataParecer { get; set; }

    public ICollection<AtividadeAvaliacaoRevisor> AvaliacoesAtividades { get; set; } = new List<AtividadeAvaliacaoRevisor>();
}
