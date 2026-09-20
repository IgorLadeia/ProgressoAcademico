namespace ProgressoAcademico.Application.DTOs.Usuarios;

public class FotoPerfilDto
{
    public byte[] Arquivo { get; init; } = [];
    public string ContentType { get; init; } = string.Empty;
}
