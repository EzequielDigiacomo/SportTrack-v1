using Microsoft.EntityFrameworkCore;
using SportTrack_v1.Controladores.Bote;
using SportTrack_v1.Entidades.Entidades;
using SportTrack.AccessDatos;

public class BoteRepository : IBoteRepository
{
    private readonly SportTrackDbContext _context;

    public BoteRepository(SportTrackDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Bote>> GetAllAsync()
    {
        return await _context.Botes
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Bote?> GetByIdAsync(int id)
    {
        return await _context.Botes
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Bote> CreateAsync(Bote bote)
    {
        _context.Botes.Add(bote);
        await _context.SaveChangesAsync();
        return bote;
    }

    public async Task<Bote> UpdateAsync(Bote bote)
    {
        _context.Entry(bote).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return bote;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var bote = await _context.Botes.FindAsync(id);
        if (bote == null)
            return false;

        _context.Botes.Remove(bote);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Botes.AnyAsync(b => b.Id == id);
    }

    public async Task<bool> ExistsByTipoAsync(string tipo)
    {
        return await _context.Botes.AnyAsync(b => b.Tipo == tipo);
    }
}