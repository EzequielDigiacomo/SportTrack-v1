using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportTrack_v1.Entidades.Entidades;

namespace SportTrack_v1.Controladores.Bote
{
    public interface IBoteRepository
    {
        Task<IEnumerable<Entidades.Entidades.Bote>> GetAllAsync();
        Task<Entidades.Entidades.Bote?> GetByIdAsync(int id);
        Task<Entidades.Entidades.Bote> CreateAsync(Entidades.Entidades.Bote bote);
        Task<Entidades.Entidades.Bote> UpdateAsync(Entidades.Entidades.Bote bote);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
