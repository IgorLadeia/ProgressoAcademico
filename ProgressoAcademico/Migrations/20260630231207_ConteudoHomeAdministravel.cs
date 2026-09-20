using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressoAcademico.Migrations
{
    /// <inheritdoc />
    public partial class ConteudoHomeAdministravel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConteudosHome",
                columns: table => new
                {
                    ConteudoHomeId = table.Column<int>(type: "int", nullable: false),
                    TituloPrincipal = table.Column<string>(type: "varchar(220)", maxLength: 220, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResumoProjeto = table.Column<string>(type: "varchar(1200)", maxLength: 1200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TituloDestaque = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ItensDestaque = table.Column<string>(type: "varchar(1600)", maxLength: 1600, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AvisoEscopo = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DescricaoProfessor = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RecursosProfessor = table.Column<string>(type: "varchar(1600)", maxLength: 1600, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DescricaoAdministrador = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RecursosAdministrador = table.Column<string>(type: "varchar(1600)", maxLength: 1600, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AtualizadoPorUsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConteudosHome", x => x.ConteudoHomeId);
                    table.ForeignKey(
                        name: "FK_ConteudosHome_Usuarios_AtualizadoPorUsuarioId",
                        column: x => x.AtualizadoPorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "ConteudosHome",
                columns: new[] { "ConteudoHomeId", "AtualizadoPorUsuarioId", "AvisoEscopo", "DataAtualizacao", "DescricaoAdministrador", "DescricaoProfessor", "ItensDestaque", "RecursosAdministrador", "RecursosProfessor", "ResumoProjeto", "TituloDestaque", "TituloPrincipal" },
                values: new object[] { 1, null, "Ferramenta acadêmica de apoio. Não substitui os sistemas e atos oficiais da UFABC.", new DateTime(2026, 6, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Conduz a análise institucional e acompanha indicadores consolidados.", "Constrói e acompanha a própria solicitação com autonomia e rastreabilidade.", "Solicitações e atividades organizadas\nDocumentos comprobatórios centralizados\nElegibilidade básica e percentual de preenchimento\nRevisão administrativa com decisão registrada", "Revisa, corrige, aprova ou reprova solicitações\nConsulta métricas operacionais\nAcessa processos finalizados com privilégio administrativo\nPublica e edita os comunicados desta Home", "Cria uma solicitação aberta por vez\nCadastra atividades e anexa comprovantes\nConsulta elegibilidade, histórico e status\nVisualiza processos encerrados sem alterá-los", "O Progresso Acadêmico UFABC é um sistema funcional web que apoia professores na organização de solicitações de progressão e oferece à administração um fluxo estruturado de análise. A solução reduz informações dispersas, retrabalho e incerteza sobre cada etapa.", "Uma jornada digital, do cadastro ao parecer.", "Progressão acadêmica com informação, evidências e acompanhamento em um só lugar." });

            migrationBuilder.CreateIndex(
                name: "IX_ConteudosHome_AtualizadoPorUsuarioId",
                table: "ConteudosHome",
                column: "AtualizadoPorUsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConteudosHome");
        }
    }
}
