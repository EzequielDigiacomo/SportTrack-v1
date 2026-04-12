using SportTrack_v1.Controladores.Resultado.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Resultado
{
    public interface IResultadoRepository
    {
        Task<IEnumerable<Entidades.Entidades.Resultado>> GetAllAsync();
        Task<Entidades.Entidades.Resultado?> GetByIdAsync(int id);
        Task<Entidades.Entidades.Resultado?> GetByInscripcionIdAsync(int inscripcionId);
        Task<Entidades.Entidades.Resultado> CreateAsync(Entidades.Entidades.Resultado resultado);
        Task<Entidades.Entidades.Resultado> UpdateAsync(Entidades.Entidades.Resultado resultado);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Entidades.Entidades.Resultado>> GetByEventoPruebaIdAsync(int eventoPruebaId);
    }

    public interface IResultadoService
    {
        Task<IEnumerable<ResultadoDto>> GetAllResultadosAsync();
        Task<ResultadoDto> GetResultadoByIdAsync(int id);
        Task<ResultadoDto> UpsertResultadoAsync(ResultadoCreateDto resultadoDto);
        Task<bool> DeleteResultadoAsync(int id);
        Task<IEnumerable<ResultadoDto>> GetResultadosByPruebaAsync(int eventoPruebaId);
    }
}
