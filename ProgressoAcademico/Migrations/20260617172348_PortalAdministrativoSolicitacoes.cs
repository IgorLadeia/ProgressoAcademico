using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressoAcademico.Migrations
{
    /// <inheritdoc />
    public partial class PortalAdministrativoSolicitacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataRevisao",
                table: "SolicitacoesProgressao",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RevisadoPorUsuarioId",
                table: "SolicitacoesProgressao",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesProgressao_RevisadoPorUsuarioId",
                table: "SolicitacoesProgressao",
                column: "RevisadoPorUsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitacoesProgressao_Usuarios_RevisadoPorUsuarioId",
                table: "SolicitacoesProgressao",
                column: "RevisadoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolicitacoesProgressao_Usuarios_RevisadoPorUsuarioId",
                table: "SolicitacoesProgressao");

            migrationBuilder.DropIndex(
                name: "IX_SolicitacoesProgressao_RevisadoPorUsuarioId",
                table: "SolicitacoesProgressao");

            migrationBuilder.DropColumn(
                name: "DataRevisao",
                table: "SolicitacoesProgressao");

            migrationBuilder.DropColumn(
                name: "RevisadoPorUsuarioId",
                table: "SolicitacoesProgressao");
        }
    }
}
