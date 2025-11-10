using System.Diagnostics.Contracts;

namespace ProgressoAcademico.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string? UsuarioNome { get; set; }
        public string? UsuarioEmail { get; set; }   
        public string? UsuarioSenha { get; set; }

        public DateOnly UsuarioDataCadastro { get; set; }  
         
        public Boolean UsuarioAtivo { get; set; }   



    }

}
