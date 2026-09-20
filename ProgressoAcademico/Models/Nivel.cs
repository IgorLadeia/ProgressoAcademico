using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models;

public class Nivel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int NivelId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Classe { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    public int Ordem { get; set; }

    [MaxLength(500)]
    public string? Descricao { get; set; }

    public bool Ativo { get; set; }

    public ICollection<SolicitacaoProgressao> SolicitacoesComoOrigem { get; set; } = new List<SolicitacaoProgressao>();
    public ICollection<SolicitacaoProgressao> SolicitacoesComoDestino { get; set; } = new List<SolicitacaoProgressao>();
    public ICollection<VinculoInstitucional> VinculoInstitucionals { get; set; } = new List<VinculoInstitucional>();
}
