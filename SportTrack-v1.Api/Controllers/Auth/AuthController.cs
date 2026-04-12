using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.Auth;
using SportTrack_v1.Controladores.Auth.Dtos;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            await _authService.RegisterAsync(registerDto);
            return Ok(new { message = "Usuario registrado con éxito" });
        }

        [HttpGet("usuarios")]
        public async Task<ActionResult> GetUsuarios()
        {
            var result = await _authService.GetUsuariosAsync();
            return Ok(result);
        }

        [HttpPut("usuarios/{id}/password")]
        public async Task<ActionResult> UpdatePassword(int id, [FromBody] string newPassword)
        {
            await _authService.UpdatePasswordAsync(id, newPassword);
            return Ok(new { message = "Contraseña actualizada con éxito" });
        }
    }
}
