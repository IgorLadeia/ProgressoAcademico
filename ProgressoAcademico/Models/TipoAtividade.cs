using System.ComponentModel.DataAnnotations;

namespace ProgressoAcademico.Models;

public class TipoAtividade
{
    [Key]
    public int TipoAtividadeId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public ICollection<SubTipoAtividade> Subtipos { get; set; } = new List<SubTipoAtividade>();
}
