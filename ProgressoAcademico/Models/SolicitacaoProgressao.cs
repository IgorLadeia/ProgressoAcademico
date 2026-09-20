using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models;

public class SolicitacaoProgressao
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SolicitacaoProgressaoId { get; set; }

    [Required]
    public int UsuarioId { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario Usuario { get; set; } = null!;

    [Required]
    public int StatusSolicitacaoId { get; set; }

    [ForeignKey(nameof(StatusSolicitacaoId))]
    public StatusSolicitacao StatusSolicitacao { get; set; } = null!;

    [Required]
    public int TipoProgressoId { get; set; }

    [ForeignKey(nameof(TipoProgressoId))]
    public TipoProgresso TipoProgresso { get; set; } = null!;

    [Required]
    public int NivelOrigemId { get; set; }

    [ForeignKey(nameof(NivelOrigemId))]
    public Nivel NivelOrigem { get; set; } = null!;

    [Required]
    public int NivelDestinoId { get; set; }

    [ForeignKey(nameof(NivelDestinoId))]
    public Nivel NivelDestino { get; set; } = null!;

    [Required]
    public DateTime DataCriacao { get; set; }

    public DateTime? DataUltimaMovimentacao { get; set; }

    public DateTime? DataFechamento { get; set; }

    [MaxLength(1000)]
    public string? Observacao { get; set; }

    [MaxLength(2000)]
    public string? ParecerFinal { get; set; }

    [MaxLength(50)]
    public string? Apelido { get; set; }

    public bool Multiprogressao { get; set; }

    public int? RevisorUsuarioId { get; set; }

    [ForeignKey(nameof(RevisorUsuarioId))]
    public Usuario? RevisorUsuario { get; set; }

    public int? AtribuidoPorUsuarioId { get; set; }

    [ForeignKey(nameof(AtribuidoPorUsuarioId))]
    public Usuario? AtribuidoPorUsuario { get; set; }

    public DateTime? DataAtribuicaoRevisor { get; set; }

    [MaxLength(2000)]
    public string? ParecerRevisor { get; set; }

    public DateTime? DataParecerRevisor { get; set; }

    public int? RevisadoPorUsuarioId { get; set; }

    [ForeignKey(nameof(RevisadoPorUsuarioId))]
    public Usuario? RevisadoPorUsuario { get; set; }

    public DateTime? DataRevisao { get; set; }

    public ICollection<Atividade> Atividades { get; set; } = new List<Atividade>();
    public ICollection<Documento> Documentos { get; set; } = new List<Documento>();
    public ICollection<SolicitacaoRevisor> Revisores { get; set; } = new List<SolicitacaoRevisor>();
}
