namespace ProgressoAcademico.Models;

public static class PerfisAcesso
{
    public const string Professor = "Professor";
    public const string Revisor = "Revisor";
    public const string Administrador = "Administrador";

    public static bool EhValido(string perfil)
    {
        return perfil == Professor || perfil == Revisor || perfil == Administrador;
    }
}
