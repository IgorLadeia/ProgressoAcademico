using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressoAcademico.Migrations
{
    /// <inheritdoc />
    public partial class DocumentosElegibilidadePortalProfessor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HashSha256",
                table: "Documentos",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "Documentos",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "OrigemDocumento",
                table: "Documentos",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "ComprovatorioProfessor")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_HashSha256",
                table: "Documentos",
                column: "HashSha256");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Documentos_HashSha256",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "HashSha256",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "OrigemDocumento",
                table: "Documentos");
        }
    }
}
