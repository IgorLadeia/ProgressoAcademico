using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models;

public class AtividadeAvaliacaoRevisor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AtividadeAvaliacaoRevisorId { get; set; }

    [Required]
    public int AtividadeId { get; set; }

    [ForeignKey(nameof(AtividadeId))]
    public Atividade Atividade { get; set; } = null!;

    [Required]
    public int SolicitacaoRevisorId { get; set; }

    [ForeignKey(nameof(SolicitacaoRevisorId))]
    public SolicitacaoRevisor SolicitacaoRevisor { get; set; } = null!;

    [Required]
    [MaxLength(30)]
    public string Resultado { get; set; } = string.Empty;

    [Required]
    [MaxLength(1500)]
    public string Parecer { get; set; } = string.Empty;

    [Required]
    public DateTime DataAvaliacao { get; set; }
}
