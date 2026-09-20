using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models;

public class SubTipoAtividade
{
    [Key]
    public int SubtipoAtividadeId { get; set; }

    [Required]
    public int TipoAtividadeId { get; set; }

    [ForeignKey(nameof(TipoAtividadeId))]
    public TipoAtividade TipoAtividade { get; set; } = null!;

    [Required]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    public int Pontos { get; set; }

    public string? Descricao { get; set; }
}
