using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models;

public class StatusSolicitacao
{
    [Key]
    public int StatusSolicitacaoId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Nome { get; set; }

    // Relacionamento
    public ICollection<SolicitacaoProgressao> SolicitacaoProgressao { get; set; }
}
