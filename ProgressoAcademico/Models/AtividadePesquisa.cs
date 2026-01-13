using ProgressoAcademico.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class AtividadePesquisa
{
    [Key, ForeignKey(nameof(Atividade))]
    public int AtividadeId { get; set; }

    [MaxLength(100)]
    public string TipoProducao { get; set; }

    [MaxLength(100)]
    public string VeiculoPublicacao { get; set; }

    [MaxLength(100)]
    public string ISSN_ISBN { get; set; }

    public int NumeroAutores { get; set; }


    public Atividade Atividade { get; set; }
}
