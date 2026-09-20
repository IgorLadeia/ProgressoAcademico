namespace ProgressoAcademico.Application.DTOs.Documentos;

public class UploadDocumentoResultadoDto
{
    public bool Sucesso { get; set; }
    public int? DocumentoId { get; set; }
    public string? Erro { get; set; }

    public static UploadDocumentoResultadoDto Falha(string erro)
    {
        return new UploadDocumentoResultadoDto { Sucesso = false, Erro = erro };
    }

    public static UploadDocumentoResultadoDto Criado(int documentoId)
    {
        return new UploadDocumentoResultadoDto { Sucesso = true, DocumentoId = documentoId };
    }
}
