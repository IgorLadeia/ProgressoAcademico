using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProgressoAcademico.Models.ViewModels.Admin;

public class AdminCorrigirSolicitacaoViewModel
{
    [Required]
    public int SolicitacaoProgressaoId { get; set; }

    [Required]
    public int TipoProgressoId { get; set; }

    [Required]
    public int NivelOrigemId { get; set; }

    [Required]
    public int NivelDestinoId { get; set; }

    [Required]
    public string Status { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Apelido { get; set; }

    [MaxLength(1000)]
    public string? Observacao { get; set; }

    [MaxLength(2000)]
    public string? ParecerFinal { get; set; }

    public List<SelectListItem> TiposProgresso { get; set; } = new();
    public List<SelectListItem> Niveis { get; set; } = new();
    public List<SelectListItem> StatusOpcoes { get; set; } = new();
}
