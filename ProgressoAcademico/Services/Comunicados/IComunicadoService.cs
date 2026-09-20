using ProgressoAcademico.Application.DTOs.Comunicados;
using ProgressoAcademico.Models.ViewModels.Comunicados;

namespace ProgressoAcademico.Services.Comunicados;

public interface IComunicadoService
{
    Task<List<ComunicadoResumoDto>> ListarPublicadosAsync();
    Task<List<ComunicadoResumoDto>> ListarTodosAsync();
    Task<ComunicadoFormViewModel?> ObterParaEdicaoAsync(int comunicadoId);
    Task CriarAsync(ComunicadoFormViewModel model, int? usuarioId);
    Task<bool> AtualizarAsync(ComunicadoFormViewModel model);
    Task<bool> ExcluirAsync(int comunicadoId);
}
