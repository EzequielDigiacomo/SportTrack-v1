// Repositories/CategoriaRepository.cs
using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Controladores.Categoria;
using SportTrack_v1.Entidades.Entidades;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly SportTrackDbContext _context;

    public CategoriaRepository(SportTrackDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Categoria>> GetAllAsync()
    {
        return await _context.Categorias
            .AsNoTracking()
            .OrderBy(c => c.EdadMin)
            .ToListAsync();
    }

    public async Task<Categoria?> GetByIdAsync(int id)
    {
        return await _context.Categorias
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Categoria> CreateAsync(Categoria categoria)
    {
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();
        return categoria;
    }

    public async Task<Categoria> UpdateAsync(Categoria categoria)
    {
        _context.Entry(categoria).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return categoria;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null)
            return false;

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Categorias.AnyAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Categoria>> GetByEdadAsync(int edad)
    {
        return await _context.Categorias
            .Where(c => (!c.EdadMin.HasValue || c.EdadMin <= edad) &&
                       (!c.EdadMax.HasValue || c.EdadMax >= edad))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> ExistsByNombreAsync(string nombre, int? excludeId = null)
    {
        var query = _context.Categorias.Where(c => c.Nombre == nombre);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> ExistsRangoEdadSuperpuestoAsync(int? edadMin, int? edadMax, int? excludeId = null)
    {
        var query = _context.Categorias.AsQueryable();

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        // Lógica para detectar superposición de rangos
        return await query.AnyAsync(c =>
            // Caso 1: Nueva categoría dentro de una existente
            ((!edadMin.HasValue || !c.EdadMin.HasValue || edadMin >= c.EdadMin) &&
             (!edadMax.HasValue || !c.EdadMax.HasValue || edadMax <= c.EdadMax)) ||

            // Caso 2: Nueva categoría que contiene una existente
            ((!c.EdadMin.HasValue || !edadMin.HasValue || c.EdadMin >= edadMin) &&
             (!c.EdadMax.HasValue || !edadMax.HasValue || c.EdadMax <= edadMax)) ||

            // Caso 3: Solapamiento parcial por el inicio
            ((!edadMin.HasValue || !c.EdadMax.HasValue || edadMin <= c.EdadMax) &&
             (!edadMax.HasValue || !c.EdadMin.HasValue || edadMax >= c.EdadMin))
        );
    }
}