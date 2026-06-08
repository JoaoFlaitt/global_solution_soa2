using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrbitGuardAI.API.Middleware;
using OrbitGuardAI.Application.Services;
using OrbitGuardAI.Domain.Entities;
using OrbitGuardAI.Domain.Enums;
using OrbitGuardAI.Domain.Interfaces;
using OrbitGuardAI.Infrastructure.Auth;
using OrbitGuardAI.Infrastructure.Data;
using OrbitGuardAI.Infrastructure.External;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------------- Banco ----------------
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("OrbitGuardDb"));

// --------------- Injeção de Dependência ---------------
builder.Services.AddScoped<IAreaRiscoRepositorio, AreaRiscoRepositorio>();
builder.Services.AddScoped<IAlertaRepositorio, AlertaRepositorio>();
builder.Services.AddScoped<ILeituraRepositorio, LeituraRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

builder.Services.AddSingleton<ISatelliteDataGateway, SatelliteDataGateway>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<IPreditorClimaticoService, PreditorClimaticoService>();
builder.Services.AddScoped<IAlertaService, AlertaService>();
builder.Services.AddScoped<INotificacaoService, NotificacaoService>();

// --------------- CORS ---------------
builder.Services.AddCors(opts =>
    opts.AddPolicy("OrbitGuardCors", p =>
        p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

// --------------- JWT ---------------
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

// --------------- Controllers + Swagger ---------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ORBIT GUARD AI",
        Version = "v1",
        Description = "Plataforma inteligente de previsão e resposta a eventos climáticos extremos. " +
                      "Integra dados de satélites, IoT, IA preditiva, Visão Computacional e Cloud. " +
                      "Global Solution FIAP 2026 — alinhada aos ODS 2, 8, 9, 11 e 13."
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT no formato: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// --------------- Pipeline ---------------
app.UseMiddleware<ExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ORBIT GUARD AI v1");
    c.RoutePrefix = string.Empty; // Swagger na raiz
});
app.UseCors("OrbitGuardCors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// --------------- Seed inicial ---------------
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedData.Inicializar(ctx);
}

app.Run();

/// <summary>
/// Carga inicial de dados — sensores, satélites, áreas de risco e usuário admin.
/// </summary>
public static class SeedData
{
    public static void Inicializar(AppDbContext ctx)
    {
        if (!ctx.Usuarios.Any())
        {
            ctx.Usuarios.Add(new Usuario
            {
                Nome = "Admin Orbit",
                Email = "admin@orbitguard.ai",
                SenhaHash = TokenService.HashSenha("orbit2026"),
                Perfil = "Admin"
            });
        }

        if (!ctx.AreasRisco.Any())
        {
            ctx.AreasRisco.AddRange(
                new AreaRisco { Nome = "Morro do Macaco", Municipio = "Petrópolis", Estado = "RJ", Latitude = -22.51, Longitude = -43.18, RaioKm = 3, PopulacaoEstimada = 18000 },
                new AreaRisco { Nome = "Bacia do Rio Taquari", Municipio = "Lajeado", Estado = "RS", Latitude = -29.46, Longitude = -51.96, RaioKm = 15, PopulacaoEstimada = 90000 },
                new AreaRisco { Nome = "Cerrado Setor Norte", Municipio = "Lábrea", Estado = "AM", Latitude = -7.25, Longitude = -64.79, RaioKm = 50, PopulacaoEstimada = 45000 }
            );
        }

        if (!ctx.Satelites.Any())
        {
            ctx.Satelites.Add(new Satelite("CBERS-4A", "INPE", 628, "SSO"));
            ctx.Satelites.Add(new Satelite("Amazonia-1", "INPE", 752, "SSO"));
            ctx.Satelites.Add(new Satelite("Sentinel-2B", "ESA", 786, "SSO"));
        }

        if (!ctx.Sensores.Any())
        {
            ctx.Sensores.Add(new SensorIoT("Pluvio-PT-001", TipoSensor.Pluviometro, -22.51, -43.18, "Petrópolis"));
            ctx.Sensores.Add(new SensorIoT("Inclino-PT-014", TipoSensor.Inclinometro, -22.50, -43.19, "Petrópolis"));
            ctx.Sensores.Add(new SensorIoT("Rio-Taquari-N1", TipoSensor.NivelRio, -29.46, -51.96, "Lajeado"));
            ctx.Sensores.Add(new SensorIoT("Ar-AM-Labrea", TipoSensor.QualidadeAr, -7.25, -64.79, "Lábrea"));
        }

        ctx.SaveChanges();
    }
}