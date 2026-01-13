using Microsoft.EntityFrameworkCore;
using ProgressoAcademico.Context;

var builder = WebApplication.CreateBuilder(args);

// =====================
// Services
// =====================

var mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ProgressoAcademicoDbContext>(options =>
    options.UseMySql(
        mySqlConnection,
        ServerVersion.AutoDetect(mySqlConnection)
    ));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();

// =====================
// App
// =====================

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
