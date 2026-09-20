using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProgressoAcademico.Application.DTOs.Admin;

namespace ProgressoAcademico.Models.ViewModels.Admin;

public class AdminAtribuirRevisorViewModel
{
    public int SolicitacaoProgressaoId { get; set; }

    public string Professor { get; set; } = string.Empty;
    public string StatusAtual { get; set; } = string.Empty;

    [Display(Name = "Revisores")]
    [Required(ErrorMessage = "Selecione ao menos um professor revisor.")]
    public List<int> RevisorUsuarioIds { get; set; } = new();

    public IReadOnlyList<SelectListItem> Revisores { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SolicitacaoRevisorDto> RevisoresAtribuidos { get; set; } = Array.Empty<SolicitacaoRevisorDto>();
}
