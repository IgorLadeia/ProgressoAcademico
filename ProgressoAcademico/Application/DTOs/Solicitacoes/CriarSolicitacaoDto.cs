namespace ProgressoAcademico.Application.DTOs.Solicitacoes;

public class CriarSolicitacaoDto
{
    public int TipoProgressoId { get; set; }
    public int NivelOrigemId { get; set; }
    public int NivelDestinoId { get; set; }
    public string? Apelido { get; set; }
    public string? Observacao { get; set; }
}
