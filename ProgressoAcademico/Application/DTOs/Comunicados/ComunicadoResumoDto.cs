namespace ProgressoAcademico.Application.DTOs.Comunicados;

public class ComunicadoResumoDto
{
    public int ComunicadoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Resumo { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public DateTime DataPublicacao { get; set; }
    public bool Publicado { get; set; }
}
