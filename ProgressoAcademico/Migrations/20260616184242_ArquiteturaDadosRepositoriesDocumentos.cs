using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressoAcademico.Migrations
{
    /// <inheritdoc />
    public partial class ArquiteturaDadosRepositoriesDocumentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET @schemaName = DATABASE();

                SET @sql = (
                    SELECT IF(COUNT(*) = 0,
                        'ALTER TABLE `Documentos` ADD COLUMN `ContentType` varchar(100) CHARACTER SET utf8mb4 NOT NULL DEFAULT ''application/octet-stream''',
                        'SELECT 1')
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = @schemaName
                      AND TABLE_NAME = 'Documentos'
                      AND COLUMN_NAME = 'ContentType'
                );
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;

                SET @sql = (
                    SELECT IF(COUNT(*) = 0,
                        'ALTER TABLE `Documentos` ADD COLUMN `DataUpload` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6)',
                        'SELECT 1')
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = @schemaName
                      AND TABLE_NAME = 'Documentos'
                      AND COLUMN_NAME = 'DataUpload'
                );
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;

                SET @sql = (
                    SELECT IF(COUNT(*) = 0,
                        'ALTER TABLE `Documentos` ADD COLUMN `NomeArquivo` varchar(255) CHARACTER SET utf8mb4 NOT NULL DEFAULT ''''',
                        'SELECT 1')
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = @schemaName
                      AND TABLE_NAME = 'Documentos'
                      AND COLUMN_NAME = 'NomeArquivo'
                );
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;

                SET @sql = (
                    SELECT IF(COUNT(*) = 0,
                        'ALTER TABLE `Documentos` ADD COLUMN `TamanhoBytes` bigint NOT NULL DEFAULT 0',
                        'SELECT 1')
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = @schemaName
                      AND TABLE_NAME = 'Documentos'
                      AND COLUMN_NAME = 'TamanhoBytes'
                );
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;

                SET @sql = (
                    SELECT IF(COUNT(*) = 0,
                        'CREATE INDEX `IX_SolicitacoesProgressao_UsuarioId_StatusSolicitacaoId` ON `SolicitacoesProgressao` (`UsuarioId`, `StatusSolicitacaoId`)',
                        'SELECT 1')
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = @schemaName
                      AND TABLE_NAME = 'SolicitacoesProgressao'
                      AND INDEX_NAME = 'IX_SolicitacoesProgressao_UsuarioId_StatusSolicitacaoId'
                );
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET @schemaName = DATABASE();

                SET @sql = (
                    SELECT IF(COUNT(*) > 0,
                        'DROP INDEX `IX_SolicitacoesProgressao_UsuarioId_StatusSolicitacaoId` ON `SolicitacoesProgressao`',
                        'SELECT 1')
                    FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_SCHEMA = @schemaName
                      AND TABLE_NAME = 'SolicitacoesProgressao'
                      AND INDEX_NAME = 'IX_SolicitacoesProgressao_UsuarioId_StatusSolicitacaoId'
                );
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;

                SET @sql = (
                    SELECT IF(COUNT(*) > 0,
                        'ALTER TABLE `Documentos` DROP COLUMN `ContentType`',
                        'SELECT 1')
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = @schemaName
                      AND TABLE_NAME = 'Documentos'
                      AND COLUMN_NAME = 'ContentType'
                );
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;

                SET @sql = (
                    SELECT IF(COUNT(*) > 0,
                        'ALTER TABLE `Documentos` DROP COLUMN `DataUpload`',
                        'SELECT 1')
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = @schemaName
                      AND TABLE_NAME = 'Documentos'
                      AND COLUMN_NAME = 'DataUpload'
                );
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;

                SET @sql = (
                    SELECT IF(COUNT(*) > 0,
                        'ALTER TABLE `Documentos` DROP COLUMN `NomeArquivo`',
                        'SELECT 1')
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = @schemaName
                      AND TABLE_NAME = 'Documentos'
                      AND COLUMN_NAME = 'NomeArquivo'
                );
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;

                SET @sql = (
                    SELECT IF(COUNT(*) > 0,
                        'ALTER TABLE `Documentos` DROP COLUMN `TamanhoBytes`',
                        'SELECT 1')
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = @schemaName
                      AND TABLE_NAME = 'Documentos'
                      AND COLUMN_NAME = 'TamanhoBytes'
                );
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");
        }
    }
}
