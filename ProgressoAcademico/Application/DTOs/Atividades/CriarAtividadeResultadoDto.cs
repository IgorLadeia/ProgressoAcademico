namespace ProgressoAcademico.Application.DTOs.Atividades;

public class CriarAtividadeResultadoDto
{
    public bool Sucesso { get; set; }
    public int? AtividadeId { get; set; }
    public string? Erro { get; set; }

    public static CriarAtividadeResultadoDto Falha(string erro)
    {
        return new CriarAtividadeResultadoDto { Sucesso = false, Erro = erro };
    }

    public static CriarAtividadeResultadoDto Criada(int atividadeId)
    {
        return new CriarAtividadeResultadoDto { Sucesso = true, AtividadeId = atividadeId };
    }
}
