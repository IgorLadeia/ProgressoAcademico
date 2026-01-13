using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models
{
    public class Nivel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NivelId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Classe { get; set; }
        // Ex: Auxiliar, Assistente, Adjunto, Associado, Titular

        [Required]
        [MaxLength(20)]
        public string Codigo { get; set; }
        // Ex: A1, A2, B1, C1

        [Required]
        public int Ordem { get; set; }
        // Usado para validar se destino > origem

        [MaxLength(500)]
        public string Descricao { get; set; }

        public bool Ativo { get; set; }

        // 🔗 Navegações inversas (opcional, mas recomendado)
        public ICollection<SolicitacaoProgressao> SolicitacoesComoOrigem { get; set; }
        public ICollection<SolicitacaoProgressao> SolicitacoesComoDestino { get; set; }

    }
}
