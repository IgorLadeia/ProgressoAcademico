using System.ComponentModel.DataAnnotations;

namespace ProgressoAcademico.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Informe seu nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Informe o e-mail")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe a senha")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }
    }
}
