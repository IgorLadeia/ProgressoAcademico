namespace ProgressoAcademico.Application.DTOs.Documentos;

public class DocumentoResumoDto
{
    public int DocumentoId { get; set; }
    public int SolicitacaoProgressaoId { get; set; }
    public int? AtividadeId { get; set; }
    public string TipoDocumento { get; set; } = string.Empty;
    public string NomeArquivo { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long TamanhoBytes { get; set; }
    public DateTime DataUpload { get; set; }
    public string OrigemDocumento { get; set; } = string.Empty;
    public string HashSha256 { get; set; } = string.Empty;
    public string? Observacao { get; set; }
}
