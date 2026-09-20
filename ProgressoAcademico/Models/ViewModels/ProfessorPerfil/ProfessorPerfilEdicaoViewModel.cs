using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProgressoAcademico.Models.ViewModels.ProfessorPerfil;

public class ProfessorPerfilEdicaoViewModel
{
    [Required(ErrorMessage = "Informe o nome completo.")]
    [MaxLength(200)]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o e-mail institucional.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail valido.")]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    public bool PossuiFoto { get; set; }
    public string? FotoPerfilUrl { get; set; }

    [MaxLength(200, ErrorMessage = "O nome social deve ter no maximo 200 caracteres.")]
    public string? NomeSocial { get; set; }

    public DateTime? DataUltimoProgresso { get; set; }
    public int? InstituicaoId { get; set; }
    public int? TipoVinculoId { get; set; }
    public int? NivelId { get; set; }
    public DateTime? DataIngressoInstituicao { get; set; }

    public IFormFile? Foto { get; set; }
    public bool RemoverFoto { get; set; }

    public List<SelectListItem> Instituicoes { get; set; } = new();
    public List<SelectListItem> TiposVinculo { get; set; } = new();
    public List<SelectListItem> Niveis { get; set; } = new();
}
