using Microsoft.EntityFrameworkCore;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Context;
using ProgressoAcademico.Models;
using ProgressoAcademico.Application.DTOs.Usuarios;

namespace ProgressoAcademico.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ProgressoAcademicoDbContext _context;

    public UsuarioRepository(ProgressoAcademicoDbContext context)
    {
        _context = context;
    }

    public Task<bool> EmailExisteAsync(string email)
    {
        return _context.Usuarios.AnyAsync(u => u.Email == email);
    }

    public Task<Usuario?> ObterPorEmailAsync(string email, bool asNoTracking = true)
    {
        var query = _context.Usuarios
            .Include(u => u.UsuarioPerfil)
            .AsQueryable();

        if (asNoTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefaultAsync(u => u.Email == email);
    }

    public Task<Usuario?> ObterAtivoPorIdComPerfilAsync(int usuarioId)
    {
        return _context.Usuarios
            .Include(u => u.UsuarioPerfil)
            .Include(u => u.VinculosInstitucionais.Where(v => v.Ativo))
            .FirstOrDefaultAsync(u => u.UsuarioId == usuarioId && u.Ativo);
    }

    public Task<bool> EmailExisteParaOutroUsuarioAsync(string email, int usuarioId)
    {
        return _context.Usuarios.AnyAsync(u => u.Email == email && u.UsuarioId != usuarioId);
    }

    public Task<ProfessorPerfilDto?> ObterResumoPerfilProfessorAsync(int usuarioId)
    {
        return _context.Usuarios
            .AsNoTracking()
            .Where(u => u.UsuarioId == usuarioId
                && u.Ativo
                && u.PerfilAcesso == PerfisAcesso.Professor)
            .Select(u => new ProfessorPerfilDto
            {
                UsuarioId = u.UsuarioId,
                NomeCompleto = u.Nome,
                NomeExibicao = u.UsuarioPerfil != null && u.UsuarioPerfil.Apelido != null
                    ? u.UsuarioPerfil.Apelido
                    : u.Nome,
                Email = u.Email,
                NomeSocial = u.UsuarioPerfil != null ? u.UsuarioPerfil.Apelido : null,
                PossuiFoto = u.UsuarioPerfil != null
                    && (u.UsuarioPerfil.FotoPerfilContentType != null
                        || u.UsuarioPerfil.FotoPerfil != null),
                FotoPerfilUrl = u.UsuarioPerfil != null
                    && u.UsuarioPerfil.FotoPerfil != null
                    && u.UsuarioPerfil.FotoPerfil.StartsWith("/images/")
                        ? u.UsuarioPerfil.FotoPerfil
                        : null,
                DataUltimoProgresso = u.DataUltimoProgresso,
                InstituicaoId = u.VinculosInstitucionais
                    .Where(v => v.Ativo)
                    .Select(v => (int?)v.InstituicaoId)
                    .FirstOrDefault(),
                TipoVinculoId = u.VinculosInstitucionais
                    .Where(v => v.Ativo)
                    .Select(v => (int?)v.TipoVinculoId)
                    .FirstOrDefault(),
                NivelId = u.VinculosInstitucionais
                    .Where(v => v.Ativo)
                    .Select(v => (int?)v.NivelId)
                    .FirstOrDefault(),
                DataIngressoInstituicao = u.VinculosInstitucionais
                    .Where(v => v.Ativo)
                    .Select(v => (DateTime?)v.DataIngressoInstituicao)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();
    }

    public async Task AdicionarAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
    }

    public async Task AdicionarPerfilAsync(UsuarioPerfil perfil)
    {
        await _context.UsuariosPerfis.AddAsync(perfil);
    }

    public async Task AdicionarVinculoAsync(VinculoInstitucional vinculo)
    {
        await _context.VinculosInstitucionais.AddAsync(vinculo);
    }

    public Task SalvarAlteracoesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
