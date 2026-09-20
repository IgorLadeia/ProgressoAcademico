using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressoAcademico.Migrations
{
    /// <inheritdoc />
    public partial class PortalPublicoComunicadosCadastro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comunicados",
                columns: table => new
                {
                    ComunicadoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Titulo = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Resumo = table.Column<string>(type: "varchar(350)", maxLength: 350, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conteudo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Categoria = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataPublicacao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Publicado = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CriadoPorUsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comunicados", x => x.ComunicadoId);
                    table.ForeignKey(
                        name: "FK_Comunicados_Usuarios_CriadoPorUsuarioId",
                        column: x => x.CriadoPorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Comunicados_CriadoPorUsuarioId",
                table: "Comunicados",
                column: "CriadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Comunicados_Publicado_DataPublicacao",
                table: "Comunicados",
                columns: new[] { "Publicado", "DataPublicacao" });

            migrationBuilder.Sql(@"
                INSERT INTO Comunicados (Titulo, Resumo, Conteudo, Categoria, DataPublicacao, Publicado, DataCriacao, DataAtualizacao, CriadoPorUsuarioId)
                SELECT 'Acompanhamento de prazos de progressao',
                       'Centralizacao das datas de referencia para reduzir perda de prazos e retrabalho.',
                       'A area administrativa podera publicar nesta pagina os marcos do calendario academico, orientacoes sobre documentos e lembretes de envio.',
                       'Prazo',
                       UTC_TIMESTAMP(),
                       1,
                       UTC_TIMESTAMP(),
                       NULL,
                       u.UsuarioId
                FROM Usuarios u
                WHERE u.Email = 'admin@ufabc.edu.br'
                AND NOT EXISTS (SELECT 1 FROM Comunicados WHERE Titulo = 'Acompanhamento de prazos de progressao');

                INSERT INTO Comunicados (Titulo, Resumo, Conteudo, Categoria, DataPublicacao, Publicado, DataCriacao, DataAtualizacao, CriadoPorUsuarioId)
                SELECT 'Checklist documental do professor',
                       'Documentos e comprovantes devem ficar associados à solicitação ou à atividade correspondente.',
                       'O sistema organiza documentos gerais do processo e múltiplos comprovantes por atividade, permitindo consulta das evidências no mesmo fluxo.',
                       'Documento',
                       UTC_TIMESTAMP(),
                       1,
                       UTC_TIMESTAMP(),
                       NULL,
                       u.UsuarioId
                FROM Usuarios u
                WHERE u.Email = 'admin@ufabc.edu.br'
                AND NOT EXISTS (SELECT 1 FROM Comunicados WHERE Titulo = 'Checklist documental do professor');

                INSERT INTO Comunicados (Titulo, Resumo, Conteudo, Categoria, DataPublicacao, Publicado, DataCriacao, DataAtualizacao, CriadoPorUsuarioId)
                SELECT 'Portal administrativo em evolucao',
                       'Administradores podem publicar e excluir informativos diretamente pela tela especializada.',
                       'Esta funcionalidade inaugura a area de comunicacao administrativa do sistema e prepara a evolucao para metricas, indicadores e gestao completa.',
                       'Sistema',
                       UTC_TIMESTAMP(),
                       1,
                       UTC_TIMESTAMP(),
                       NULL,
                       u.UsuarioId
                FROM Usuarios u
                WHERE u.Email = 'admin@ufabc.edu.br'
                AND NOT EXISTS (SELECT 1 FROM Comunicados WHERE Titulo = 'Portal administrativo em evolucao');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comunicados");
        }
    }
}
