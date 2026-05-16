using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Entidades.Entidades;
using System;
using System.Linq;
using System.Threading.Tasks;

public class DbChecker
{
    public static async Task CheckEvents(SportTrackDbContext context)
    {
        var events = await context.Eventos.Include(e => e.Club).ToListAsync();
        Console.WriteLine($"Total events in DB: {events.Count}");
        foreach (var e in events)
        {
            Console.WriteLine($"- Event: {e.Nombre}, Date: {e.Fecha}, ClubId: {e.ClubId}, ClubName: {e.Club?.Nombre ?? "NULL"}");
        }

        var clubs = await context.Clubes.ToListAsync();
        Console.WriteLine($"\nTotal clubs in DB: {clubs.Count}");
        foreach (var c in clubs)
        {
            Console.WriteLine($"- Club: {c.Nombre}, Id: {c.Id}, ParentId: {c.ParentClubId}");
        }
    }
}
