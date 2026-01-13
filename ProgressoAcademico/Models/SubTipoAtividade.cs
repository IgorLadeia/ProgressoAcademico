using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SubTipoAtividade
{
    [Key]
    public int SubtipoAtividadeId { get; set; }

    //estamos fazendo uma referência ao TipoAtividadeId, que é a chave estrangeira que liga o SubTipoAtividade ao seu TipoAtividade correspondente.
    [Required]
    public int TipoAtividadeId { get; set; }

    //aqui estamos definindo a propriedade de navegação TipoAtividade, que permite acessar o objeto TipoAtividade associado a este SubTipoAtividade.
    [ForeignKey(nameof(TipoAtividadeId))]
    public TipoAtividade TipoAtividade { get; set; }


    [Required, MaxLength(150)]
    public string Nome { get; set; }

    public string? Descricao { get; set; }

    // Ensino, Pesquisa, Extensão, Gestão


}
