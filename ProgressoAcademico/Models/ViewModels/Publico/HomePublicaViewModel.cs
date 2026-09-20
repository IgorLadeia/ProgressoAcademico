using ProgressoAcademico.Application.DTOs.Comunicados;
using ProgressoAcademico.Application.DTOs.ConteudoHome;

namespace ProgressoAcademico.Models.ViewModels.Publico;

public class HomePublicaViewModel
{
    public ConteudoHomeDto Conteudo { get; init; } = new();
    public IReadOnlyCollection<ComunicadoResumoDto> Comunicados { get; init; } = [];
}
