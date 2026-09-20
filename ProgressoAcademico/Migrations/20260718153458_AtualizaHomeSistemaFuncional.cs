using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressoAcademico.Migrations
{
    /// <inheritdoc />
    public partial class AtualizaHomeSistemaFuncional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ConteudosHome",
                keyColumn: "ConteudoHomeId",
                keyValue: 1,
                column: "ResumoProjeto",
                value: "O Progresso Academico UFABC e um sistema web funcional que apoia professores na organizacao de solicitacoes de progressao e oferece a administracao um fluxo estruturado de analise. A solucao reduz informacoes dispersas, retrabalho e incerteza sobre cada etapa.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ConteudosHome",
                keyColumn: "ConteudoHomeId",
                keyValue: 1,
                column: "ResumoProjeto",
                value: "O Progresso Acadêmico UFABC é um sistema funcional web que apoia professores na organização de solicitações de progressão e oferece à administração um fluxo estruturado de análise. A solução reduz informações dispersas, retrabalho e incerteza sobre cada etapa.");
        }
    }
}
