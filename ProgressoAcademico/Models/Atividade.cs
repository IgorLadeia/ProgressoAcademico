using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models;

public class Atividade
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AtividadeId { get; set; }

    [Required]
    public int SolicitacaoProgressaoId { get; set; }

    [ForeignKey(nameof(SolicitacaoProgressaoId))]
    public SolicitacaoProgressao? SolicitacaoProgressao { get; set; }

    [Required]
    public int TipoAtividadeId { get; set; }

    [ForeignKey(nameof(TipoAtividadeId))]
    public TipoAtividade? TipoAtividade { get; set; }

    [Required]
    public int SubTipoAtividadeId { get; set; }

    [ForeignKey(nameof(SubTipoAtividadeId))]
    public SubTipoAtividade? SubTipoAtividade { get; set; }

    [Required]
    public DateTime DataInicio { get; set; }

    public DateTime? DataTermino { get; set; }

    [Required]
    [MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    public string? Descricao { get; set; }
    public decimal Quantidade { get; set; }
    public decimal PontuacaoCalculada { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pendente";

    [Required]
    [MaxLength(30)]
    public string OrigemCadastro { get; set; } = OrigensAtividade.CadastroManual;

    [MaxLength(1500)]
    public string? ParecerAvaliacao { get; set; }

    public DateTime? DataAvaliacao { get; set; }

    public AtividadeEnsino? AtividadeEnsino { get; set; }
    public AtividadePesquisa? AtividadePesquisa { get; set; }
    public AtividadeExtensao? AtividadeExtensao { get; set; }
    public AtividadeAdministrativa? AtividadeAdministrativa { get; set; }

    public ICollection<Documento> Documentos { get; set; } = new List<Documento>();
    public ICollection<AtividadeAvaliacaoRevisor> AvaliacoesRevisores { get; set; } = new List<AtividadeAvaliacaoRevisor>();
}
