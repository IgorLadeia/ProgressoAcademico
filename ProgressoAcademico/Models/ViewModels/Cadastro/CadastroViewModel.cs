using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProgressoAcademico.Models.ViewModels.Cadastro;

public class CadastroViewModel
{
    [Required(ErrorMessage = "Informe o nome completo.")]
    [MaxLength(200)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o email institucional.")]
    [EmailAddress(ErrorMessage = "Informe um email valido.")]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha.")]
    [MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres.")]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme a senha.")]
    [Compare(nameof(Senha), ErrorMessage = "As senhas informadas nao conferem.")]
    public string ConfirmarSenha { get; set; } = string.Empty;

    [Required]
    public string ModoCadastro { get; set; } = CadastroModos.Basico;

    [MaxLength(200)]
    public string? Apelido { get; set; }

    public int? InstituicaoId { get; set; }
    public int? TipoVinculoId { get; set; }
    public int? NivelId { get; set; }
    public DateTime? DataIngressoInstituicao { get; set; }
    public DateTime? DataUltimoProgresso { get; set; }

    public List<SelectListItem> Instituicoes { get; set; } = new();
    public List<SelectListItem> TiposVinculo { get; set; } = new();
    public List<SelectListItem> Niveis { get; set; } = new();
}

public static class CadastroModos
{
    public const string Basico = "Basico";
    public const string Completo = "Completo";
}
