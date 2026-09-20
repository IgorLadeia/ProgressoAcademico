using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProgressoAcademico.Models.ViewModels.Admin;

public class AdminSolicitacaoFiltroViewModel
{
    public string? Status { get; set; }
    public string? Professor { get; set; }
    public int? TipoProgressoId { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int? PercentualMinimo { get; set; }
    public bool SomentePendentes { get; set; }
    public List<SelectListItem> StatusOpcoes { get; set; } = new();
    public List<SelectListItem> TiposProgresso { get; set; } = new();
}
