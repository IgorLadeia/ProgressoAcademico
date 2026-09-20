using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressoAcademico.Migrations
{
    /// <inheritdoc />
    public partial class RevisoresMultiplosSolicitacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PodeRevisar",
                table: "Usuarios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SolicitacoesRevisores",
                columns: table => new
                {
                    SolicitacaoRevisorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SolicitacaoProgressaoId = table.Column<int>(type: "int", nullable: false),
                    RevisorUsuarioId = table.Column<int>(type: "int", nullable: false),
                    AtribuidoPorUsuarioId = table.Column<int>(type: "int", nullable: true),
                    DataAtribuicao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    StatusRevisao = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Parecer = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataParecer = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitacoesRevisores", x => x.SolicitacaoRevisorId);
                    table.ForeignKey(
                        name: "FK_SolicitacoesRevisores_SolicitacoesProgressao_SolicitacaoProg~",
                        column: x => x.SolicitacaoProgressaoId,
                        principalTable: "SolicitacoesProgressao",
                        principalColumn: "SolicitacaoProgressaoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitacoesRevisores_Usuarios_AtribuidoPorUsuarioId",
                        column: x => x.AtribuidoPorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SolicitacoesRevisores_Usuarios_RevisorUsuarioId",
                        column: x => x.RevisorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesRevisores_AtribuidoPorUsuarioId",
                table: "SolicitacoesRevisores",
                column: "AtribuidoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesRevisores_RevisorUsuarioId",
                table: "SolicitacoesRevisores",
                column: "RevisorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesRevisores_SolicitacaoProgressaoId_RevisorUsuario~",
                table: "SolicitacoesRevisores",
                columns: new[] { "SolicitacaoProgressaoId", "RevisorUsuarioId" },
                unique: true);

            migrationBuilder.Sql(@"
                UPDATE Usuarios
                SET PodeRevisar = 1,
                    PerfilAcesso = 'Professor'
                WHERE PerfilAcesso = 'Revisor';

                INSERT INTO SolicitacoesRevisores
                    (SolicitacaoProgressaoId, RevisorUsuarioId, AtribuidoPorUsuarioId, DataAtribuicao, StatusRevisao, Parecer, DataParecer)
                SELECT sp.SolicitacaoProgressaoId,
                       sp.RevisorUsuarioId,
                       sp.AtribuidoPorUsuarioId,
                       COALESCE(sp.DataAtribuicaoRevisor, sp.DataCriacao, UTC_TIMESTAMP()),
                       CASE
                           WHEN sp.StatusSolicitacaoId = (SELECT StatusSolicitacaoId FROM StatusSolicitacoes WHERE Nome = 'Aguardando Decisao Final' LIMIT 1)
                                THEN 'EncaminhadaAoAdm'
                           WHEN sp.StatusSolicitacaoId = (SELECT StatusSolicitacaoId FROM StatusSolicitacoes WHERE Nome = 'Ajustes Solicitados' LIMIT 1)
                                THEN 'AjustesSolicitados'
                           ELSE 'EmRevisao'
                       END,
                       sp.ParecerRevisor,
                       sp.DataParecerRevisor
                FROM SolicitacoesProgressao sp
                WHERE sp.RevisorUsuarioId IS NOT NULL
                  AND NOT EXISTS (
                      SELECT 1
                      FROM SolicitacoesRevisores sr
                      WHERE sr.SolicitacaoProgressaoId = sp.SolicitacaoProgressaoId
                        AND sr.RevisorUsuarioId = sp.RevisorUsuarioId
                  );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitacoesRevisores");

            migrationBuilder.DropColumn(
                name: "PodeRevisar",
                table: "Usuarios");
        }
    }
}
