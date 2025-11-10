var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Éssa parte é muito importante é aqui que estou definindo todos os serviços que a aplicação vai usar.
// perceba que o builder.Services é uma coleção de serviços que a aplicação vai utilizar.
// existem muitos serviços que podem ser adicionados aqui, dependendo das necessidades da aplicação.
// o entyti framework, autenticação, autorização, logging, etc.
// todos serão servições que a aplicação pode usar.


builder.Services.AddControllers();
// o builder.Services.AddDbContext é usado para adicionar o contexto do banco de dados à aplicação.
// nesse caso, estou usando o contexto ProgressoAcademicoContext que está definido na pasta Data.
// o options.UseSqlServer é usado para configurar a conexão com o banco de dados MySQL Server.

builder.Services.AddOpenApi();

var app = builder.Build();

// configura o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection é usado para redirecionar todas as requisições HTTP para HTTPS.
app.UseHttpsRedirection();
// app.UseAuthorization é usado para habilitar a autorização na aplicação.
app.UseAuthorization();
// app.MapControllers é usado para mapear os controladores da aplicação.
app.MapControllers();
// por fim, o app.Run é usado para iniciar a aplicação.
app.Run();
