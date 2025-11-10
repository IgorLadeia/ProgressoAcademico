namespace ProgressoAcademico.Models
{
    public class PerfilUsuario
    {
        public int UsuarioId { get; set; } 
        public string? UsuarioApelido { get; set; } 
        public string? UsuarioNome { get; set; }
        public string? UsuarioSobreNome { get; set; }
        public string? UsuarioTelephone { get; set; }    
        public DateOnly UsuarioDataNascimento { get; set; }
        
    }
}
