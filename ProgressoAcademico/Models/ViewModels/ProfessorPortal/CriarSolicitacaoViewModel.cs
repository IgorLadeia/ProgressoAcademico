using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProgressoAcademico.Models.ViewModels.ProfessorPortal;

public class CriarSolicitacaoViewModel
{
    [Required(ErrorMessage = "Selecione o tipo de progressao.")]
    public int? TipoProgressoId { get; set; }

    [Required(ErrorMessage = "Selecione o nivel de origem.")]
    public int? NivelOrigemId { get; set; }

    [Required(ErrorMessage = "Selecione o nivel de destino.")]
    public int? NivelDestinoId { get; set; }

    [MaxLength(50, ErrorMessage = "O apelido deve ter no maximo 50 caracteres.")]
    public string? Apelido { get; set; }

    [MaxLength(1000, ErrorMessage = "A observacao deve ter no maximo 1000 caracteres.")]
    public string? Observacao { get; set; }

    public List<SelectListItem> TiposProgresso { get; set; } = new();
    public List<SelectListItem> Niveis { get; set; } = new();
}
