using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models;

public class AtividadeEnsino
{
    [Key]
    [ForeignKey(nameof(Atividade))]
    public int AtividadeId { get; set; }

    [MaxLength(50)]
    public string? CodigoDisciplina { get; set; }

    [MaxLength(100)]
    public string? NomeDisciplina { get; set; }

    public int NumeroAlunos { get; set; }
    public int Creditos { get; set; }

    [MaxLength(50)]
    public string? Turno { get; set; }

    public int CargaHoraria { get; set; }

    public Atividade Atividade { get; set; } = null!;
}
