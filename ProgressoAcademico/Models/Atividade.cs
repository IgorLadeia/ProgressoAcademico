using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models;

public class Atividade
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AtividadeId { get; set; }

    [Required]
    public int SolicitacaoProgressaoId { get; set; } = 0;

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
    public string Titulo { get; set; } = null!;
    public string? Descricao { get; set; }
    public decimal Quantidade { get; set; }
    public decimal PontuacaoCalculada { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pendente";

    //especialização da tabela Atividade para cada tipo específico de atividade
    public AtividadeEnsino AtividadeEnsino { get; set; }
    public AtividadePesquisa AtividadePesquisa { get; set; }
    public AtividadeExtensao AtividadeExtensao { get; set; }
    public AtividadeAdministrativa AtividadeAdministrativa { get; set; }

    //relacionamento com a tabela Documento
    public ICollection<Documento> Documentos { get; set; }
}


