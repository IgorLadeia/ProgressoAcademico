using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models;

public class AtividadeExtensao
{
    [Key]
    [ForeignKey(nameof(Atividade))]
    public int AtividadeId { get; set; }

    [MaxLength(100)]
    public string? Projeto { get; set; }

    [MaxLength(100)]
    public string? CargaHoraria { get; set; }

    [MaxLength(100)]
    public string? PublicoAlvo { get; set; }

    public Atividade? Atividade { get; set; }
}
