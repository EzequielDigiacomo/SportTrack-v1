using SportTrack_v1.Controladores.Auth.Dtos;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Auth
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<bool> RegisterAsync(RegisterDto registerDto);
        Task<bool> UserExistsAsync(string username);
        Task<System.Collections.Generic.IEnumerable<UsuarioDto>> GetUsuariosAsync(string? requesterUsername = null);
        Task<bool> UpdatePasswordAsync(int id, string newPassword);
        Task<UsuarioDto> GetMeAsync(string username);
        Task<bool> ToggleActivoAsync(int id);
        Task<bool> UpdatePerfilAsync(int id, UpdatePerfilDto dto);
    }
}
