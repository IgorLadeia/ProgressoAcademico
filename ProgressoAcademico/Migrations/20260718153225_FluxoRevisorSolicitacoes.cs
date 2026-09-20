using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressoAcademico.Migrations
{
    /// <inheritdoc />
    public partial class FluxoRevisorSolicitacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AtribuidoPorUsuarioId",
                table: "SolicitacoesProgressao",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAtribuicaoRevisor",
                table: "SolicitacoesProgressao",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataParecerRevisor",
                table: "SolicitacoesProgressao",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Multiprogressao",
                table: "SolicitacoesProgressao",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ParecerRevisor",
                table: "SolicitacoesProgressao",
                type: "varchar(2000)",
                maxLength: 2000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "RevisorUsuarioId",
                table: "SolicitacoesProgressao",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesProgressao_AtribuidoPorUsuarioId",
                table: "SolicitacoesProgressao",
                column: "AtribuidoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesProgressao_RevisorUsuarioId",
                table: "SolicitacoesProgressao",
                column: "RevisorUsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitacoesProgressao_Usuarios_AtribuidoPorUsuarioId",
                table: "SolicitacoesProgressao",
                column: "AtribuidoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitacoesProgressao_Usuarios_RevisorUsuarioId",
                table: "SolicitacoesProgressao",
                column: "RevisorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.Sql(@"
                INSERT INTO StatusSolicitacoes (Nome)
                SELECT 'Aguardando Atribuicao'
                WHERE NOT EXISTS (SELECT 1 FROM StatusSolicitacoes WHERE Nome = 'Aguardando Atribuicao');

                INSERT INTO StatusSolicitacoes (Nome)
                SELECT 'Em Revisao'
                WHERE NOT EXISTS (SELECT 1 FROM StatusSolicitacoes WHERE Nome = 'Em Revisao');

                INSERT INTO StatusSolicitacoes (Nome)
                SELECT 'Aguardando Decisao Final'
                WHERE NOT EXISTS (SELECT 1 FROM StatusSolicitacoes WHERE Nome = 'Aguardando Decisao Final');
            ");

            migrationBuilder.Sql(@"
                INSERT INTO Usuarios (Nome, Email, SenhaHash, PerfilAcesso, Ativo, DataCriacao, DataExclusao, DataUltimoProgresso)
                SELECT 'Revisor Padrao', 'revisor@ufabc.edu.br', '$2b$10$p7.K0fwHFoI3FoJx7DaZue9ImQ/dPjnidyYfBT036bRqiglSdChz.', 'Revisor', 1, UTC_TIMESTAMP(), NULL, '2024-02-10'
                WHERE NOT EXISTS (SELECT 1 FROM Usuarios WHERE Email = 'revisor@ufabc.edu.br');

                INSERT INTO UsuariosPerfis (UsuarioId, Apelido, FotoPerfil)
                SELECT u.UsuarioId, 'Revisor', NULL
                FROM Usuarios u
                WHERE u.Email = 'revisor@ufabc.edu.br'
                  AND NOT EXISTS (SELECT 1 FROM UsuariosPerfis p WHERE p.UsuarioId = u.UsuarioId);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolicitacoesProgressao_Usuarios_AtribuidoPorUsuarioId",
                table: "SolicitacoesProgressao");

            migrationBuilder.Sql(@"
                DELETE FROM UsuariosPerfis
                WHERE UsuarioId IN (
                    SELECT UsuarioId FROM Usuarios
                    WHERE Email = 'revisor@ufabc.edu.br'
                );

                DELETE FROM Usuarios
                WHERE Email = 'revisor@ufabc.edu.br';

                DELETE FROM StatusSolicitacoes
                WHERE Nome IN ('Aguardando Atribuicao', 'Em Revisao', 'Aguardando Decisao Final');
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_SolicitacoesProgressao_Usuarios_RevisorUsuarioId",
                table: "SolicitacoesProgressao");

            migrationBuilder.DropIndex(
                name: "IX_SolicitacoesProgressao_AtribuidoPorUsuarioId",
                table: "SolicitacoesProgressao");

            migrationBuilder.DropIndex(
                name: "IX_SolicitacoesProgressao_RevisorUsuarioId",
                table: "SolicitacoesProgressao");

            migrationBuilder.DropColumn(
                name: "AtribuidoPorUsuarioId",
                table: "SolicitacoesProgressao");

            migrationBuilder.DropColumn(
                name: "DataAtribuicaoRevisor",
                table: "SolicitacoesProgressao");

            migrationBuilder.DropColumn(
                name: "DataParecerRevisor",
                table: "SolicitacoesProgressao");

            migrationBuilder.DropColumn(
                name: "Multiprogressao",
                table: "SolicitacoesProgressao");

            migrationBuilder.DropColumn(
                name: "ParecerRevisor",
                table: "SolicitacoesProgressao");

            migrationBuilder.DropColumn(
                name: "RevisorUsuarioId",
                table: "SolicitacoesProgressao");
        }
    }
}
