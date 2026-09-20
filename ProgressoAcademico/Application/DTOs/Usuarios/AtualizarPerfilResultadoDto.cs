namespace ProgressoAcademico.Application.DTOs.Usuarios;

public class AtualizarPerfilResultadoDto
{
    public bool Sucesso { get; init; }
    public string? Erro { get; init; }

    public static AtualizarPerfilResultadoDto Atualizado() => new() { Sucesso = true };

    public static AtualizarPerfilResultadoDto Falha(string erro) => new() { Erro = erro };
}
