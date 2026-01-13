using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models
{
    public class TipoProgresso
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TipoProgressoId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [MaxLength(500)]
        public string Descricao { get; set; }

        /// <summary>
        /// Indica se esse tipo está ativo para novas solicitações
        /// </summary>

        // 🔗 Um tipo de progresso pode ser usado em várias solicitações
        public ICollection<SolicitacaoProgressao> SolicitacaoProgressao { get; set; }
    }
}
