using ProgressoAcademico.Models;
using System.ComponentModel.DataAnnotations;

public class TipoDocumento
{
    [Key]
    public int TipoDocumentoId { get; set; }
    [Required, MaxLength(100)]
    public string Tipo { get; set; }

    public ICollection<Documento> Documentos { get; set; }
}

