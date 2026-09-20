using System.ComponentModel.DataAnnotations;

namespace ProgressoAcademico.Models.ViewModels.Admin;

public class AdminAvaliarAtividadeViewModel
{
    [Required]
    public int SolicitacaoProgressaoId { get; set; }

    [Required]
    public int AtividadeId { get; set; }

    [Required(ErrorMessage = "Selecione o resultado da avaliação.")]
    public string Resultado { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o parecer da atividade.")]
    [MaxLength(1500)]
    public string Parecer { get; set; } = string.Empty;
}
