using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models
{
    public class Documento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentoId { get; set; }

        // 🔗 Documento pertence a uma solicitação
        [Required]
        public int SolicitacaoProgressaoId { get; set; }

        [ForeignKey(nameof(SolicitacaoProgressaoId))]
        public SolicitacaoProgressao SolicitacaoProgressao { get; set; }

        // 🔗 Tipo do documento (ex: Diploma, Comprovante, Ata)
        [Required]
        public int TipoDocumentoId { get; set; }

        [ForeignKey(nameof(TipoDocumentoId))]
        public TipoDocumento TipoDocumento { get; set; }

        // 🔗 Documento pode estar vinculado a uma atividade
        public int? AtividadeId { get; set; }

        [ForeignKey(nameof(AtividadeId))]
        public Atividade Atividade { get; set; }

        public byte[] Arquivo { get; set; }

    }

}
    
