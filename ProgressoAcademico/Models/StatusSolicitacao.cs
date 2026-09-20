using System.ComponentModel.DataAnnotations;

namespace ProgressoAcademico.Models;

public class StatusSolicitacao
{
    [Key]
    public int StatusSolicitacaoId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Nome { get; set; } = string.Empty;

    public ICollection<SolicitacaoProgressao> SolicitacaoProgressao { get; set; } = new List<SolicitacaoProgressao>();
}
