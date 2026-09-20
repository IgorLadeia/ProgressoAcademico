using System.ComponentModel.DataAnnotations;

namespace ProgressoAcademico.Models;

public class TipoDocumento
{
    [Key]
    public int TipoDocumentoId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Tipo { get; set; } = string.Empty;

    public ICollection<Documento> Documentos { get; set; } = new List<Documento>();
}
