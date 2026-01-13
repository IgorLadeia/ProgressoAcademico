using ProgressoAcademico.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TipoVinculo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TipoVinculoId { get; set; }

    [Required, MaxLength(100)]
    public string Nome { get; set; }
    // Ex: Dedicação Exclusiva, 40h, 20h

    public ICollection<VinculoInstitucional> VinculoInstitucional { get; set; }
}
