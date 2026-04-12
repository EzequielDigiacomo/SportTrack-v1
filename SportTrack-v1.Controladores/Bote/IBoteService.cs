using SportTrack_v1.Controladores.Bote.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Bote
{
    public interface IBoteService
    {
        Task<IEnumerable<BoteDto>> GetAllBotesAsync();
        Task<BoteDto> GetBoteByIdAsync(int id);
        Task<BoteDto> CreateBoteAsync(BoteCreateDto boteDto);
        Task<BoteDto> UpdateBoteAsync(int id, BoteUpdateDto boteDto);
        Task<bool> DeleteBoteAsync(int id);
        Task<IEnumerable<TipoBoteDto>> GetTiposBoteAsync();
    }
}
