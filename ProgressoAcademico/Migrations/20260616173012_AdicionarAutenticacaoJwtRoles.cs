using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressoAcademico.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarAutenticacaoJwtRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PerfilAcesso",
                table: "Usuarios",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Professor")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.Sql(@"
                INSERT INTO Usuarios
                    (Nome, Email, SenhaHash, PerfilAcesso, Ativo, DataCriacao)
                SELECT
                    'Administrador do Sistema',
                    'admin@ufabc.edu.br',
                    '$2b$10$RWLDftcYJQGKrytjpt9OQOgcjkJj2HbaUVPuBCNuxbSdpDJgVZ1pW',
                    'Administrador',
                    1,
                    UTC_TIMESTAMP()
                WHERE NOT EXISTS (
                    SELECT 1 FROM Usuarios WHERE Email = 'admin@ufabc.edu.br'
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM Usuarios
                WHERE Email = 'admin@ufabc.edu.br'
                  AND PerfilAcesso = 'Administrador';
            ");

            migrationBuilder.DropColumn(
                name: "PerfilAcesso",
                table: "Usuarios");
        }
    }
}
