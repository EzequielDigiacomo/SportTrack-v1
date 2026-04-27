using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SportTrack.AccessDatos;
using SportTrack_v1.Api.Hubs;
using SportTrack_v1.Api.Middleware;
using SportTrack_v1.Api.Services;
using SportTrack_v1.Controladores.Auth;
using SportTrack_v1.Controladores.Bote;
using SportTrack_v1.Controladores.Categoria;
using SportTrack_v1.Controladores.Club;
using SportTrack_v1.Controladores.Distancia;
using SportTrack_v1.Controladores.Evento;
using SportTrack_v1.Controladores.Inscripcion;
using SportTrack_v1.Controladores.Participante;
using SportTrack_v1.Controladores.Fase.Dtos;
using SportTrack_v1.Controladores.Participante.Dtos;
using SportTrack_v1.Controladores.Mappings;
using SportTrack_v1.Controladores.Audit;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la base de datos
builder.Services.AddDbContext<SportTrackDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// SignalR para tiempo real
builder.Services.AddSignalR();

// Configuración de CORS
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:3000", "http://localhost:5173" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .WithOrigins(allowedOrigins)
              .AllowCredentials();
    });
});


// Autenticación JWT
var tokenKey = builder.Configuration["TokenKey"] ?? "SportTrackSuperSecretKey2026!ForEducationalPurposeOnly_LongEnoughToBeSecure";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        // Soporte para SignalR con JWT en el query string
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// Inyección de Dependencias
// Botes
builder.Services.AddScoped<IBoteService, BoteService>();
builder.Services.AddScoped<IBoteRepository, BoteRepository>();
// Categorias
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
// Distancias
builder.Services.AddScoped<IDistanciaService, DistanciaService>();
builder.Services.AddScoped<IDistanciaRepository, DistanciaRepository>();
// Inscripciones
builder.Services.AddScoped<IInscripcionService, InscripcionService>();
builder.Services.AddScoped<IInscripcionRepository, InscripcionRepository>();
// Participantes
builder.Services.AddScoped<IParticipanteService, ParticipanteService>();
builder.Services.AddScoped<IParticipanteRepository, ParticipanteRepository>();
// Eventos
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<IEventoRepository, EventoRepository>();
// Fases y Resultados
builder.Services.AddScoped<SportTrack_v1.Controladores.Fase.IEtapaRepository, SportTrack_v1.Controladores.Fase.EtapaRepository>();
builder.Services.AddScoped<SportTrack_v1.Controladores.Fase.IFaseRepository, SportTrack_v1.Controladores.Fase.FaseRepository>();
builder.Services.AddScoped<SportTrack_v1.Controladores.Fase.IFaseService, SportTrack_v1.Controladores.Fase.FaseService>();
builder.Services.AddScoped<SportTrack_v1.Controladores.Resultado.IResultadoRepository, SportTrack_v1.Controladores.Resultado.ResultadoRepository>();
// Notificador (vamos a inyectarlo si es necesario luego)
builder.Services.AddScoped<SportTrack_v1.Api.Services.INotificadorResultados, SportTrack_v1.Api.Services.NotificadorResultados>();
// Clubes
builder.Services.AddScoped<IClubService, ClubService>();
builder.Services.AddScoped<IClubRepository, ClubRepository>();
// Auth
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Auditoria
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuditService, AuditService>();

// AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SportTrack API", Version = "v1" });

    // Configurar el botón 'Authorize' para JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: 'Bearer 12345abcdef'",
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
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Pipeline de la aplicación
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS debe ir ANTES de autenticación y ANTES de HttpsRedirection
app.UseCors("CorsPolicy");

// Comentado en desarrollo para evitar conflictos con el frontend en HTTP
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Mapeo de los Hubs de SignalR
app.MapHub<ResultsHub>("/hubs/results");
app.MapHub<TimingHub>("/hubs/timing");

app.Run();
