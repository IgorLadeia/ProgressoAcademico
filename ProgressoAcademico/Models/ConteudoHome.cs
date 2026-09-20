using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models;

public class ConteudoHome
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ConteudoHomeId { get; set; }

    [Required]
    [MaxLength(220)]
    public string TituloPrincipal { get; set; } = string.Empty;

    [Required]
    [MaxLength(1200)]
    public string ResumoProjeto { get; set; } = string.Empty;

    [Required]
    [MaxLength(180)]
    public string TituloDestaque { get; set; } = string.Empty;

    [Required]
    [MaxLength(1600)]
    public string ItensDestaque { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string AvisoEscopo { get; set; } = string.Empty;

    [Required]
    [MaxLength(600)]
    public string DescricaoProfessor { get; set; } = string.Empty;

    [Required]
    [MaxLength(1600)]
    public string RecursosProfessor { get; set; } = string.Empty;

    [Required]
    [MaxLength(600)]
    public string DescricaoAdministrador { get; set; } = string.Empty;

    [Required]
    [MaxLength(1600)]
    public string RecursosAdministrador { get; set; } = string.Empty;

    public DateTime DataAtualizacao { get; set; }

    public int? AtualizadoPorUsuarioId { get; set; }
    public Usuario? AtualizadoPorUsuario { get; set; }
}
