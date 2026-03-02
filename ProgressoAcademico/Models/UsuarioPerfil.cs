using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models;

public class UsuarioPerfil
{
    [Key]
    [ForeignKey(nameof(Usuario))]
    public int UsuarioId { get; set; }


    // 🔗 Navegação obrigatória
    public Usuario Usuario { get; set; }

    [MaxLength(200)]
    public string Apelido { get; set; }

    public string FotoPerfil { get; set; }
}
