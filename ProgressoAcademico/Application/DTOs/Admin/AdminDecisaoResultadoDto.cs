namespace ProgressoAcademico.Application.DTOs.Admin;

public class AdminDecisaoResultadoDto
{
    public bool Sucesso { get; set; }
    public string? Erro { get; set; }

    public static AdminDecisaoResultadoDto Ok() => new() { Sucesso = true };
    public static AdminDecisaoResultadoDto Falha(string erro) => new() { Sucesso = false, Erro = erro };
}
