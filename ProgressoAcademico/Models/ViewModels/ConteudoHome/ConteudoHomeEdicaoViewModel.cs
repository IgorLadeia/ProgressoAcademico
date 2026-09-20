using System.ComponentModel.DataAnnotations;

namespace ProgressoAcademico.Models.ViewModels.ConteudoHome;

public class ConteudoHomeEdicaoViewModel
{
    [Required(ErrorMessage = "Informe o titulo principal.")]
    [MaxLength(220)]
    public string TituloPrincipal { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o resumo do projeto.")]
    [MaxLength(1200)]
    public string ResumoProjeto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o titulo de destaque.")]
    [MaxLength(180)]
    public string TituloDestaque { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe ao menos um destaque.")]
    [MaxLength(1600)]
    public string ItensDestaque { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o aviso de escopo.")]
    [MaxLength(500)]
    public string AvisoEscopo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a descricao do perfil Professor.")]
    [MaxLength(600)]
    public string DescricaoProfessor { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe os recursos do perfil Professor.")]
    [MaxLength(1600)]
    public string RecursosProfessor { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a descricao do perfil Administrador.")]
    [MaxLength(600)]
    public string DescricaoAdministrador { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe os recursos do perfil Administrador.")]
    [MaxLength(1600)]
    public string RecursosAdministrador { get; set; } = string.Empty;
}
