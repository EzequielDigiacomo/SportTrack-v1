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
            var users = context.Usuarios.Include(u => u.Club).ThenInclude(c => c.ParentClub).ToList();
            Console.WriteLine("=== LISTADO DE USUARIOS ===");
            foreach (var user in users)
            {
                Console.WriteLine($"Usuario: {user.Username}, Rol: {user.Rol}, ClubId: {user.ClubId}, ClubNombre: {user.Club?.Nombre ?? "null"}, ParentClubId: {user.Club?.ParentClubId ?? null}");
            }
            Console.WriteLine("===========================");
        }
    }
}
