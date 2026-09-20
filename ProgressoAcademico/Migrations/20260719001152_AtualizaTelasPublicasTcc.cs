using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressoAcademico.Migrations
{
    /// <inheritdoc />
    public partial class AtualizaTelasPublicasTcc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ConteudosHome",
                keyColumn: "ConteudoHomeId",
                keyValue: 1,
                columns: new[] { "AvisoEscopo", "DescricaoAdministrador", "DescricaoProfessor", "ItensDestaque", "RecursosAdministrador", "RecursosProfessor", "ResumoProjeto", "TituloDestaque", "TituloPrincipal" },
                values: new object[] { "Sistema acadêmico funcional de apoio. Não substitui normas, sistemas oficiais ou atos administrativos da UFABC.", "Coordena a análise das solicitações, atribui revisores, consulta pareceres e registra a decisão final.", "Prepara e acompanha a própria solicitação, registra atividades e vincula documentos comprobatórios.", "Solicitações e atividades centralizadas\nMúltiplos documentos por atividade\nRevisão técnica por professores avaliadores\nDecisão administrativa com parecer registrado", "Atribui um ou mais revisores por solicitação\nConsulta pareceres técnicos e documentos\nSolicita ajustes quando necessário\nRegistra aprovação, rejeição ou encerramento", "Cria e acompanha solicitações\nCadastra atividades de ensino, pesquisa, extensão e gestão\nAnexa múltiplos comprovantes por atividade\nConsulta histórico, status e pendências", "Aplicação web funcional desenvolvida para organizar solicitações, atividades, documentos, revisões técnicas e decisão administrativa no processo de progressão acadêmica dos professores da Universidade Federal do ABC.", "Fluxo estruturado para professor, revisor e administrador.", "Sistema integrado para apoio à progressão acadêmica docente." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ConteudosHome",
                keyColumn: "ConteudoHomeId",
                keyValue: 1,
                columns: new[] { "AvisoEscopo", "DescricaoAdministrador", "DescricaoProfessor", "ItensDestaque", "RecursosAdministrador", "RecursosProfessor", "ResumoProjeto", "TituloDestaque", "TituloPrincipal" },
                values: new object[] { "Ferramenta acadêmica de apoio. Não substitui os sistemas e atos oficiais da UFABC.", "Conduz a análise institucional e acompanha indicadores consolidados.", "Constrói e acompanha a própria solicitação com autonomia e rastreabilidade.", "Solicitações e atividades organizadas\nDocumentos comprobatórios centralizados\nElegibilidade básica e percentual de preenchimento\nRevisão administrativa com decisão registrada", "Revisa, corrige, aprova ou reprova solicitações\nConsulta métricas operacionais\nAcessa processos finalizados com privilégio administrativo\nPublica e edita os comunicados desta Home", "Cria uma solicitação aberta por vez\nCadastra atividades e anexa comprovantes\nConsulta elegibilidade, histórico e status\nVisualiza processos encerrados sem alterá-los", "O Progresso Academico UFABC e um sistema web funcional que apoia professores na organizacao de solicitacoes de progressao e oferece a administracao um fluxo estruturado de analise. A solucao reduz informacoes dispersas, retrabalho e incerteza sobre cada etapa.", "Uma jornada digital, do cadastro ao parecer.", "Progressão acadêmica com informação, evidências e acompanhamento em um só lugar." });
        }
    }
}
