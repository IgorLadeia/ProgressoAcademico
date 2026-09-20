using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressoAcademico.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaUltimaMovimentacaoSolicitacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataUltimaMovimentacao",
                table: "SolicitacoesProgressao",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE SolicitacoesProgressao sp
                SET DataUltimaMovimentacao = (
                    SELECT MAX(data_movimento)
                    FROM (
                        SELECT sp.DataCriacao AS data_movimento
                        UNION ALL SELECT sp.DataFechamento
                        UNION ALL SELECT sp.DataAtribuicaoRevisor
                        UNION ALL SELECT sp.DataParecerRevisor
                        UNION ALL SELECT sp.DataRevisao
                        UNION ALL SELECT MAX(d.DataUpload)
                            FROM Documentos d
                            WHERE d.SolicitacaoProgressaoId = sp.SolicitacaoProgressaoId
                        UNION ALL SELECT MAX(a.DataAvaliacao)
                            FROM Atividades a
                            WHERE a.SolicitacaoProgressaoId = sp.SolicitacaoProgressaoId
                        UNION ALL SELECT MAX(sr.DataAtribuicao)
                            FROM SolicitacoesRevisores sr
                            WHERE sr.SolicitacaoProgressaoId = sp.SolicitacaoProgressaoId
                        UNION ALL SELECT MAX(sr.DataParecer)
                            FROM SolicitacoesRevisores sr
                            WHERE sr.SolicitacaoProgressaoId = sp.SolicitacaoProgressaoId
                        UNION ALL SELECT MAX(ar.DataAvaliacao)
                            FROM AtividadesAvaliacoesRevisores ar
                            INNER JOIN Atividades a ON a.AtividadeId = ar.AtividadeId
                            WHERE a.SolicitacaoProgressaoId = sp.SolicitacaoProgressaoId
                    ) movimentos
                    WHERE data_movimento IS NOT NULL
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataUltimaMovimentacao",
                table: "SolicitacoesProgressao");
        }
    }
}
