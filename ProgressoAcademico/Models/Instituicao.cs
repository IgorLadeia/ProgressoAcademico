using ProgressoAcademico.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Instituicao
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int InstituicaoId { get; set; }

    [Required, MaxLength(200)]
    public string Nome { get; set; } 

    [Required, MaxLength(20)]
    public string Sigla { get; set; } // UFABC

    public ICollection<VinculoInstitucional> Vinculos { get; set; }
}
