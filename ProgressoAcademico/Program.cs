using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProgressoAcademico.Application.Interfaces.Repositories;
using ProgressoAcademico.Context;
using ProgressoAcademico.Infrastructure.Repositories;
using ProgressoAcademico.Models;
using ProgressoAcademico.Services.Admin;
using ProgressoAcademico.Services.Atividades;
using ProgressoAcademico.Services.Auth;
using ProgressoAcademico.Services.Comunicados;
using ProgressoAcademico.Services.ConteudoHome;
using ProgressoAcademico.Services.Documentos;
using ProgressoAcademico.Services.Elegibilidade;
using ProgressoAcademico.Services.Revisor;
using ProgressoAcademico.Services.Solicitacoes;
using ProgressoAcademico.Services.Usuarios;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>(optional: true);

var mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("A connection string 'DefaultConnection' nao foi configurada.");

builder.Services.AddDbContext<ProgressoAcademicoDbContext>(options =>
    options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Jwt:Issuer nao foi configurado.");
var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Jwt:Audience nao foi configurado.");
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key nao foi configurado.");

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IDadosAcademicosService, DadosAcademicosService>();
builder.Services.AddScoped<IComunicadoService, ComunicadoService>();
builder.Services.AddScoped<IConteudoHomeService, ConteudoHomeService>();
builder.Services.AddScoped<ISolicitacaoProgressaoService, SolicitacaoProgressaoService>();
builder.Services.AddScoped<IDocumentoService, DocumentoService>();
builder.Services.AddScoped<IAtividadeService, AtividadeService>();
builder.Services.AddScoped<IElegibilidadeService, ElegibilidadeService>();
builder.Services.AddScoped<IAdminSolicitacaoService, AdminSolicitacaoService>();
builder.Services.AddScoped<IAdminUsuarioService, AdminUsuarioService>();
builder.Services.AddScoped<IRevisorSolicitacaoService, RevisorSolicitacaoService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IDadosAcademicosRepository, DadosAcademicosRepository>();
builder.Services.AddScoped<IComunicadoRepository, ComunicadoRepository>();
builder.Services.AddScoped<IConteudoHomeRepository, ConteudoHomeRepository>();
builder.Services.AddScoped<ISolicitacaoProgressaoRepository, SolicitacaoProgressaoRepository>();
builder.Services.AddScoped<IDocumentoRepository, DocumentoRepository>();
builder.Services.AddScoped<IAtividadeRepository, AtividadeRepository>();
builder.Services.AddScoped<IAdminSolicitacaoRepository, AdminSolicitacaoRepository>();
builder.Services.AddScoped<IAdminUsuarioRepository, AdminUsuarioRepository>();
builder.Services.AddScoped<IRevisorSolicitacaoRepository, RevisorSolicitacaoRepository>();

const string AuthScheme = "JwtOrCookie";

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = AuthScheme;
        options.DefaultChallengeScheme = AuthScheme;
    })
    .AddPolicyScheme(AuthScheme, "JWT ou cookie", options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            var authorization = context.Request.Headers.Authorization.ToString();

            return authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? JwtBearerDefaults.AuthenticationScheme
                : CookieAuthenticationDefaults.AuthenticationScheme;
        };
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "ProgressoAcademico.Auth";
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                }

                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                }

                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            }
        };
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            NameClaimType = System.Security.Claims.ClaimTypes.Name,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PerfisAcesso.Professor, policy => policy.RequireRole(PerfisAcesso.Professor));
    options.AddPolicy(PerfisAcesso.Revisor, policy => policy.RequireRole(PerfisAcesso.Revisor));
    options.AddPolicy(PerfisAcesso.Administrador, policy => policy.RequireRole(PerfisAcesso.Administrador));
});

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT no formato: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllers();

app.Run();
