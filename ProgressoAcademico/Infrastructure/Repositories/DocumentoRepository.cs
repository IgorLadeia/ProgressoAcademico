using Microsoft.EntityFrameworkCore;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Context;
using ProgressoAcademico.Models;

namespace ProgressoAcademico.Infrastructure.Repositories;

public class DocumentoRepository : IDocumentoRepository
{
    private readonly ProgressoAcademicoDbContext _context;

    public DocumentoRepository(ProgressoAcademicoDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Documento>> ListarPorSolicitacaoAsync(int solicitacaoProgressaoId, int usuarioId)
    {
        return await _context.Documentos
            .AsNoTracking()
            .Include(d => d.TipoDocumento)
            .Where(d => d.SolicitacaoProgressaoId == solicitacaoProgressaoId
                && d.SolicitacaoProgressao.UsuarioId == usuarioId)
            .OrderByDescending(d => d.DataUpload)
            .ToListAsync();
    }

    public Task<Documento?> ObterPorIdEUsuarioAsync(int documentoId, int usuarioId, bool incluirArquivo)
    {
        var query = _context.Documentos
            .Include(d => d.TipoDocumento)
            .Where(d => d.DocumentoId == documentoId && d.SolicitacaoProgressao.UsuarioId == usuarioId);

        if (!incluirArquivo)
            query = query.AsNoTracking();

        return query.FirstOrDefaultAsync();
    }

    public Task<bool> TipoDocumentoExisteAsync(int tipoDocumentoId)
    {
        return _context.TiposDocumento.AnyAsync(t => t.TipoDocumentoId == tipoDocumentoId);
    }

    public Task<TipoDocumento?> ObterTipoDocumentoComprovanteAtividadeAsync()
    {
        return _context.TiposDocumento
            .AsNoTracking()
            .Where(t => t.Tipo.Contains("Comprovante"))
            .OrderByDescending(t => t.Tipo == "Comprovante academico")
            .ThenBy(t => t.Tipo)
            .FirstOrDefaultAsync();
    }

    public Task<List<TipoDocumento>> ListarTiposDocumentoAsync()
    {
        return _context.TiposDocumento
            .AsNoTracking()
            .OrderBy(t => t.Tipo)
            .ToListAsync();
    }

    public async Task AdicionarAsync(Documento documento)
    {
        await _context.Documentos.AddAsync(documento);
    }

    public void Remover(Documento documento)
    {
        _context.Documentos.Remove(documento);
    }

    public Task SalvarAlteracoesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
