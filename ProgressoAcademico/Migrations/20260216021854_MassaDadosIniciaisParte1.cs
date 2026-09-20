using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressoAcademico.Migrations
{
    /// <inheritdoc />
    public partial class MassaDadosIniciaisParte1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {

            mb.Sql("INSERT INTO TiposVinculo (Nome) " +
                        "VALUES ('Professor Efetivo - 20h'), " +
                               "('Professor Efetivo - 40h'), " +
                               "('Professor Efetivo - Dedicação Exclusiva'), " +
                               "('Professor Substituto'), " +
                               "('Professor Visitante'), " +
                               "('Professor Voluntário'), " +
                               "('Professor Colaborador'), " +
                               "('Professor Aposentado com Vínculo Acadêmico'), " +
                               "('Professor Cedidos'), " +
                               "('Professor Redistribuído'), " +
                               "('Professor Temporário via Convênio'), " +
                               "('Professor Bolsista (CAPES/CNPq)');");

            mb.Sql("INSERT INTO TiposProgresso (Nome, Descricao) " +
                        "VALUES ( 'Progressão de Nível (Simples)', 'Progressão horizontal dentro da mesma classe, com avanço de apenas um nível.' )," +
                              " ( 'Progressão de Nível Múltipla', 'Progressão horizontal com avanço de dois ou mais níveis acumulados por atraso administrativo ou reconhecimento retroativo.' )," +
                              " ( 'Promoção de Classe', 'Mudança vertical de classe na carreira do magistério superior (ex: Assistente para Adjunto).' )," +
                              " ( 'Aceleração por Titulação', 'Promoção antecipada em razão da obtenção de nova titulação acadêmica (Mestrado ou Doutorado).' )," +
                              " ( 'Promoção à Classe de Titular', 'Promoção para a classe final da carreira, mediante requisitos específicos e avaliação por banca.' )," +
                              " ( 'Alteração de Regime de Trabalho', 'Mudança do regime de trabalho do docente (ex: 20h para 40h ou Dedicação Exclusiva).' ); ");

            mb.Sql("INSERT INTO TiposDocumento (Tipo) " +
                        "VALUES  " +
                              " ('DOC. PESSOAL: Documento de Identidade (RG)')," +
                              " ('DOC. PESSOAL: CPF')," +
                              " ('DOC. PESSOAL: Comprovante de Residência')," +
                              " ('DOC. PESSOAL: Certidão de Nascimento ou Casamento')," +
                              " ('DOC. PESSOAL: Currículo Lattes')," +
                              " ('DOC. PESSOAL: Diploma de Graduação')," +
                              " ('DOC. PESSOAL: Diploma de Mestrado')," +
                              " ('DOC. PESSOAL: Diploma de Doutorado')," +
                              " ('DOC. PESSOAL: Portaria de Nomeação')," +
                              " ('DOC. PESSOAL: Portaria de Posse')," +
                              " ('DOC. PESSOAL: Termo de Exercício')," +

                              " ('DOC. ENSINO: Plano de Ensino')," +
                              " ('DOC. ENSINO: Relatório de Atividades de Ensino')," +
                              " ('DOC. ENSINO: Declaração de Disciplinas Ministradas')," +
                              " ('DOC. ENSINO: Avaliação Discente')," +
                              " ('DOC. ENSINO: Comprovação de Orientação de PGC')," +
                              " ('DOC. ENSINO: Comprovação de Orientação de Mestrado')," +
                              " ('DOC. ENSINO: Comprovação de Orientação de Doutorado')," +
                              " ('DOC. ENSINO: Declaração de Participação em Banca')," +
                              " ('DOC. ENSINO: Coordenação de Curso')," +
                              " ('DOC. ENSINO: Supervisão de Estágio')," +

                              " ('DOC. PESQUISA: Artigo Científico Publicado')," +
                              " ('DOC. PESQUISA: Artigo Aceito para Publicação')," +
                              " ('DOC. PESQUISA: Livro Publicado')," +
                              " ('DOC. PESQUISA: Capítulo de Livro')," +
                              " ('DOC. PESQUISA: Trabalho em Anais de Evento')," +
                              " ('DOC. PESQUISA: Projeto de Pesquisa Aprovado')," +
                              " ('DOC. PESQUISA: Relatório Final de Pesquisa')," +
                              " ('DOC. PESQUISA: Comprovação de Bolsa de Pesquisa')," +
                              " ('DOC. PESQUISA: Declaração de Participação em Grupo de Pesquisa')," +
                              " ('DOC. PESQUISA: Registro de Patente')," +
                              " ('DOC. PESQUISA: Produção Técnica')," +

                              " ('DOC. EXTENSÃO: Projeto de Extensão')," +
                              " ('DOC. EXTENSÃO: Relatório de Projeto de Extensão')," +
                              " ('DOC. EXTENSÃO: Coordenação de Ação Extensionista')," +
                              " ('DOC. EXTENSÃO: Declaração de Participação em Evento de Extensão')," +
                              " ('DOC. EXTENSÃO: Certificado de Curso Ministrado à Comunidade')," +
                              " ('DOC. EXTENSÃO: Atividade Cultural ou Social')," +

                              " ('DOC. GESTÃO: Portaria de Designação para Cargo Administrativo')," +
                              " ('DOC. GESTÃO: Comprovação de Chefia de Departamento')," +
                              " ('DOC. GESTÃO: Comprovação de Direção de Unidade')," +
                              " ('DOC. GESTÃO: Participação em Comissão')," +
                              " ('DOC. GESTÃO: Participação em Conselho Universitário')," +
                              " ('DOC. GESTÃO: Participação em Núcleo Docente Estruturante (NDE)')," +
                              " ('DOC. PROGRESSÃO: Requerimento de Progressão')," +
                              " ('DOC. PROGRESSÃO: Relatório de Atividades Docentes (RAD)')," +
                              " ('DOC. PROGRESSÃO: Memorial Descritivo')," +
                              " ('DOC. PROGRESSÃO: Plano de Trabalho')," +
                              " ('DOC. PROGRESSÃO: Ata de Avaliação')," +
                              " ('DOC. PROGRESSÃO: Parecer da Comissão Avaliadora')," +
                              " ('DOC. PROGRESSÃO: Resultado Final da Avaliação')," +

                              "('DOC. REGIME: Solicitação de Alteração de Regime')," +
                              " ('DOC. REGIME: Justificativa para Mudança de Regime')," +
                              " ('DOC. REGIME: Plano de Trabalho para Dedicação Exclusiva')," +
                              " ('DOC. REGIME: Declaração de Não Acúmulo de Cargo')," +

                              " ('DOC. GERAL: Certificado de Curso')," +
                              " ('DOC. GERAL: Certificado de Evento')," +
                              " ('DOC. GERAL: Certificado de Premiação')," +
                              " ('DOC. GERAL: Declaração de Participação em Congresso')," +
                              " ('DOC. GERAL: Comprovante de Publicação')," +
                              " ('DOC. GERAL: Declaração Institucional'); ");

            mb.Sql("INSERT INTO TiposAtividade (Nome, Descricao) " +
                    "VALUES ('Ensino', 'Atividades relacionadas ao ensino de graduação e pós-graduação.')," +
                          " ('Pesquisa', 'Atividades relacionadas à produção científica e tecnológica.')," +
                          " ('Extensão', 'Atividades voltadas à interação com a sociedade.')," +
                          " ('Gestão', 'Atividades administrativas e de gestão acadêmica.'); ");

            mb.Sql(@"

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Aula em Graduação',
                    'Ministração de disciplina em curso de graduação.', 10
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Ensino'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Aula em Graduação');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Aula em Pós-Graduação',
                    'Ministração de disciplina em mestrado ou doutorado.', 15
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Ensino'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Aula em Pós-Graduação');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Orientação de PGC',
                    'Orientação de Projeto de Graduação em Computação.', 8
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Ensino'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Orientação de PGC');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Orientação de Mestrado',
                    'Orientação de dissertação de mestrado.', 20
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Ensino'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Orientação de Mestrado');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Orientação de Doutorado',
                    'Orientação de tese de doutorado.', 30
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Ensino'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Orientação de Doutorado');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Participação em Banca',
                    'Participação em banca examinadora.', 5
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Ensino'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Participação em Banca');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Supervisão de Estágio',
                    'Supervisão de estágio curricular.', 6
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Ensino'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Supervisão de Estágio');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Coordenação de Disciplina',
                    'Responsável pela organização da disciplina.', 12
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Ensino'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Coordenação de Disciplina');

                    ");




            mb.Sql(@"

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Artigo Científico Publicado',
                    'Publicação de artigo em periódico científico.', 25
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Pesquisa'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Artigo Científico Publicado');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Livro Publicado',
                    'Publicação de livro acadêmico.', 40
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Pesquisa'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Livro Publicado');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Capítulo de Livro',
                    'Publicação de capítulo em livro.', 18
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Pesquisa'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Capítulo de Livro');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Trabalho em Congresso',
                    'Publicação em anais de evento científico.', 12
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Pesquisa'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Trabalho em Congresso');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Coordenação de Projeto de Pesquisa',
                    'Coordenação de projeto aprovado.', 35
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Pesquisa'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Coordenação de Projeto de Pesquisa');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Participação em Projeto de Pesquisa',
                    'Participação em projeto de pesquisa.', 15
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Pesquisa'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Participação em Projeto de Pesquisa');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Registro de Patente',
                    'Depósito ou concessão de patente.', 50
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Pesquisa'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Registro de Patente');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Liderança de Grupo de Pesquisa',
                    'Atuação como líder de grupo certificado.', 28
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Pesquisa'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Liderança de Grupo de Pesquisa');

                    ");


            mb.Sql(@"

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Coordenação de Projeto de Extensão',
                    'Coordenação de ação extensionista.', 25
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Extensão'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Coordenação de Projeto de Extensão');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Participação em Projeto de Extensão',
                    'Participação em projeto de extensão.', 12
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Extensão'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Participação em Projeto de Extensão');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Curso de Extensão',
                    'Ministração de curso para comunidade.', 15
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Extensão'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Curso de Extensão');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Palestra ou Workshop',
                    'Realização de palestra ou oficina.', 10
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Extensão'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Palestra ou Workshop');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Organização de Evento',
                    'Organização de evento extensionista.', 18
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Extensão'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Organização de Evento');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Programa Institucional de Extensão',
                    'Atuação em programa permanente.', 22
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Extensão'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Programa Institucional de Extensão');

                    ");


            mb.Sql(@"

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Reitor',
                    'Cargo máximo da instituição.', 100
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Gestão'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Reitor');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Vice-Reitor',
                    'Substituto imediato do reitor.', 80
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Gestão'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Vice-Reitor');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Pró-Reitor',
                    'Gestor de área estratégica.', 70
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Gestão'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Pró-Reitor');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Diretor de Unidade',
                    'Direção de centro ou faculdade.', 60
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Gestão'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Diretor de Unidade');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Chefe de Departamento',
                    'Responsável por departamento acadêmico.', 50
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Gestão'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Chefe de Departamento');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Coordenador de Curso',
                    'Coordenação de curso de graduação ou pós.', 40
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Gestão'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Coordenador de Curso');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Participação em Conselho',
                    'Membro de conselho universitário.', 15
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Gestão'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Participação em Conselho');

                    INSERT INTO SubTiposAtividade (TipoAtividadeId, Nome, Descricao, Pontos)
                    SELECT ta.TipoAtividadeId, 'Participação em Comissão',
                    'Atuação em comissão institucional.', 12
                    FROM TiposAtividade ta
                    WHERE ta.Nome = 'Gestão'
                    AND NOT EXISTS (SELECT 1 FROM SubTiposAtividade s WHERE s.Nome = 'Participação em Comissão');

                    ");


            mb.Sql("INSERT INTO TiposVinculo (Nome) " +
                        "VALUES ('Professor Efetivo - 20h'), " +
                               "('Professor Efetivo - 40h'), " +
                               "('Professor Efetivo - Dedicação Exclusiva'), " +
                               "('Professor Substituto'), " +
                               "('Professor Visitante'), " +
                               "('Professor Voluntário'), " +
                               "('Professor Colaborador'), " +
                               "('Professor Aposentado com Vínculo Acadêmico'), " +
                               "('Professor Cedidos'), " +
                               "('Professor Redistribuído'), " +
                               "('Professor Temporário via Convênio'), " +
                               "('Professor Bolsista (CAPES/CNPq)');");


            mb.Sql(@"

                    INSERT INTO Niveis (Classe, Codigo, Ordem, Descricao, Ativo)
                    VALUES
                    ('Classe A', 'A1', 1,  'Classe inicial da carreira do Magistério Superior.', 1),
                    ('Classe A', 'A2', 2,  'Progressão dentro da Classe A.', 1),

                    ('Assistente', 'B1', 3, 'Ingresso na Classe B - Assistente (Mestrado).', 1),
                    ('Assistente', 'B2', 4, 'Progressão dentro da Classe B.', 1),

                    ('Adjunto', 'C1', 5, 'Ingresso na Classe C - Adjunto (Doutorado).', 1),
                    ('Adjunto', 'C2', 6, 'Progressão dentro da Classe C.', 1),
                    ('Adjunto', 'C3', 7, 'Progressão dentro da Classe C.', 1),
                    ('Adjunto', 'C4', 8, 'Último nível da Classe C.', 1),

                    ('Associado', 'D1', 9,  'Promoção para Classe D - Associado.', 1),
                    ('Associado', 'D2', 10, 'Progressão dentro da Classe D.', 1),
                    ('Associado', 'D3', 11, 'Progressão dentro da Classe D.', 1),
                    ('Associado', 'D4', 12, 'Último nível da Classe D.', 1),

                    ('Titular', 'E1', 13, 'Classe final da carreira - Professor Titular.', 1);

                    ");

            mb.Sql(@"
                    INSERT INTO Instituicoes (Nome, Sigla)
                    VALUES ('Universidade Federal do ABC', 'UFABC');
                    ");

            mb.Sql(@"
                INSERT INTO StatusSolicitacoes (Nome) VALUES
                ('Em Processamento'),
                ('Aprovada'),
                ('Negada'),
                ('Encerrada');
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            // 1️⃣ SubTiposAtividade (filha)
            mb.Sql(@"
                    DELETE FROM SubTiposAtividade 
                    WHERE Nome IN (
                        'Aula em Graduação',
                        'Aula em Pós-Graduação',
                        'Orientação de PGC',
                        'Orientação de Mestrado',
                        'Orientação de Doutorado',
                        'Participação em Banca',
                        'Supervisão de Estágio',
                        'Coordenação de Disciplina',
                        'Artigo Científico Publicado',
                        'Livro Publicado',
                        'Capítulo de Livro',
                        'Trabalho em Congresso',
                        'Coordenação de Projeto de Pesquisa',
                        'Participação em Projeto de Pesquisa',
                        'Registro de Patente',
                        'Liderança de Grupo de Pesquisa',
                        'Coordenação de Projeto de Extensão',
                        'Participação em Projeto de Extensão',
                        'Curso de Extensão',
                        'Palestra ou Workshop',
                        'Organização de Evento',
                        'Programa Institucional de Extensão',
                        'Reitor',
                        'Vice-Reitor',
                        'Pró-Reitor',
                        'Diretor de Unidade',
                        'Chefe de Departamento',
                        'Coordenador de Curso',
                        'Participação em Conselho',
                        'Participação em Comissão'
                    );
                ");

            // 2️⃣ TiposAtividade (pai)
            mb.Sql(@"
                    DELETE FROM TiposAtividade
                    WHERE Nome IN ('Ensino', 'Pesquisa', 'Extensão', 'Gestão');
                ");

            // 3️⃣ TiposDocumento
            mb.Sql(@"
                    DELETE FROM TiposDocumento
                    WHERE Tipo LIKE 'DOC.%';
                ");

            // 4️⃣ TiposProgresso
            mb.Sql(@"
                    DELETE FROM TiposProgresso
                    WHERE Nome IN (
                        'Progressão de Nível (Simples)',
                        'Progressão de Nível Múltipla',
                        'Promoção de Classe',
                        'Aceleração por Titulação',
                        'Promoção à Classe de Titular',
                        'Alteração de Regime de Trabalho'
                    );
                ");

            // 5️⃣ TiposVinculo
            mb.Sql(@"
                    DELETE FROM TiposVinculo
                    WHERE Nome LIKE 'Professor%';
                ");

            mb.Sql("DELETE FROM Niveis");

            mb.Sql("DELETE FROM Instituicoes");

            mb.Sql(@"
                DELETE FROM StatusSolicitacoes
                WHERE Nome IN (
                    'Em Processamento',
                    'Aprovada',
                    'Negada',
                    'Encerrada'
                );
            ");


        }
    }
}
