# Progresso Acadêmico

Sistema web para apoio ao cadastro, acompanhamento e revisão de solicitações de progressão acadêmica de docentes da Universidade Federal do ABC.

> Esta aplicação é um protótipo acadêmico de apoio. Ela não substitui sistemas oficiais, atos administrativos, normativos institucionais ou a tramitação formal da UFABC.

## Objetivo

Centralizar informações que normalmente ficam dispersas durante a preparação de uma solicitação de progressão acadêmica:

- cadastro do professor;
- criação de solicitação de progressão;
- registro de atividades de ensino, pesquisa, extensão e gestão;
- associação de documentos comprobatórios às atividades;
- acompanhamento de status, pendências e histórico;
- revisão técnica por avaliador;
- decisão administrativa;
- gestão de comunicados públicos.

## Stack

- .NET 8
- ASP.NET Core MVC
- Razor Views
- Entity Framework Core
- MySQL
- JWT Authentication
- BCrypt para hash de senha
- Swagger
- xUnit

## Estrutura do Projeto

```text
ProgressoAcademico/
  Application/
    DTOs/
    Interfaces/
  Context/
  Controllers/
  Infrastructure/
    Repositories/
  Migrations/
  Models/
    ViewModels/
  Services/
  Views/
  wwwroot/

ProgressoAcademico.Tests/
```

## Perfis

O sistema trabalha com três responsabilidades principais:

- **Professor**: cria e acompanha solicitações, cadastra atividades e anexa documentos.
- **Revisor**: analisa atividades atribuídas, registra pareceres e pode solicitar ajustes.
- **Administrador**: acompanha indicadores, atribui revisores, revisa solicitações e registra decisão final.

## Usuários de Desenvolvimento

As migrations criam apenas contas genéricas para uso local:

| Perfil | E-mail | Senha |
| --- | --- | --- |
| Administrador | `admin@ufabc.edu.br` | `Admin@123456` |
| Professor | `professor@ufabc.edu.br` | `Professor@123456` |
| Revisor | `revisor@ufabc.edu.br` | `Professor@123456` |

Altere essas credenciais antes de qualquer publicação fora de ambiente local.

## Configuração Local

1. Instale o .NET SDK 8.
2. Instale e configure um servidor MySQL.
3. Copie o arquivo de exemplo de configuração:

```powershell
copy ProgressoAcademico\appsettings.Example.json ProgressoAcademico\appsettings.Development.json
```

4. Ajuste a string de conexão em `ProgressoAcademico/appsettings.Development.json`.
5. Restaure os pacotes:

```powershell
dotnet restore ProgressoAcademico.slnx
```

6. Aplique as migrations:

```powershell
dotnet ef database update --project ProgressoAcademico\ProgressoAcademico.csproj --startup-project ProgressoAcademico\ProgressoAcademico.csproj
```

7. Execute a aplicação:

```powershell
dotnet run --project ProgressoAcademico\ProgressoAcademico.csproj
```

## Validação

Para compilar:

```powershell
dotnet build ProgressoAcademico.slnx --no-restore
```

Para executar os testes:

```powershell
dotnet test ProgressoAcademico.slnx --no-build
```

## Fluxo Principal

1. O professor cria ou acessa sua conta.
2. O professor abre uma solicitação de progressão.
3. O professor cadastra atividades por grupo: ensino, extensão, gestão e pesquisa.
4. Cada atividade pode receber documentos comprobatórios próprios.
5. O professor acompanha elegibilidade auxiliar, pendências e percentual de preenchimento.
6. O administrador atribui a solicitação a um ou mais revisores.
7. O revisor analisa cada atividade e registra parecer.
8. A solicitação pode ser aprovada, reprovada ou devolvida para ajustes.
9. O professor visualiza o retorno, corrige o que for necessário e reenvia quando aplicável.
10. O administrador registra a decisão final.

## Segurança e Privacidade

- Senhas são armazenadas com BCrypt.
- Rotas administrativas exigem perfil adequado.
- Acesso a documentos deve ser validado por usuário, solicitação e perfil.
- Dados pessoais devem ser tratados conforme princípios de finalidade, necessidade e segurança.
- Segredos e strings de conexão reais não devem ser versionados.

## Escopo Atual

Incluído:

- autenticação e autorização;
- portal público;
- portal do professor;
- portal de revisão;
- portal administrativo;
- cadastro de atividades;
- upload de documentos associados às atividades;
- elegibilidade auxiliar;
- dashboards e indicadores básicos;
- migrations e testes automatizados.

Fora do escopo atual:

- tramitação oficial em sistemas institucionais;
- assinatura digital oficial;
- integração real com SIPAC, SouGov ou SIGRH;
- leitura automática completa de documentos institucionais;
- aplicativo mobile;
- substituição de decisão administrativa formal.

## Observações

O sistema foi estruturado com separação por camadas, services, repositories, DTOs, ViewModels e migrations para facilitar manutenção e evolução. As regras de elegibilidade implementadas são auxiliares e devem ser conferidas com normas oficiais antes de qualquer uso institucional.
