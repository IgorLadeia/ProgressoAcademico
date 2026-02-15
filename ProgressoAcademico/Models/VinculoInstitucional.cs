using ProgressoAcademico.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models
{
    public class VinculoInstitucional
    {
        // 🔑 PK + FK (1:1 com Usuario)
        [Key]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario Usuario { get; set; }

        // 🏫 Instituição
        [Required]
        public int InstituicaoId { get; set; }

        [ForeignKey(nameof(InstituicaoId))]
        public Instituicao Instituicao { get; set; }

        // 👔 Tipo de vínculo (Efetivo, Temporário, etc.)
        [Required]
        public int TipoVinculoId { get; set; }

        [ForeignKey(nameof(TipoVinculoId))]
        public TipoVinculo TipoVinculo { get; set; }

        // 🎓 Classe docente atual
        [Required]
        public int NivelId { get; set; }

        [ForeignKey(nameof(NivelId))]
        public Nivel Nivel { get; set; }

        // 📅 Datas
        [Required]
        public DateTime DataIngressoInstituicao { get; set; }

        public DateTime? DataUltimaProgressao { get; set; }

        // 🔄 Status do vínculo
        [Required]
        public bool Ativo { get; set; }
    }
}
