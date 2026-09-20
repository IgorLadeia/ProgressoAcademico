using ProgressoAcademico.Application.DTOs.Elegibilidade;

namespace ProgressoAcademico.Services.Elegibilidade;

public interface IElegibilidadeService
{
    Task<ElegibilidadeResultadoDto?> AvaliarAsync(int usuarioId, int solicitacaoProgressaoId);
}
