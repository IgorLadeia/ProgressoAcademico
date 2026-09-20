using System.ComponentModel.DataAnnotations;

namespace ProgressoAcademico.Models.ViewModels.Comunicados;

public class ComunicadoFormViewModel
{
    public int? ComunicadoId { get; set; }

    [Required(ErrorMessage = "Informe o titulo.")]
    [MaxLength(180)]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o resumo.")]
    [MaxLength(350)]
    public string Resumo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o conteudo.")]
    public string Conteudo { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string Categoria { get; set; } = "Informativo";

    public bool Publicado { get; set; } = true;
}
