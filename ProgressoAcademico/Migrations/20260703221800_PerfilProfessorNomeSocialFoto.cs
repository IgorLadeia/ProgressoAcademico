using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressoAcademico.Migrations
{
    /// <inheritdoc />
    public partial class PerfilProfessorNomeSocialFoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "FotoPerfilArquivo",
                table: "UsuariosPerfis",
                type: "longblob",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FotoPerfilAtualizadaEm",
                table: "UsuariosPerfis",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotoPerfilContentType",
                table: "UsuariosPerfis",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoPerfilArquivo",
                table: "UsuariosPerfis");

            migrationBuilder.DropColumn(
                name: "FotoPerfilAtualizadaEm",
                table: "UsuariosPerfis");

            migrationBuilder.DropColumn(
                name: "FotoPerfilContentType",
                table: "UsuariosPerfis");
        }
    }
}
