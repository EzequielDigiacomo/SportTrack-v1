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
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la base de datos
builder.Services.AddDbContext<SportTrackDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// SignalR para tiempo real
builder.Services.AddSignalR();

// Configuración de CORS
var originsConfig = builder.Configuration["AllowedOrigins"];
var configOrigins = !string.IsNullOrEmpty(originsConfig) 
    ? originsConfig.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()).ToArray() 
    : Array.Empty<string>();
var allowedOrigins = configOrigins.Concat(new[] { "http://localhost:3000", "http://localhost:5173", "https://sporttrack-fec.vercel.app" }).Distinct().ToArray();

Console.WriteLine($"Configurando CORS para orígenes: {string.Join(", ", allowedOrigins)}");

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(origin => true) // Permitir cualquier origen para facilitar pruebas en móviles (CORS + Credentials)
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
                // 1. Intentar desde Query String (SignalR)
                var accessToken = context.Request.Query["access_token"];
                
                // 2. Intentar desde Cookies (HttpOnly)
                if (string.IsNullOrEmpty(accessToken))
                {
                    accessToken = context.Request.Cookies["X-Access-Token"];
                }

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken))
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
// SaaS
builder.Services.AddScoped<SportTrack_v1.Controladores.SaaS.ISaaSService, SportTrack_v1.Controladores.SaaS.SaaSService>();

// Auditoria
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuditService, AuditService>();

// Pagos
builder.Services.AddScoped<SportTrack_v1.Controladores.Pago.IPagoService, SportTrack_v1.Controladores.Pago.PagoService>();

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

// Configuración para leer IP real a través del proxy de Render
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Ejecutar migraciones automáticamente al iniciar (con salvaguardas para Render)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<SportTrackDbContext>();
        
        // Salvaguarda para Render: si Categoria ID 11 ya existe en la BD pero la migración no se ha registrado,
        // la eliminamos temporalmente para evitar que la migración falle por clave duplicada.
        try
        {
            var appliedMigrations = context.Database.GetAppliedMigrations();
            if (!appliedMigrations.Contains("20260508003834_AddHabilitacionesToEvento"))
            {
                Console.WriteLine("Safeguard: La migración 'AddHabilitacionesToEvento' no está registrada. Eliminando Categoria ID 11 conflictiva...");
                context.Database.ExecuteSqlRaw(@"
                    DELETE FROM catalogos.""Categorias"" WHERE ""Id"" = 11;
                ");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Safeguard Warning: No se pudo limpiar la Categoria 11 (puede que esté en uso): {ex.Message}");
        }

        if (context.Database.GetPendingMigrations().Any())
        {
            Console.WriteLine("Aplicando migraciones pendientes...");
            context.Database.Migrate();
            Console.WriteLine("Migraciones aplicadas con éxito.");
        }

        // Safeguard: Asegurar que la columna UserAgent existe en la DB.
        context.Database.ExecuteSqlRaw(@"
            ALTER TABLE ""Auditoria"" ADD COLUMN IF NOT EXISTS ""UserAgent"" text NOT NULL DEFAULT '';
        ");
        Console.WriteLine("Safeguard: Verificada la columna UserAgent en Auditoria.");

        // Safeguard: Asegurar que la columna UsarGapVariable existe en la DB.
        try
        {
            context.Database.ExecuteSqlRaw(@"
                ALTER TABLE regatas.""Eventos"" ADD COLUMN IF NOT EXISTS ""UsarGapVariable"" boolean NOT NULL DEFAULT FALSE;
            ");
            Console.WriteLine("Safeguard: Verificada la columna UsarGapVariable en Eventos.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Safeguard Warning: No se pudo verificar/agregar UsarGapVariable: {ex.Message}");
        }

        // Safeguard: Asegurar que las columnas de Perfil en Usuarios existen.
        try
        {
            context.Database.ExecuteSqlRaw(@"
                ALTER TABLE catalogos.""Usuarios"" ADD COLUMN IF NOT EXISTS ""Nombre"" text;
                ALTER TABLE catalogos.""Usuarios"" ADD COLUMN IF NOT EXISTS ""Apellido"" text;
                ALTER TABLE catalogos.""Usuarios"" ADD COLUMN IF NOT EXISTS ""Dni"" text;
                ALTER TABLE catalogos.""Usuarios"" ADD COLUMN IF NOT EXISTS ""Telefono"" text;
            ");
            Console.WriteLine("Safeguard: Verificadas las columnas de Perfil en Usuarios.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Safeguard Warning: No se pudo verificar/agregar Perfil en Usuarios: {ex.Message}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al aplicar migraciones o salvaguardas: {ex.Message}");
    }
}

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

// Las migraciones y salvaguardas se ejecutan al iniciar arriba.

app.MapControllers();

// Mapeo de los Hubs de SignalR
app.MapHub<ResultsHub>("/hubs/results");
app.MapHub<SportTrack_v1.Controladores.Hubs.TimingHub>("/hubs/timing");

// ENDPOINT DE EMERGENCIA (Temporal para arreglar la base de datos en Render)
app.MapGet("/api/fix-db", async (SportTrack.AccessDatos.SportTrackDbContext db) => {
    try {
        await db.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Auditoria"" ADD COLUMN IF NOT EXISTS ""UserAgent"" text NOT NULL DEFAULT '';
        ");
        await db.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE regatas.""Eventos"" ADD COLUMN IF NOT EXISTS ""UsarGapVariable"" boolean NOT NULL DEFAULT FALSE;
        ");
        await db.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE catalogos.""Usuarios"" ADD COLUMN IF NOT EXISTS ""Nombre"" text;
            ALTER TABLE catalogos.""Usuarios"" ADD COLUMN IF NOT EXISTS ""Apellido"" text;
            ALTER TABLE catalogos.""Usuarios"" ADD COLUMN IF NOT EXISTS ""Dni"" text;
            ALTER TABLE catalogos.""Usuarios"" ADD COLUMN IF NOT EXISTS ""Telefono"" text;
        ");
        return Results.Ok(new { Message = "Base de datos arreglada. Las columnas de Usuario (Nombre, Apellido, etc.), UserAgent y UsarGapVariable fueron verificadas/creadas con éxito." });
    } catch (Exception ex) {
        return Results.Problem($"Error al arreglar la base de datos: {ex.Message}");
    }
});

// ENDPOINT TEMPORAL PARA DEBUG
app.MapGet("/api/debug-events", async (SportTrack.AccessDatos.SportTrackDbContext db) => {
    var user = await db.Usuarios.Include(u => u.Club).FirstOrDefaultAsync(u => u.Username == "largador1");
    var events = await db.Eventos.Select(e => new { e.Id, e.Nombre, e.ClubId }).ToListAsync();
    return Results.Ok(new { 
        User = new { user?.Username, user?.ClubId, ParentClubId = user?.Club?.ParentClubId, user?.Rol },
        Events = events
    });
});

app.Run();
