using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressoAcademico.Migrations
{
    /// <inheritdoc />
    public partial class AvaliacaoIndividualOrigemAtividades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataAvaliacao",
                table: "Atividades",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrigemCadastro",
                table: "Atividades",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "CadastroManual")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ParecerAvaliacao",
                table: "Atividades",
                type: "varchar(1500)",
                maxLength: 1500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataAvaliacao",
                table: "Atividades");

            migrationBuilder.DropColumn(
                name: "OrigemCadastro",
                table: "Atividades");

            migrationBuilder.DropColumn(
                name: "ParecerAvaliacao",
                table: "Atividades");
        }
    }
}
