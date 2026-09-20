using System.Globalization;
using System.Text;

namespace ProgressoAcademico.Models.Atividades;

public static class GruposAtividade
{
    public const string Ensino = "Ensino";
    public const string Extensao = "Extensão";
    public const string Gestao = "Gestão";
    public const string Pesquisa = "Pesquisa";

    public static IReadOnlyList<string> Todos { get; } =
    [
        Ensino,
        Extensao,
        Gestao,
        Pesquisa
    ];

    public static string ObterValidoOuPadrao(string? grupo)
    {
        var chave = Normalizar(grupo);
        return Todos.FirstOrDefault(item => MesmoGrupo(item, chave)) ?? Ensino;
    }

    public static bool MesmoGrupo(string? valor, string? grupo)
    {
        return string.Equals(Normalizar(valor), Normalizar(grupo), StringComparison.OrdinalIgnoreCase);
    }

    public static bool ItemSelecaoPertenceAoGrupo(string? texto, string? grupo)
    {
        var textoNormalizado = Normalizar(texto);
        var grupoNormalizado = Normalizar(grupo);

        return textoNormalizado.StartsWith($"{grupoNormalizado} - ", StringComparison.OrdinalIgnoreCase);
    }

    public static string Normalizar(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return string.Empty;

        var textoCorrigido = CorrigirMojibake(texto);
        var decomposed = textoCorrigido.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);

        foreach (var caractere in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(caractere) != UnicodeCategory.NonSpacingMark)
                builder.Append(caractere);
        }

        return builder.ToString()
            .Normalize(NormalizationForm.FormC)
            .Trim()
            .ToUpperInvariant();
    }

    private static string CorrigirMojibake(string texto)
    {
        return texto
            .Replace("ÃƒÂ£", "ã", StringComparison.OrdinalIgnoreCase)
            .Replace("Ã£", "ã", StringComparison.OrdinalIgnoreCase)
            .Replace("ÃƒÂ§", "ç", StringComparison.OrdinalIgnoreCase)
            .Replace("Ã§", "ç", StringComparison.OrdinalIgnoreCase)
            .Replace("ÃƒÂ©", "é", StringComparison.OrdinalIgnoreCase)
            .Replace("Ã©", "é", StringComparison.OrdinalIgnoreCase);
    }
}
