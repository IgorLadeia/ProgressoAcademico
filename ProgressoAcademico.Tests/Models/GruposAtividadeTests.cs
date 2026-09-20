using FluentAssertions;
using ProgressoAcademico.Models.Atividades;

namespace ProgressoAcademico.Tests.Models;

public class GruposAtividadeTests
{
    [Theory]
    [InlineData("Ensino", "Ensino")]
    [InlineData("Extensão", "Extensão")]
    [InlineData("Extensao", "Extensão")]
    [InlineData("ExtensÃ£o", "Extensão")]
    [InlineData("Gestão", "Gestão")]
    [InlineData("Gestao", "Gestão")]
    [InlineData("GestÃ£o", "Gestão")]
    [InlineData("Pesquisa", "Pesquisa")]
    public void ObterValidoOuPadrao_DeveRetornarGrupoCanonico(string entrada, string esperado)
    {
        var resultado = GruposAtividade.ObterValidoOuPadrao(entrada);

        resultado.Should().Be(esperado);
    }

    [Theory]
    [InlineData("Extensão", "Extensao")]
    [InlineData("Extensao", "Extensão")]
    [InlineData("ExtensÃ£o", "Extensão")]
    [InlineData("Gestão", "Gestao")]
    [InlineData("GestÃ£o", "Gestão")]
    public void MesmoGrupo_DeveCompararComNormalizacaoDosDoisLados(string valor, string grupo)
    {
        var resultado = GruposAtividade.MesmoGrupo(valor, grupo);

        resultado.Should().BeTrue();
    }

    [Theory]
    [InlineData("Extensão - Projeto de extensão (10 pts)", "Extensao")]
    [InlineData("ExtensÃ£o - Projeto de extensÃ£o (10 pts)", "Extensão")]
    [InlineData("Gestão - Coordenação (10 pts)", "Gestao")]
    [InlineData("GestÃ£o - CoordenaÃ§Ã£o (10 pts)", "Gestão")]
    public void ItemSelecaoPertenceAoGrupo_DeveFiltrarSubtiposPorGrupo(string texto, string grupo)
    {
        var resultado = GruposAtividade.ItemSelecaoPertenceAoGrupo(texto, grupo);

        resultado.Should().BeTrue();
    }
}
