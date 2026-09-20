using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressoAcademico.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarStatusAjustesSolicitados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO StatusSolicitacoes (Nome)
                SELECT 'Ajustes Solicitados'
                WHERE NOT EXISTS (
                    SELECT 1 FROM StatusSolicitacoes WHERE Nome = 'Ajustes Solicitados'
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM StatusSolicitacoes
                WHERE Nome = 'Ajustes Solicitados'
                  AND NOT EXISTS (
                      SELECT 1 FROM SolicitacoesProgressao sp
                      WHERE sp.StatusSolicitacaoId = StatusSolicitacoes.StatusSolicitacaoId
                  );
            ");
        }
    }
}
