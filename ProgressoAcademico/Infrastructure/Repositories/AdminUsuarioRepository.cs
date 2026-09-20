using Microsoft.EntityFrameworkCore;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Context;
using ProgressoAcademico.Models;

namespace ProgressoAcademico.Infrastructure.Repositories;

public class AdminUsuarioRepository : IAdminUsuarioRepository
{
    private readonly ProgressoAcademicoDbContext _context;

    public AdminUsuarioRepository(ProgressoAcademicoDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Usuario>> ListarAsync()
    {
        return await _context.Usuarios
            .AsNoTracking()
            .Include(u => u.UsuarioPerfil)
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }

    public Task<Usuario?> ObterAsync(int usuarioId)
    {
        return _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);
    }

    public Task SalvarAlteracoesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
