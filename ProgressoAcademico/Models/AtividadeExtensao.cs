using ProgressoAcademico.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class AtividadeExtensao
{
    [Key, ForeignKey(nameof(Atividade))]
    public int AtividadeId { get; set; }

    [MaxLength(100)]
    public string? Projeto { get; set; }

    [MaxLength(100)]
    public string? CargaHoraria { get; set; }

    [MaxLength(100)]
    public string? PublicoAlvo { get; set; }


    public LoginViewModel? Atividade { get; set; }
}
