namespace ProgressoAcademico.Application.DTOs.Usuarios;

public class ProfessorPerfilDto
{
    public int UsuarioId { get; init; }
    public string NomeCompleto { get; init; } = string.Empty;
    public string NomeExibicao { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? NomeSocial { get; init; }
    public bool PossuiFoto { get; init; }
    public string? FotoPerfilUrl { get; init; }
    public DateTime? DataUltimoProgresso { get; init; }
    public int? InstituicaoId { get; init; }
    public int? TipoVinculoId { get; init; }
    public int? NivelId { get; init; }
    public DateTime? DataIngressoInstituicao { get; init; }
}
