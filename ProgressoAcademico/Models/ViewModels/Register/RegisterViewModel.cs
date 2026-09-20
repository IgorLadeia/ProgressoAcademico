using System.ComponentModel.DataAnnotations;

namespace ProgressoAcademico.Models.ViewModels.Register;

public class RegisterViewModel
{
    [Required]
    [MaxLength(200)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Senha { get; set; } = string.Empty;

    public DateTime DataUltimoProgresso { get; set; }

    [MaxLength(200)]
    public string? Apelido { get; set; }

    public string? FotoPerfil { get; set; }

    [Required]
    public int InstituicaoId { get; set; }

    [Required]
    public int TipoVinculoId { get; set; }

    [Required]
    public int NivelId { get; set; }

    [Required]
    public DateTime DataIngressoInstituicao { get; set; }
}
