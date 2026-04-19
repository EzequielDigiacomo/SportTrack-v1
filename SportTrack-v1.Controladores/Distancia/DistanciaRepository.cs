// Repositories/DistanciaRepository.cs
using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Controladores.Distancia;
using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.Enums;

public class DistanciaRepository : IDistanciaRepository
{
    private readonly SportTrackDbContext _context;

    public DistanciaRepository(SportTrackDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Distancia>> GetAllAsync()
    {
        return await _context.Distancias
            .AsNoTracking()
            .OrderBy(d => d.DistanciaRegata)
            .ToListAsync();
    }

    public async Task<Distancia?> GetByIdAsync(int id)
    {
        return await _context.Distancias
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Distancia> CreateAsync(Distancia distancia)
    {
        _context.Distancias.Add(distancia);
        await _context.SaveChangesAsync();
        return distancia;
    }

    public async Task<Distancia> UpdateAsync(Distancia distancia)
    {
        _context.Entry(distancia).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return distancia;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var distancia = await _context.Distancias.FindAsync(id);
        if (distancia == null)
            return false;

        _context.Distancias.Remove(distancia);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Distancias.AnyAsync(d => d.Id == id);
    }

    public async Task<bool> ExistsByDistanciaRegataAsync(DistanciaRegataEnum distanciaRegata, int? excludeId = null)
    {
        var query = _context.Distancias.Where(d => d.DistanciaRegata == distanciaRegata);

        if (excludeId.HasValue)
        {
            query = query.Where(d => d.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<Distancia?> GetByDistanciaRegataAsync(DistanciaRegataEnum distanciaRegata)
    {
        return await _context.Distancias
            .FirstOrDefaultAsync(d => d.DistanciaRegata == distanciaRegata);
    }

    public async Task<IEnumerable<Distancia>> GetByRangoMetrosAsync(int metrosMin, int metrosMax)
    {
        var all = await _context.Distancias.AsNoTracking().ToListAsync();
        return all.Where(d => d.Metros >= metrosMin && d.Metros <= metrosMax);
    }
}