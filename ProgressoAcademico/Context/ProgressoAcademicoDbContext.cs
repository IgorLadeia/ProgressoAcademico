using Microsoft.EntityFrameworkCore;
using ProgressoAcademico.Models;

namespace ProgressoAcademico.Context
{
    public class ProgressoAcademicoDbContext : DbContext
    {
        public ProgressoAcademicoDbContext(DbContextOptions<ProgressoAcademicoDbContext> options)
            : base(options)
        {
        }

        /* ==========================
           DBSETS (TABELAS)
           ========================== */

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioPerfil> UsuariosPerfis { get; set; }
        public DbSet<VinculoInstitucional> VinculosInstitucionais { get; set; }
        public DbSet<TipoVinculo> TiposVinculo { get; set; }
        public DbSet<ClasseDocente> ClassesDocente { get; set; }

        public DbSet<SolicitacaoProgressao> SolicitacoesProgressao { get; set; }
        public DbSet<StatusSolicitacao> StatusSolicitacoes { get; set; }
        public DbSet<TipoProgresso> TiposProgresso { get; set; }
        public DbSet<Nivel> Niveis { get; set; }

        public DbSet<Atividade> Atividades { get; set; }
        public DbSet<TipoAtividade> TiposAtividade { get; set; }
        public DbSet<SubTipoAtividade> SubtiposAtividade { get; set; }
        public DbSet<AtividadeEnsino> AtividadesEnsino { get; set; }
        public DbSet<AtividadePesquisa> AtividadesPesquisa { get; set; }
        public DbSet<AtividadeExtensao> AtividadesExtensao { get; set; }
        public DbSet<AtividadeAdministrativa> AtividadesAdministrativas { get; set; }

        public DbSet<Documento> Documentos { get; set; }
        public DbSet<TipoDocumento> TiposDocumento { get; set; }

        public DbSet<Instituicao> Instituicoes { get; set; }

        /* ==========================
           CONFIGURAÇÕES
           ========================== */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /* ==========================
               USUARIO ↔ PERFIL (1:1)
               ========================== */
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.UsuarioPerfil)
                .WithOne(p => p.Usuario)
                .HasForeignKey<UsuarioPerfil>(p => p.UsuarioId);

            /* ==========================
               USUARIO ↔ VINCULO INSTITUCIONAL (1:1, PK = FK)
               ========================== */
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.VinculoInstitucional)
                .WithOne(v => v.Usuario)
                .HasForeignKey<VinculoInstitucional>(v => v.UsuarioId);

            /* ==========================
               ATIVIDADE ↔ ESPECIALIZAÇÕES (1:1)
               ========================== */
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

            /* ==========================
               SUBTIPO ↔ TIPO ATIVIDADE (N:1)
               ========================== */
            modelBuilder.Entity<SubTipoAtividade>()
                .HasOne(st => st.TipoAtividade)
                .WithMany(t => t.Subtipos)
                .HasForeignKey(st => st.TipoAtividadeId)
                .OnDelete(DeleteBehavior.Restrict);

            /* ==========================
               SOLICITACAO_PROGRESSÃO ↔ NÍVEL (ORIGEM / DESTINO) (N:1)
               ========================== */
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

            /* ==========================
               DOCUMENTO ↔ ATIVIDADE (OPCIONAL)
               ========================== */
            modelBuilder.Entity<Documento>()
                .HasOne(d => d.Atividade)
                .WithMany(a => a.Documentos)
                .HasForeignKey(d => d.AtividadeId)
                .OnDelete(DeleteBehavior.SetNull);

            /* ==========================
               OUTROS RELACIONAMENTOS (TIPO DOCUMENTO, INSTITUICAO, TIPO VINCULO, CLASSE DOCENTE)
               ========================== */
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
                .HasOne(v => v.ClasseDocente)
                .WithMany(c => c.VinculosInstitucionais)
                .HasForeignKey(v => v.ClasseDocenteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Documento>()
                .HasOne(d => d.TipoDocumento)
                .WithMany(td => td.Documentos)
                .HasForeignKey(d => d.TipoDocumentoId)
                .OnDelete(DeleteBehavior.Restrict);

            /* ==========================
               ÍNDICES IMPORTANTES
               ========================== */
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
