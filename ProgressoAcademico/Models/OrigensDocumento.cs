namespace ProgressoAcademico.Models;

public static class OrigensDocumento
{
    public const string ComprovatorioProfessor = "ComprovatorioProfessor";
    public const string DocumentoOficialUfabc = "DocumentoOficialUfabc";

    public static bool EhValida(string origem)
    {
        return origem == ComprovatorioProfessor
            || origem == DocumentoOficialUfabc;
    }
}
