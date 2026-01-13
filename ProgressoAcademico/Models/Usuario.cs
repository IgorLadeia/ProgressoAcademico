using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UsuarioId { get; set; }

        [Required, MaxLength(200)]
        public string Nome { get; set; }

        [Required, EmailAddress, MaxLength(200)]
        public string Email { get; set; }

        [Required]
        public string SenhaHash { get; set; }

        [Required]
        public bool Ativo { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; }

        public DateTime? DataExclusao { get; set; }

        public DateTime? DataUltimoProgresso { get; set; }

        // 🔗 1:1 — um usuário possui exatamente um perfil
        public UsuarioPerfil UsuarioPerfil { get; set; }

        // 🔗 1:1 — um usuário possui exatamente um perfil
        public VinculoInstitucional VinculoInstitucional { get; set; }

        public ICollection<SolicitacaoProgressao> SolicitacaoProgressao { get; set; }
    }

}
