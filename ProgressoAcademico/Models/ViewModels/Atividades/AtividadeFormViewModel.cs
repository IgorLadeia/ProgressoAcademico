using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProgressoAcademico.Models.ViewModels.Atividades;

public class AtividadeFormViewModel
{
    public int? AtividadeId { get; set; }

    [Required]
    public int SolicitacaoProgressaoId { get; set; }

    [Required(ErrorMessage = "Selecione o tipo de atividade.")]
    public int? SubTipoAtividadeId { get; set; }

    [Required(ErrorMessage = "Informe o titulo da atividade.")]
    [MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    [Required(ErrorMessage = "Informe a data de inicio.")]
    public DateTime? DataInicio { get; set; }

    public DateTime? DataTermino { get; set; }

    [Range(1, 999, ErrorMessage = "Informe uma quantidade inteira maior que zero.")]
    public int Quantidade { get; set; } = 1;

    public List<IFormFile> Comprovantes { get; set; } = new();

    public List<SelectListItem> SubtiposAtividade { get; set; } = new();
}
