using System.ComponentModel.DataAnnotations;

namespace ProgressoAcademico.Models.ViewModels.Admin;

public class AdminDecisaoSolicitacaoViewModel
{
    [Required]
    public int SolicitacaoProgressaoId { get; set; }

    [Required(ErrorMessage = "Informe o parecer administrativo.")]
    [MaxLength(2000)]
    public string Parecer { get; set; } = string.Empty;
}
