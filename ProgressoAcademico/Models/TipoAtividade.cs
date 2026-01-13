using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.ComponentModel.DataAnnotations;

public class TipoAtividade
{
    [Key]
    public int TipoAtividadeId { get; set; }

    [Required, MaxLength(150)]
    public string Nome { get; set; }
    // Ensino, Pesquisa, Extensão, Gestão


    public string? Descricao { get; set; }
    // Ensino, Pesquisa, Extensão, Gestão

    //Isso é um navigation property para a relação com SubTipoAtividade e serve para acessar os subtipos associados a esse tipo de atividade.
    //é associado a relacionamento um-para-muitos, onde um TipoAtividade pode ter múltiplos SubTipoAtividade associados a ele.
    public ICollection<SubTipoAtividade> Subtipos { get; set; }
}
