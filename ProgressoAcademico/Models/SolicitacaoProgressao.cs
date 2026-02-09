using ProgressoAcademico.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProgressoAcademico.Models
{
    public class SolicitacaoProgressao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SolicitacaoProgressaoId { get; set; }

        // 🔗 Professor solicitante
        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario Usuario { get; set; }

        // 🔗 Status da solicitação (Aberta, Em Análise, Deferida, Indeferida)
        [Required]
        public int StatusSolicitacaoId { get; set; }

        [ForeignKey(nameof(StatusSolicitacaoId))]
        public StatusSolicitacao StatusSolicitacao { get; set; }

        // 🔗 Tipo de progresso (Vertical, Horizontal, Classe)
        [Required]
        public int TipoProgressoId { get; set; }

        [ForeignKey(nameof(TipoProgressoId))]
        public TipoProgresso TipoProgresso { get; set; }

        // 🔗 Nível atual
        [Required]
        public int NivelOrigemId { get; set; }

        [ForeignKey(nameof(NivelOrigemId))]
        public Nivel NivelOrigem { get; set; }

        // 🔗 Nível pretendido
        [Required]
        public int NivelDestinoId { get; set; }

        [ForeignKey(nameof(NivelDestinoId))]
        public Nivel NivelDestino { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; }

        public DateTime? DataFechamento { get; set; }

        [MaxLength(1000)]
        public string Observacao { get; set; }

        [MaxLength(2000)]
        public string ParecerFinal { get; set; }

        /// <summary>
        /// Resultado final (ex: DEFERIDO, INDEFERIDO)
        /// </summary>
        [MaxLength(50)]
        public string Apelido { get; set; }

        // 🔗 Uma solicitação possui várias atividades
        public ICollection<LoginViewModel> Atividades { get; set; }

        // 🔗 Uma solicitação pode ter vários documentos
        public ICollection<Documento> Documentos { get; set; }
    }
}