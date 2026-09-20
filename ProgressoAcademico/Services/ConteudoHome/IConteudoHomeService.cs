using ProgressoAcademico.Application.DTOs.ConteudoHome;
using ProgressoAcademico.Models.ViewModels.ConteudoHome;

namespace ProgressoAcademico.Services.ConteudoHome;

public interface IConteudoHomeService
{
    Task<ConteudoHomeDto> ObterAsync();
    Task<ConteudoHomeEdicaoViewModel?> ObterParaEdicaoAsync();
    Task<bool> AtualizarAsync(ConteudoHomeEdicaoViewModel model, int? usuarioId);
}
