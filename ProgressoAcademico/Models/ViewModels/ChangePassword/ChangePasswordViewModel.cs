using System.ComponentModel.DataAnnotations;

namespace ProgressoAcademico.Models.ViewModels.ChangePassword;

public class ChangePasswordViewModel
{
    [Required]
    public string SenhaAtual { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string NovaSenha { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(NovaSenha))]
    public string ConfirmarNovaSenha { get; set; } = string.Empty;
}
