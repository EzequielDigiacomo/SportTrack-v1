using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile("appsettings.Development.json", optional: true);

var configuration = builder.Build();
var connectionString = configuration.GetConnectionString("DefaultConnection");

var optionsBuilder = new DbContextOptionsBuilder<SportTrackDbContext>();
optionsBuilder.UseNpgsql(connectionString);

using (var context = new SportTrackDbContext(optionsBuilder.Options))
{
    Console.WriteLine("--- DIAGNÓSTICO DE EVENTOS ---");
    var today = DateTime.UtcNow.Date;
    Console.WriteLine($"Fecha UTC hoy: {today:yyyy-MM-dd}");

    var eventos = context.Eventos
        .Include(e => e.EventoPruebas)
            .ThenInclude(ep => ep.Prueba)
        .ToList();

    foreach (var e in eventos)
    {
        bool isUpcoming = e.Fecha >= today;
        Console.WriteLine($"ID: {e.Id} | Nombre: {e.Nombre} | Fecha: {e.Fecha:yyyy-MM-dd} | Próximo: {isUpcoming}");
        
        foreach (var ep in e.EventoPruebas)
        {
            var p = ep.Prueba;
            Console.WriteLine($"  - Prueba ID: {ep.Id} | Nombre: {p?.Nombre ?? "N/A"} | Pruebas Técnicas: {p?.Id}");
            
            var fases = context.Fases
                .Include(f => f.Etapa)
                .Where(f => f.Etapa.EventoPruebaId == ep.Id)
                .ToList();
            
            if (!fases.Any())
            {
                Console.WriteLine("    [!] ADVERTENCIA: Esta prueba no tiene FASES (Series/Finales) generadas.");
            }
            else
            {
                foreach (var f in fases)
                {
                    Console.WriteLine($"    * Fase ID: {f.Id} | Nombre: {f.NombreFase} | Estado: {f.Estado}");
                }
            }
        }
        Console.WriteLine();
    }
}
