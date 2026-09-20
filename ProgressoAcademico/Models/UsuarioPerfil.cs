using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models;

public class UsuarioPerfil
{
    [Key]
    [ForeignKey(nameof(Usuario))]
    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;

    [MaxLength(200)]
    public string? Apelido { get; set; }

    public string? FotoPerfil { get; set; }

    public byte[]? FotoPerfilArquivo { get; set; }

    [MaxLength(50)]
    public string? FotoPerfilContentType { get; set; }

    public DateTime? FotoPerfilAtualizadaEm { get; set; }
}
