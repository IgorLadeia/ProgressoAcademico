using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressoAcademico.Migrations
{
    /// <inheritdoc />
    public partial class AtualizaComunicadosPublicosTcc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE Comunicados
                SET Resumo = 'Centralização de datas de referência para apoiar o acompanhamento do processo.',
                    Conteudo = 'A área administrativa pode publicar nesta página marcos do calendário acadêmico, orientações sobre documentos e lembretes relacionados à submissão das solicitações.',
                    Categoria = 'Prazo'
                WHERE Titulo = 'Acompanhamento de prazos de progressao';
                """);

            migrationBuilder.Sql("""
                UPDATE Comunicados
                SET Titulo = 'Organização documental da solicitação',
                    Resumo = 'Documentos e comprovantes devem ficar associados à solicitação ou à atividade correspondente.',
                    Conteudo = 'O sistema organiza documentos gerais do processo e múltiplos comprovantes por atividade, permitindo que professores, revisores e administradores consultem as evidências no mesmo fluxo.',
                    Categoria = 'Documento'
                WHERE Titulo = 'Checklist documental do professor';
                """);

            migrationBuilder.Sql("""
                UPDATE Comunicados
                SET Titulo = 'Portal administrativo e revisão técnica',
                    Resumo = 'Administradores podem acompanhar solicitações, atribuir revisores e registrar decisões.',
                    Conteudo = 'A área administrativa concentra indicadores, filtros, atribuição de múltiplos revisores, consulta de pareceres técnicos e registro da decisão final do processo.',
                    Categoria = 'Sistema'
                WHERE Titulo = 'Portal administrativo em evolucao';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE Comunicados
                SET Resumo = 'Centralizacao das datas de referencia para reduzir perda de prazos e retrabalho.',
                    Conteudo = 'A area administrativa podera publicar nesta pagina os marcos do calendario academico, orientacoes sobre documentos e lembretes de envio.',
                    Categoria = 'Prazo'
                WHERE Titulo = 'Acompanhamento de prazos de progressao';
                """);

            migrationBuilder.Sql("""
                UPDATE Comunicados
                SET Titulo = 'Checklist documental do professor',
                    Resumo = 'O portal de referencias reune arquivos auxiliares em PDF, TXT e XLSX para apoiar a validacao do sistema funcional.',
                    Conteudo = 'Os arquivos disponibilizados no portal servem como material de apoio para usuários e administradores durante o uso do sistema.',
                    Categoria = 'Documento'
                WHERE Titulo = 'Organização documental da solicitação';
                """);

            migrationBuilder.Sql("""
                UPDATE Comunicados
                SET Titulo = 'Portal administrativo em evolucao',
                    Resumo = 'Administradores podem publicar e excluir informativos diretamente pela tela especializada.',
                    Conteudo = 'Esta funcionalidade inaugura a area de comunicacao administrativa do sistema e prepara a evolucao para metricas, indicadores e gestao completa.',
                    Categoria = 'Sistema'
                WHERE Titulo = 'Portal administrativo e revisão técnica';
                """);
        }
    }
}
