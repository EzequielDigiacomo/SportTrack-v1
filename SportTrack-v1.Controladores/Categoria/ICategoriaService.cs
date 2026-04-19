using SportTrack_v1.Controladores.Categoria.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Categoria
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaDto>> GetAllCategoriasAsync();
        Task<CategoriaDto> GetCategoriaByIdAsync(int id);
        Task<CategoriaDto> CreateCategoriaAsync(CategoriaCreateDto categoriaDto);
        Task<CategoriaDto> UpdateCategoriaAsync(int id, CategoriaUpdateDto categoriaDto);
        Task<bool> DeleteCategoriaAsync(int id);
        Task<IEnumerable<CategoriaEdadDto>> GetCategoriasEdadAsync();
        Task<IEnumerable<CategoriaDto>> GetCategoriasByEdadAsync(int edad);
    }
}
