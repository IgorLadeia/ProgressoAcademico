using System.ComponentModel.DataAnnotations;

namespace ProgressoAcademico.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Informe o e-mail")]        
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }
    }
}