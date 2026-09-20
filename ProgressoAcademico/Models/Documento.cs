using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models;

public class Documento
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DocumentoId { get; set; }

    [Required]
    public int SolicitacaoProgressaoId { get; set; }

    [ForeignKey(nameof(SolicitacaoProgressaoId))]
    public SolicitacaoProgressao SolicitacaoProgressao { get; set; } = null!;

    [Required]
    public int TipoDocumentoId { get; set; }

    [ForeignKey(nameof(TipoDocumentoId))]
    public TipoDocumento TipoDocumento { get; set; } = null!;

    public int? AtividadeId { get; set; }

    [ForeignKey(nameof(AtividadeId))]
    public Atividade? Atividade { get; set; }

    [Required]
    [MaxLength(255)]
    public string NomeArquivo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ContentType { get; set; } = "application/octet-stream";

    public long TamanhoBytes { get; set; }

    [Required]
    public DateTime DataUpload { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(30)]
    public string OrigemDocumento { get; set; } = OrigensDocumento.ComprovatorioProfessor;

    [Required]
    [MaxLength(64)]
    public string HashSha256 { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Observacao { get; set; }

    public byte[] Arquivo { get; set; } = Array.Empty<byte>();
}
