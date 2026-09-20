using Microsoft.EntityFrameworkCore;
using ProgressoAcademico.Models;

namespace ProgressoAcademico.Context;

public class ProgressoAcademicoDbContext : DbContext
{
    public ProgressoAcademicoDbContext(DbContextOptions<ProgressoAcademicoDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<UsuarioPerfil> UsuariosPerfis { get; set; }
    public DbSet<VinculoInstitucional> VinculosInstitucionais { get; set; }
    public DbSet<TipoVinculo> TiposVinculo { get; set; }
    public DbSet<SolicitacaoProgressao> SolicitacoesProgressao { get; set; }
    public DbSet<SolicitacaoRevisor> SolicitacoesRevisores { get; set; }
    public DbSet<StatusSolicitacao> StatusSolicitacoes { get; set; }
    public DbSet<TipoProgresso> TiposProgresso { get; set; }
    public DbSet<Nivel> Niveis { get; set; }
    public DbSet<Atividade> Atividades { get; set; }
    public DbSet<AtividadeAvaliacaoRevisor> AtividadesAvaliacoesRevisores { get; set; }
    public DbSet<TipoAtividade> TiposAtividade { get; set; }
    public DbSet<SubTipoAtividade> SubtiposAtividade { get; set; }
    public DbSet<AtividadeEnsino> AtividadesEnsino { get; set; }
    public DbSet<AtividadePesquisa> AtividadesPesquisa { get; set; }
    public DbSet<AtividadeExtensao> AtividadesExtensao { get; set; }
    public DbSet<AtividadeAdministrativa> AtividadesAdministrativas { get; set; }
    public DbSet<Documento> Documentos { get; set; }
    public DbSet<TipoDocumento> TiposDocumento { get; set; }
    public DbSet<Instituicao> Instituicoes { get; set; }
    public DbSet<Comunicado> Comunicados { get; set; }
    public DbSet<ConteudoHome> ConteudosHome { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigurarUsuarios(modelBuilder);
        ConfigurarAtividades(modelBuilder);
        ConfigurarSolicitacoes(modelBuilder);
        ConfigurarDocumentos(modelBuilder);
        ConfigurarVinculosInstitucionais(modelBuilder);
        ConfigurarComunicados(modelBuilder);
        ConfigurarConteudoHome(modelBuilder);
    }

    private static void ConfigurarUsuarios(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.UsuarioPerfil)
            .WithOne(p => p.Usuario)
            .HasForeignKey<UsuarioPerfil>(p => p.UsuarioId);

        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.VinculosInstitucionais)
            .WithOne(v => v.Usuario)
            .HasForeignKey(v => v.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Usuario>()
            .Property(u => u.PerfilAcesso)
            .HasMaxLength(30)
            .HasDefaultValue(PerfisAcesso.Professor);

        modelBuilder.Entity<Usuario>()
            .Property(u => u.PodeRevisar)
            .HasDefaultValue(false);

        modelBuilder.Entity<UsuarioPerfil>()
            .Property(p => p.FotoPerfilContentType)
            .HasMaxLength(50);
    }

