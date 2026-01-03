using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Categoria
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Entidades.Entidades.Categoria>> GetAllAsync();
        Task<Entidades.Entidades.Categoria?> GetByIdAsync(int id);
        Task<Entidades.Entidades.Categoria> CreateAsync(Entidades.Entidades.Categoria categoria);
        Task<Entidades.Entidades.Categoria> UpdateAsync(Entidades.Entidades.Categoria categoria);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Entidades.Entidades.Categoria>> GetByEdadAsync(int edad);
    }
}
