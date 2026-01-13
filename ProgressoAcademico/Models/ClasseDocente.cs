using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models
{
    public class ClasseDocente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClasseDocenteId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        /// <summary>
        /// Ordem hierárquica da classe docente
        /// Ex: 1 = Auxiliar, 2 = Assistente, 3 = Adjunto, etc.
        /// </summary>
        [Required]
        public int NivelHierarquico { get; set; }

        /// <summary>
        /// Indica se a classe está ativa no sistema
        /// </summary>

        // 🔗 Relacionamento (opcional, mas comum)
        public ICollection<VinculoInstitucional> VinculosInstitucionais { get; set; }
    }
}
