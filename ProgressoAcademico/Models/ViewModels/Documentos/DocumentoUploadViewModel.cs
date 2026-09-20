using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProgressoAcademico.Models.ViewModels.Documentos;

public class DocumentoUploadViewModel
{
    [Required]
    public int SolicitacaoProgressaoId { get; set; }

    public int? AtividadeId { get; set; }

    public string? RetornoGrupo { get; set; }

    public int? TipoDocumentoId { get; set; }

    [Required(ErrorMessage = "Selecione a origem do documento.")]
    public string OrigemDocumento { get; set; } = ProgressoAcademico.Models.OrigensDocumento.ComprovatorioProfessor;

    [MaxLength(1000, ErrorMessage = "A observacao deve ter no maximo 1000 caracteres.")]
    public string? Observacao { get; set; }

    public IFormFile? Arquivo { get; set; }

    public List<IFormFile> Arquivos { get; set; } = new();

    public List<SelectListItem> TiposDocumento { get; set; } = new();
    public List<SelectListItem> OpcoesOrigensDocumento { get; set; } = new();
}
