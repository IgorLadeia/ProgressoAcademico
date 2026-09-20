namespace ProgressoAcademico.Application.DTOs.Elegibilidade;

public class RequisitoElegibilidadeDto
{
    public string Nome { get; set; } = string.Empty;
    public bool Atendido { get; set; }
    public bool Obrigatorio { get; set; } = true;
    public string Mensagem { get; set; } = string.Empty;
    public decimal ValorAtual { get; set; }
    public decimal ValorMinimo { get; set; }
    public string Unidade { get; set; } = string.Empty;
    public string? AcaoTexto { get; set; }
    public string? AcaoController { get; set; }
    public string? AcaoAction { get; set; }
}
