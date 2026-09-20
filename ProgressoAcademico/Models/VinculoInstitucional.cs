using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models;

public class VinculoInstitucional
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int VinculoInstitucionalId { get; set; }

    [Required]
    public int UsuarioId { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario Usuario { get; set; } = null!;

    [Required]
    public int InstituicaoId { get; set; }

    [ForeignKey(nameof(InstituicaoId))]
    public Instituicao Instituicao { get; set; } = null!;

    [Required]
    public int TipoVinculoId { get; set; }

    [ForeignKey(nameof(TipoVinculoId))]
    public TipoVinculo TipoVinculo { get; set; } = null!;

    [Required]
    public int NivelId { get; set; }

    [ForeignKey(nameof(NivelId))]
    public Nivel Nivel { get; set; } = null!;

    [Required]
    public DateTime DataIngressoInstituicao { get; set; }

    public DateTime? DataUltimaProgressao { get; set; }

    [Required]
    public bool Ativo { get; set; }
}
