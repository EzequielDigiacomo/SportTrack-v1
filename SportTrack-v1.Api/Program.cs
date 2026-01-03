using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SportTrack.AccessDatos;
using SportTrack_v1.Controladores.Bote;
using SportTrack_v1.Controladores.Categoria;
using SportTrack_v1.Controladores.Distancia;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// Configurar DbContext con PostgreSQL
builder.Services.AddDbContext<SportTrackDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.MigrationsAssembly("SportTrack-v1.AccesoDatos")
    )
);

// Configuración en Program.cs
builder.Services.AddScoped<IBoteService, BoteService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IDistanciaService, DistanciaService>();

builder.Services.AddScoped<IBoteRepository, BoteRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IDistanciaRepository, DistanciaRepository>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

// Agregar controladores
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
