using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgressoAcademico.Models;

public class Usuario
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UsuarioId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string SenhaHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string PerfilAcesso { get; set; } = PerfisAcesso.Professor;

    [Required]
    public bool Ativo { get; set; }

    [Required]
    public bool PodeRevisar { get; set; }

    [Required]
    public DateTime DataCriacao { get; set; }

    public DateTime? DataExclusao { get; set; }
    public DateTime? DataUltimoProgresso { get; set; }

    public UsuarioPerfil? UsuarioPerfil { get; set; }
    public ICollection<VinculoInstitucional> VinculosInstitucionais { get; set; } = new List<VinculoInstitucional>();
    public ICollection<SolicitacaoProgressao> SolicitacaoProgressao { get; set; } = new List<SolicitacaoProgressao>();
    public ICollection<SolicitacaoRevisor> RevisoesAtribuidas { get; set; } = new List<SolicitacaoRevisor>();
}
