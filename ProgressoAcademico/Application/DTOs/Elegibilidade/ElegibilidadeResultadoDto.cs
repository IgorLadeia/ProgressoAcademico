namespace ProgressoAcademico.Application.DTOs.Elegibilidade;

public class ElegibilidadeResultadoDto
{
    public int SolicitacaoProgressaoId { get; set; }
    public string Status { get; set; } = "NaoCalculado";
    public int PercentualElegibilidade { get; set; }
    public int RequisitosAtendidos { get; set; }
    public int TotalRequisitos { get; set; }
    public IReadOnlyList<RequisitoElegibilidadeDto> Requisitos { get; set; } = Array.Empty<RequisitoElegibilidadeDto>();
    public IReadOnlyList<string> Pendencias { get; set; } = Array.Empty<string>();
}
