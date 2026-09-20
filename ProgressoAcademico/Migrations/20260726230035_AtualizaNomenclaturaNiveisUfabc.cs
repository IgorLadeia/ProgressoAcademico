using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressoAcademico.Migrations
{
    /// <inheritdoc />
    public partial class AtualizaNomenclaturaNiveisUfabc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE Niveis
                SET Classe = 'Classe B - Professor adjunto',
                    Descricao = 'Classe B conforme nomenclatura publicada pelo CECS/UFABC para acesso a professor adjunto.'
                WHERE Codigo IN ('B1', 'B2');
                """);

            migrationBuilder.Sql("""
                UPDATE Niveis
                SET Classe = 'Classe C - Professor associado',
                    Descricao = CASE Codigo
                        WHEN 'C1' THEN 'Ingresso na Classe C conforme nomenclatura publicada pelo CECS/UFABC para professor associado.'
                        ELSE 'Progressao dentro da Classe C conforme nomenclatura publicada pelo CECS/UFABC.'
                    END
                WHERE Codigo IN ('C1', 'C2', 'C3', 'C4');
                """);

            migrationBuilder.Sql("""
                UPDATE Niveis
                SET Classe = 'Classe D - Professor titular',
                    Descricao = CASE Codigo
                        WHEN 'D1' THEN 'Ingresso na Classe D conforme nomenclatura publicada pelo CECS/UFABC para professor titular.'
                        ELSE 'Progressao dentro da Classe D conforme nomenclatura publicada pelo CECS/UFABC.'
                    END
                WHERE Codigo IN ('D1', 'D2', 'D3', 'D4');
                """);

            migrationBuilder.Sql("""
                UPDATE Niveis
                SET Ativo = 0,
                    Descricao = 'Registro historico mantido por compatibilidade com dados anteriores.'
                WHERE Codigo = 'E1';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE Niveis
                SET Classe = 'Assistente',
                    Descricao = CASE Codigo
                        WHEN 'B1' THEN 'Ingresso na Classe B - Assistente (Mestrado).'
                        ELSE 'Progressao dentro da Classe B.'
                    END
                WHERE Codigo IN ('B1', 'B2');
                """);

            migrationBuilder.Sql("""
                UPDATE Niveis
                SET Classe = 'Adjunto',
                    Descricao = CASE Codigo
                        WHEN 'C1' THEN 'Ingresso na Classe C - Adjunto (Doutorado).'
                        WHEN 'C4' THEN 'Ultimo nivel da Classe C.'
                        ELSE 'Progressao dentro da Classe C.'
                    END
                WHERE Codigo IN ('C1', 'C2', 'C3', 'C4');
                """);

            migrationBuilder.Sql("""
                UPDATE Niveis
                SET Classe = 'Associado',
                    Descricao = CASE Codigo
                        WHEN 'D1' THEN 'Promocao para Classe D - Associado.'
                        WHEN 'D4' THEN 'Ultimo nivel da Classe D.'
                        ELSE 'Progressao dentro da Classe D.'
                    END
                WHERE Codigo IN ('D1', 'D2', 'D3', 'D4');
                """);

            migrationBuilder.Sql("""
                UPDATE Niveis
                SET Classe = 'Titular',
                    Descricao = 'Classe final da carreira - Professor Titular.',
                    Ativo = 1
                WHERE Codigo = 'E1';
                """);
        }
    }
}
