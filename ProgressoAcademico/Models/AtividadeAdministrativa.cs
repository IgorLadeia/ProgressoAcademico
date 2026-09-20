using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models;

public class AtividadeAdministrativa
{
    [Key]
    [ForeignKey(nameof(Atividade))]
    public int AtividadeId { get; set; }

    [MaxLength(100)]
    public string? Cargo { get; set; }

    [MaxLength(100)]
    public string? Unidade { get; set; }

    public Atividade? Atividade { get; set; }
}
