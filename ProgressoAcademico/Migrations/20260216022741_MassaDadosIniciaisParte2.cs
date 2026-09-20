using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressoAcademico.Migrations
{
    /// <inheritdoc />
    public partial class MassaDadosIniciaisParte2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {
            // Usuário + Perfil + Vínculo de forma dinâmica
            mb.Sql(@"

                -- =========================================
                -- 1️ Garantir Usuario
                -- =========================================

                -- Criar usuário se não existir
                INSERT INTO Usuarios
                (Nome, Email, SenhaHash, Ativo, DataCriacao)
                SELECT
                    'Professor Padrao',
                    'professor@ufabc.edu.br',
                    '$2b$10$XEIJUH/Emiu/WK2OFs2YmeKWOHfjkb5ebJ90piyrvgRRIpbjbYmua',
                    1,
                    NOW()
                WHERE NOT EXISTS (
                    SELECT 1 FROM Usuarios
                    WHERE Email = 'professor@ufabc.edu.br'
                );

                -- Capturar ID (novo ou existente)
                SET @UsuarioId = (
                    SELECT UsuarioId
                    FROM Usuarios
                    WHERE Email = 'professor@ufabc.edu.br'
                    LIMIT 1
                );

                -- =========================================
                -- 2️ Garantir Perfil (1:1)
                -- =========================================

                INSERT INTO UsuariosPerfis (UsuarioId, Apelido, FotoPerfil)
                SELECT
                    @UsuarioId,
                    'Professor',
                    'UFABC/padrao'
                WHERE NOT EXISTS (
                    SELECT 1 FROM UsuariosPerfis
                    WHERE UsuarioId = @UsuarioId
                );

                -- =========================================
                -- 3️ Garantir Vinculo Institucional
                -- =========================================

                INSERT INTO VinculosInstitucionais
                (UsuarioId,
                 InstituicaoId,
                 TipoVinculoId,
                 NivelId,
                 DataIngressoInstituicao,
                 Ativo)
                SELECT
                    @UsuarioId,
                    (SELECT InstituicaoId FROM Instituicoes WHERE Sigla = 'UFABC' LIMIT 1),
                    (SELECT TipoVinculoId FROM TiposVinculo WHERE Nome = 'Professor Efetivo - Dedicação Exclusiva' LIMIT 1),
                    (SELECT NivelId FROM Niveis WHERE Codigo = 'A1' LIMIT 1),
                    DATE_SUB(NOW(), INTERVAL 2 YEAR),
                    1
                WHERE NOT EXISTS (
                    SELECT 1 FROM VinculosInstitucionais
                    WHERE UsuarioId = @UsuarioId
                );

                -- ===============================
                -- 1️ Criar Solicitação de Progressão
                -- ===============================

                INSERT INTO SolicitacoesProgressao
                (UsuarioId,
                 StatusSolicitacaoId,
                 TipoProgressoId,
                 NivelOrigemId,
                 NivelDestinoId,
                 DataCriacao,
                 Observacao, 
                 ParecerFinal,
                 Apelido)
                VALUES
                (
                    @UsuarioId,
                    (SELECT StatusSolicitacaoId FROM StatusSolicitacoes WHERE Nome = 'Em Processamento' LIMIT 1),
                    (SELECT TipoProgressoId FROM TiposProgresso WHERE Nome = 'Progressão de Nível (Simples)' LIMIT 1),
                    (SELECT NivelId FROM Niveis WHERE Codigo = 'A1' LIMIT 1),
                    (SELECT NivelId FROM Niveis WHERE Codigo = 'A2' LIMIT 1),
                    NOW(),
                    'Solicitação automática de teste com pontuação suficiente.',
                    'Em analise.',
                    'Teste de Progressão A1 para A2'
                );

                SET @SolicitacaoId = LAST_INSERT_ID();


                -- ===============================
                -- 2️ ATIVIDADE ENSINO - Aula Graduação (2x)
                -- ===============================

                INSERT INTO Atividades
                (SolicitacaoProgressaoId, TipoAtividadeId, SubTipoAtividadeId,
                 DataInicio, DataTermino, Titulo, Quantidade, PontuacaoCalculada, Status)
                VALUES
                (
                 @SolicitacaoId,
                 (SELECT TipoAtividadeId FROM TiposAtividade WHERE Nome = 'Ensino' LIMIT 1),
                 (SELECT SubTipoAtividadeId FROM SubTiposAtividade WHERE Nome = 'Aula em Graduação' LIMIT 1),
                 DATE_SUB(NOW(), INTERVAL 1 YEAR),
                 DATE_SUB(NOW(), INTERVAL 6 MONTH),
                 'Disciplina Estrutura de Dados',
                 2,
                 20,
                 'Aprovado'
                );

                SET @AtividadeId = LAST_INSERT_ID();

                INSERT INTO AtividadesEnsino
                (AtividadeId, CodigoDisciplina, NomeDisciplina, NumeroAlunos, Creditos, Turno, CargaHoraria)
                VALUES
                (@AtividadeId, 'BCC101', 'Estrutura de Dados', 40, 4, 'Noturno', 60);


                -- ===============================
                -- 3️ ATIVIDADE PESQUISA - Artigo
                -- ===============================

                INSERT INTO Atividades
                (SolicitacaoProgressaoId, TipoAtividadeId, SubTipoAtividadeId,
                 DataInicio, DataTermino, Titulo, Quantidade, PontuacaoCalculada, Status)
                VALUES
                (
                 @SolicitacaoId,
                 (SELECT TipoAtividadeId FROM TiposAtividade WHERE Nome = 'Pesquisa' LIMIT 1),
                 (SELECT SubTipoAtividadeId FROM SubTiposAtividade WHERE Nome = 'Artigo Científico Publicado' LIMIT 1),
                 DATE_SUB(NOW(), INTERVAL 1 YEAR),
                 DATE_SUB(NOW(), INTERVAL 10 MONTH),
                 'Artigo sobre Inteligência Artificial',
                 1,
                 25,
                 'Aprovado'
                );

                SET @AtividadeId = LAST_INSERT_ID();

                INSERT INTO AtividadesPesquisa
                (AtividadeId, TipoProducao, VeiculoPublicacao, ISSN_ISBN, NumeroAutores)
                VALUES
                (@AtividadeId, 'Artigo Qualis A2', 'Revista Brasileira de Computação', '1234-5678', 3);


                -- ===============================
                -- 4️ ATIVIDADE EXTENSÃO
                -- ===============================

                INSERT INTO Atividades
                (SolicitacaoProgressaoId, TipoAtividadeId, SubTipoAtividadeId,
                 DataInicio, DataTermino, Titulo, Quantidade, PontuacaoCalculada, Status)
                VALUES
                (
                 @SolicitacaoId,
                 (SELECT TipoAtividadeId FROM TiposAtividade WHERE Nome = 'Extensão' LIMIT 1),
                 (SELECT SubTipoAtividadeId FROM SubTiposAtividade WHERE Nome = 'Coordenação de Projeto de Extensão' LIMIT 1),
                 DATE_SUB(NOW(), INTERVAL 1 YEAR),
                 DATE_SUB(NOW(), INTERVAL 3 MONTH),
                 'Projeto Comunidade Digital',
                 1,
                 25,
                 'Aprovado'
                );

                SET @AtividadeId = LAST_INSERT_ID();

                INSERT INTO AtividadesExtensao
                (AtividadeId, Projeto, CargaHoraria, PublicoAlvo)
                VALUES
                (@AtividadeId, 'Comunidade Digital', '120h', 'Comunidade local');


                -- ===============================
                -- 5️ ATIVIDADE GESTÃO
                -- ===============================

                INSERT INTO Atividades
                (SolicitacaoProgressaoId, TipoAtividadeId, SubTipoAtividadeId,
                 DataInicio, DataTermino, Titulo, Quantidade, PontuacaoCalculada, Status)
                VALUES
                (
                 @SolicitacaoId,
                 (SELECT TipoAtividadeId FROM TiposAtividade WHERE Nome = 'Gestão' LIMIT 1),
                 (SELECT SubTipoAtividadeId FROM SubTiposAtividade WHERE Nome = 'Participação em Comissão' LIMIT 1),
                 DATE_SUB(NOW(), INTERVAL 1 YEAR),
                 DATE_SUB(NOW(), INTERVAL 2 MONTH),
                 'Comissão de Avaliação Institucional',
                 1,
                 12,
                 'Aprovado'
                );

                SET @AtividadeId = LAST_INSERT_ID();

                INSERT INTO AtividadesAdministrativas
                (AtividadeId, Cargo, Unidade)
                VALUES
                (@AtividadeId, 'Membro', 'Departamento de Computação');
            ");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql(@"

                SET @UsuarioId = (
                    SELECT UsuarioId 
                    FROM Usuarios 
                    WHERE Email = 'professor@ufabc.edu.br'
                    LIMIT 1
                );

                -- Capturar Solicitação
                SET @SolicitacaoId = (
                    SELECT SolicitacaoProgressaoId
                    FROM SolicitacoesProgressao
                    WHERE UsuarioId = @UsuarioId
                    LIMIT 1
                );
                
                 DELETE FROM AtividadesEnsino
                WHERE AtividadeId IN (
                    SELECT AtividadeId FROM Atividades
                    WHERE SolicitacaoProgressaoId = @SolicitacaoId
                );

                DELETE FROM AtividadesPesquisa
                WHERE AtividadeId IN (
                    SELECT AtividadeId FROM Atividades
                    WHERE SolicitacaoProgressaoId = @SolicitacaoId
                );

                DELETE FROM AtividadesExtensao
                WHERE AtividadeId IN (
                    SELECT AtividadeId FROM Atividades
                    WHERE SolicitacaoProgressaoId = @SolicitacaoId
                );

                DELETE FROM AtividadesAdministrativas
                WHERE AtividadeId IN (
                    SELECT AtividadeId FROM Atividades
                    WHERE SolicitacaoProgressaoId = @SolicitacaoId
                );

                -- ===============================
                -- 2️⃣ Remover Atividades
                -- ===============================

                DELETE FROM Atividades
                WHERE SolicitacaoProgressaoId = @SolicitacaoId;

                -- ===============================
                -- 3️⃣ Remover Solicitação
                -- ===============================

                DELETE FROM SolicitacoesProgressao
                WHERE SolicitacaoProgressaoId = @SolicitacaoId;

                DELETE FROM VinculosInstitucionais
                WHERE UsuarioId IN (
                    SELECT UsuarioId FROM Usuarios WHERE Email = 'professor@ufabc.edu.br'
                );

                DELETE FROM UsuariosPerfis
                WHERE UsuarioId IN (
                    SELECT UsuarioId FROM Usuarios WHERE Email = 'professor@ufabc.edu.br'
                );

                DELETE FROM Usuarios
                WHERE Email = 'professor@ufabc.edu.br';

            ");
        }
    }
}
