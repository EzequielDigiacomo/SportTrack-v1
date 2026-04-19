using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Distancia
{
    public interface IDistanciaRepository
    {
        Task<IEnumerable<Entidades.Entidades.Distancia>> GetAllAsync();
        Task<Entidades.Entidades.Distancia?> GetByIdAsync(int id);
        Task<Entidades.Entidades.Distancia> CreateAsync(Entidades.Entidades.Distancia distancia);
        Task<Entidades.Entidades.Distancia> UpdateAsync(Entidades.Entidades.Distancia distancia);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
