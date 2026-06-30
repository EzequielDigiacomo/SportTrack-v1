using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.Auth;
using SportTrack_v1.Controladores.Auth.Dtos;
using System.Linq;
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
            
            // Configurar la Cookie HttpOnly
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps, // Solo true si es HTTPS
                SameSite = Request.IsHttps ? SameSiteMode.None : SameSiteMode.Lax, 
                Expires = DateTime.UtcNow.AddHours(5)
            };

            Response.Cookies.Append("X-Access-Token", result.Token, cookieOptions);

            // Devolvemos el resultado pero ya no es estrictamente necesario que el front guarde el token en localStorage
            return Ok(result);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("X-Access-Token", new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = Request.IsHttps ? SameSiteMode.None : SameSiteMode.Lax
            });
            return Ok(new { message = "Sesión cerrada correctamente" });
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            await _authService.RegisterAsync(registerDto);
            return Ok(new { message = "Usuario registrado con éxito" });
        }

        [HttpGet("usuarios")]
        [Authorize]
        public async Task<ActionResult> GetUsuarios()
        {
            var username = User.Identity?.Name;
            var result = await _authService.GetUsuariosAsync(username);
            return Ok(result);
        }

        [HttpPut("usuarios/{id}/password")]
        [Authorize]
        public async Task<ActionResult> UpdatePassword(int id, [FromBody] string newPassword)
        {
            await _authService.UpdatePasswordAsync(id, newPassword);
            return Ok(new { message = "Contraseña actualizada con éxito" });
        }

        [HttpPut("usuarios/{id}/perfil")]
        [Authorize]
        public async Task<ActionResult> UpdatePerfil(int id, [FromBody] UpdatePerfilDto dto)
        {
            await _authService.UpdatePerfilAsync(id, dto);
            return Ok(new { message = "Perfil actualizado con éxito" });
        }

        [HttpPatch("usuarios/{id}/toggle-activo")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult> ToggleActivo(int id)
        {
            await _authService.ToggleActivoAsync(id);
            return Ok(new { message = "Estado de cuenta actualizado correctamente" });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UsuarioDto>> GetMe()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var result = await _authService.GetMeAsync(username);
            return Ok(result);
        }
    }
}
