namespace ProgressoAcademico.Application.DTOs.ConteudoHome;

public class ConteudoHomeDto
{
    public string TituloPrincipal { get; init; } = string.Empty;
    public string ResumoProjeto { get; init; } = string.Empty;
    public string TituloDestaque { get; init; } = string.Empty;
    public IReadOnlyCollection<string> ItensDestaque { get; init; } = [];
    public string AvisoEscopo { get; init; } = string.Empty;
    public string DescricaoProfessor { get; init; } = string.Empty;
    public IReadOnlyCollection<string> RecursosProfessor { get; init; } = [];
    public string DescricaoAdministrador { get; init; } = string.Empty;
    public IReadOnlyCollection<string> RecursosAdministrador { get; init; } = [];
    public DateTime DataAtualizacao { get; init; }
}
