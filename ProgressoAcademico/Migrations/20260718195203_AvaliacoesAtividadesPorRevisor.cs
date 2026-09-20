using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressoAcademico.Migrations
{
    /// <inheritdoc />
    public partial class AvaliacoesAtividadesPorRevisor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AtividadesAvaliacoesRevisores",
                columns: table => new
                {
                    AtividadeAvaliacaoRevisorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AtividadeId = table.Column<int>(type: "int", nullable: false),
                    SolicitacaoRevisorId = table.Column<int>(type: "int", nullable: false),
                    Resultado = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Parecer = table.Column<string>(type: "varchar(1500)", maxLength: 1500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataAvaliacao = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtividadesAvaliacoesRevisores", x => x.AtividadeAvaliacaoRevisorId);
                    table.ForeignKey(
                        name: "FK_AtividadesAvaliacoesRevisores_Atividades_AtividadeId",
                        column: x => x.AtividadeId,
                        principalTable: "Atividades",
                        principalColumn: "AtividadeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AtividadesAvaliacoesRevisores_SolicitacoesRevisores_Solicita~",
                        column: x => x.SolicitacaoRevisorId,
                        principalTable: "SolicitacoesRevisores",
                        principalColumn: "SolicitacaoRevisorId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AtividadesAvaliacoesRevisores_AtividadeId_SolicitacaoRevisor~",
                table: "AtividadesAvaliacoesRevisores",
                columns: new[] { "AtividadeId", "SolicitacaoRevisorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AtividadesAvaliacoesRevisores_SolicitacaoRevisorId",
                table: "AtividadesAvaliacoesRevisores",
                column: "SolicitacaoRevisorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AtividadesAvaliacoesRevisores");
        }
    }
}
