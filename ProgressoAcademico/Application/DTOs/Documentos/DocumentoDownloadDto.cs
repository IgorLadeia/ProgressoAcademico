namespace ProgressoAcademico.Application.DTOs.Documentos;

public class DocumentoDownloadDto
{
    public string NomeArquivo { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/octet-stream";
    public byte[] Arquivo { get; set; } = Array.Empty<byte>();
}
