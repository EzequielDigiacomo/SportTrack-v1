using SportTrack_v1.Controladores.Club.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Club
{
    public interface IClubRepository
    {
        Task<IEnumerable<Entidades.Entidades.Club>> GetAllAsync();
        Task<Entidades.Entidades.Club?> GetByIdAsync(int id);
        Task<Entidades.Entidades.Club> CreateAsync(Entidades.Entidades.Club club);
        Task<Entidades.Entidades.Club> UpdateAsync(Entidades.Entidades.Club club);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }

    public interface IClubService
    {
        Task<IEnumerable<ClubDto>> GetAllClubesAsync();
        Task<ClubDto> GetClubByIdAsync(int id);
        Task<ClubDto> CreateClubAsync(ClubCreateDto clubDto);
        Task<ClubDto> UpdateClubAsync(int id, ClubUpdateDto clubDto);
        Task<bool> DeleteClubAsync(int id);
    }
}
