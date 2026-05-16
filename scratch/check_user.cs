using System;
using System.Linq;
using SportTrack.AccessDatos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

class Program
{
    static void Main()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "SportTrack-v1.Api"))
            .AddJsonFile("appsettings.json");
            
        var config = builder.Build();
        var optionsBuilder = new DbContextOptionsBuilder<SportTrackDbContext>();
        optionsBuilder.UseNpgsql(config.GetConnectionString("DefaultConnection"));

        using (var context = new SportTrackDbContext(optionsBuilder.Options))
        {
            var user = context.Usuarios.Include(u => u.Club).FirstOrDefault(u => u.Rol == "Largador" || u.Rol == "Cronometrista" || u.Username == "largador1");
            if (user != null)
            {
                Console.WriteLine($"Usuario: {user.Username}, Rol: {user.Rol}, ClubId: {user.ClubId}");
                var club = user.Club;
                if (club != null)
                {
                    Console.WriteLine($"Club del Usuario: {club.Nombre}, ParentClubId: {club.ParentClubId}");
                }
            }
            else
            {
                Console.WriteLine("No se encontró usuario largador.");
            }

            var eventos = context.Eventos.ToList();
            foreach (var ev in eventos)
            {
                Console.WriteLine($"Evento: {ev.Nombre}, ClubId: {ev.ClubId}");
            }
        }
    }
}
