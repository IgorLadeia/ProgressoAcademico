using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models;

public class Comunicado
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ComunicadoId { get; set; }

    [Required]
    [MaxLength(180)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [MaxLength(350)]
    public string Resumo { get; set; } = string.Empty;

    [Required]
    public string Conteudo { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string Categoria { get; set; } = "Informativo";

    [Required]
    public DateTime DataPublicacao { get; set; }

    [Required]
    public bool Publicado { get; set; }

    [Required]
    public DateTime DataCriacao { get; set; }

    public DateTime? DataAtualizacao { get; set; }

    public int? CriadoPorUsuarioId { get; set; }
    public Usuario? CriadoPorUsuario { get; set; }
}