    private static void ConfigurarAtividades(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Atividade>()
            .HasOne(a => a.AtividadeEnsino)
            .WithOne(e => e.Atividade)
            .HasForeignKey<AtividadeEnsino>(e => e.AtividadeId);

        modelBuilder.Entity<Atividade>()
            .HasOne(a => a.AtividadePesquisa)
            .WithOne(p => p.Atividade)
            .HasForeignKey<AtividadePesquisa>(p => p.AtividadeId);

        modelBuilder.Entity<Atividade>()
            .HasOne(a => a.AtividadeExtensao)
            .WithOne(x => x.Atividade)
            .HasForeignKey<AtividadeExtensao>(x => x.AtividadeId);

        modelBuilder.Entity<Atividade>()
            .HasOne(a => a.AtividadeAdministrativa)
            .WithOne(ad => ad.Atividade)
            .HasForeignKey<AtividadeAdministrativa>(ad => ad.AtividadeId);

        modelBuilder.Entity<Atividade>()
            .HasIndex(a => a.SolicitacaoProgressaoId);

        modelBuilder.Entity<AtividadeAvaliacaoRevisor>()
            .HasOne(a => a.Atividade)
            .WithMany(a => a.AvaliacoesRevisores)
            .HasForeignKey(a => a.AtividadeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AtividadeAvaliacaoRevisor>()
            .HasOne(a => a.SolicitacaoRevisor)
            .WithMany(sr => sr.AvaliacoesAtividades)
            .HasForeignKey(a => a.SolicitacaoRevisorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AtividadeAvaliacaoRevisor>()
            .HasIndex(a => new { a.AtividadeId, a.SolicitacaoRevisorId })
            .IsUnique();

        modelBuilder.Entity<AtividadeAvaliacaoRevisor>()
            .Property(a => a.Resultado)
            .HasMaxLength(30);

        modelBuilder.Entity<AtividadeAvaliacaoRevisor>()
            .Property(a => a.Parecer)
            .HasMaxLength(1500);

        modelBuilder.Entity<SubTipoAtividade>()
            .HasOne(st => st.TipoAtividade)
            .WithMany(t => t.Subtipos)
            .HasForeignKey(st => st.TipoAtividadeId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigurarSolicitacoes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SolicitacaoProgressao>()
            .HasOne(s => s.NivelOrigem)
            .WithMany(n => n.SolicitacoesComoOrigem)
            .HasForeignKey(s => s.NivelOrigemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SolicitacaoProgressao>()
            .HasOne(s => s.NivelDestino)
            .WithMany(n => n.SolicitacoesComoDestino)
            .HasForeignKey(s => s.NivelDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SolicitacaoProgressao>()
            .HasOne(s => s.RevisadoPorUsuario)
            .WithMany()
            .HasForeignKey(s => s.RevisadoPorUsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<SolicitacaoProgressao>()
            .HasOne(s => s.RevisorUsuario)
            .WithMany()
            .HasForeignKey(s => s.RevisorUsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<SolicitacaoProgressao>()
            .HasOne(s => s.AtribuidoPorUsuario)
            .WithMany()
            .HasForeignKey(s => s.AtribuidoPorUsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<SolicitacaoProgressao>()
            .Property(s => s.ParecerRevisor)
            .HasMaxLength(2000);

        modelBuilder.Entity<SolicitacaoProgressao>()
            .Property(s => s.Multiprogressao)
            .HasDefaultValue(false);

        modelBuilder.Entity<SolicitacaoProgressao>()
            .HasIndex(s => s.UsuarioId);

        modelBuilder.Entity<SolicitacaoProgressao>()
            .HasIndex(s => s.StatusSolicitacaoId);

        modelBuilder.Entity<SolicitacaoProgressao>()
            .HasIndex(s => new { s.UsuarioId, s.StatusSolicitacaoId });

        modelBuilder.Entity<SolicitacaoProgressao>()
            .HasIndex(s => s.RevisadoPorUsuarioId);

        modelBuilder.Entity<SolicitacaoProgressao>()
            .HasIndex(s => s.RevisorUsuarioId);

        modelBuilder.Entity<SolicitacaoProgressao>()
            .HasIndex(s => s.AtribuidoPorUsuarioId);

        modelBuilder.Entity<SolicitacaoRevisor>()
            .HasOne(sr => sr.SolicitacaoProgressao)
            .WithMany(s => s.Revisores)
            .HasForeignKey(sr => sr.SolicitacaoProgressaoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SolicitacaoRevisor>()
            .HasOne(sr => sr.RevisorUsuario)
            .WithMany(u => u.RevisoesAtribuidas)
            .HasForeignKey(sr => sr.RevisorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SolicitacaoRevisor>()
            .HasOne(sr => sr.AtribuidoPorUsuario)
            .WithMany()
            .HasForeignKey(sr => sr.AtribuidoPorUsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<SolicitacaoRevisor>()
            .HasIndex(sr => new { sr.SolicitacaoProgressaoId, sr.RevisorUsuarioId })
            .IsUnique();

        modelBuilder.Entity<SolicitacaoRevisor>()
            .HasIndex(sr => sr.RevisorUsuarioId);

        modelBuilder.Entity<SolicitacaoRevisor>()
            .Property(sr => sr.StatusRevisao)
            .HasMaxLength(30);

        modelBuilder.Entity<SolicitacaoRevisor>()
            .Property(sr => sr.Parecer)
            .HasMaxLength(2000);
    }

    private static void ConfigurarDocumentos(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Documento>()
            .HasOne(d => d.Atividade)
            .WithMany(a => a.Documentos)
            .HasForeignKey(d => d.AtividadeId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Documento>()
            .HasOne(d => d.TipoDocumento)
            .WithMany(td => td.Documentos)
            .HasForeignKey(d => d.TipoDocumentoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Documento>()
            .HasIndex(d => d.SolicitacaoProgressaoId);

        modelBuilder.Entity<Documento>()
            .HasIndex(d => d.AtividadeId);

        modelBuilder.Entity<Documento>()
            .HasIndex(d => d.HashSha256);

        modelBuilder.Entity<Documento>()
            .Property(d => d.NomeArquivo)
            .HasMaxLength(255);

        modelBuilder.Entity<Documento>()
            .Property(d => d.ContentType)
            .HasMaxLength(100)
            .HasDefaultValue("application/octet-stream");

        modelBuilder.Entity<Documento>()
            .Property(d => d.OrigemDocumento)
            .HasMaxLength(30)
            .HasDefaultValue(OrigensDocumento.ComprovatorioProfessor);

        modelBuilder.Entity<Documento>()
            .Property(d => d.HashSha256)
            .HasMaxLength(64);

        modelBuilder.Entity<Documento>()
            .Property(d => d.Observacao)
            .HasMaxLength(1000);
    }

    private static void ConfigurarVinculosInstitucionais(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VinculoInstitucional>()
            .HasOne(v => v.Instituicao)
            .WithMany(i => i.Vinculos)
            .HasForeignKey(v => v.InstituicaoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VinculoInstitucional>()
            .HasOne(v => v.TipoVinculo)
            .WithMany(t => t.VinculoInstitucional)
            .HasForeignKey(v => v.TipoVinculoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VinculoInstitucional>()
            .HasOne(v => v.Nivel)
            .WithMany(c => c.VinculoInstitucionals)
            .HasForeignKey(v => v.NivelId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VinculoInstitucional>()
            .HasIndex(v => v.UsuarioId);
    }

    private static void ConfigurarComunicados(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comunicado>()
            .HasOne(c => c.CriadoPorUsuario)
            .WithMany()
            .HasForeignKey(c => c.CriadoPorUsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Comunicado>()
            .HasIndex(c => new { c.Publicado, c.DataPublicacao });

        modelBuilder.Entity<Comunicado>()
            .Property(c => c.Titulo)
            .HasMaxLength(180);

        modelBuilder.Entity<Comunicado>()
            .Property(c => c.Resumo)
            .HasMaxLength(350);

        modelBuilder.Entity<Comunicado>()
            .Property(c => c.Categoria)
            .HasMaxLength(80);
    }

    private static void ConfigurarConteudoHome(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConteudoHome>()
            .HasOne(c => c.AtualizadoPorUsuario)
            .WithMany()
            .HasForeignKey(c => c.AtualizadoPorUsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ConteudoHome>()
            .HasData(new ConteudoHome
            {
                ConteudoHomeId = 1,
                TituloPrincipal = "Sistema integrado para apoio à progressão acadêmica docente.",
                ResumoProjeto = "Aplicação web funcional desenvolvida para organizar solicitações, atividades, documentos, revisões técnicas e decisão administrativa no processo de progressão acadêmica dos professores da Universidade Federal do ABC.",
                TituloDestaque = "Fluxo estruturado para professor, revisor e administrador.",
                ItensDestaque = "Solicitações e atividades centralizadas\nMúltiplos documentos por atividade\nRevisão técnica por professores avaliadores\nDecisão administrativa com parecer registrado",
                AvisoEscopo = "Sistema acadêmico funcional de apoio. Não substitui normas, sistemas oficiais ou atos administrativos da UFABC.",
                DescricaoProfessor = "Prepara e acompanha a própria solicitação, registra atividades e vincula documentos comprobatórios.",
                RecursosProfessor = "Cria e acompanha solicitações\nCadastra atividades de ensino, pesquisa, extensão e gestão\nAnexa múltiplos comprovantes por atividade\nConsulta histórico, status e pendências",
                DescricaoAdministrador = "Coordena a análise das solicitações, atribui revisores, consulta pareceres e registra a decisão final.",
                RecursosAdministrador = "Atribui um ou mais revisores por solicitação\nConsulta pareceres técnicos e documentos\nSolicita ajustes quando necessário\nRegistra aprovação, rejeição ou encerramento",
                DataAtualizacao = new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc)
            });
    }
}
