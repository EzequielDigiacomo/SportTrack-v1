using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Controladores.Auth.Dtos;
using SportTrack_v1.Controladores.Exceptions;
using SportTrack_v1.Entidades.Entidades;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Auth
{
    public class AuthService : IAuthService
    {
        private readonly SportTrackDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly Audit.IAuditService _auditService;

        public AuthService(SportTrackDbContext context, ITokenService tokenService, IMapper mapper, Audit.IAuditService auditService)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
            _auditService = auditService;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var cleanUsername = loginDto.Username.Trim().ToLower();
            var cleanPassword = loginDto.Password.Trim();

            Console.WriteLine($"--- INTENTO DE LOGIN: {cleanUsername} ---");

            var user = await _context.Usuarios
                .Include(u => u.Club)
                    .ThenInclude(c => c.ParentClub) // Importante para ver la jerarquía
                .FirstOrDefaultAsync(u => u.Username == cleanUsername);

            if (user == null) 
            {
                Console.WriteLine($"USUARIO NO ENCONTRADO: {cleanUsername}");
                await _auditService.RegistrarAccionAsync("LOGIN_FAILED", $"Intento fallido: Usuario '{cleanUsername}' no encontrado.", cleanUsername, "Auth");
                throw new UnauthorizedException("Usuario no encontrado en la base de datos");
            }

            Console.WriteLine($"USUARIO ENCONTRADO. Verificando hash para: {cleanUsername}");

            if (!BCrypt.Net.BCrypt.Verify(cleanPassword, user.PasswordHash))
            {
                Console.WriteLine($"CONTRASEÑA INCORRECTA para: {cleanUsername}");
                await _auditService.RegistrarAccionAsync("LOGIN_FAILED", $"Intento fallido: Contraseña incorrecta para usuario '{cleanUsername}'.", cleanUsername, "Auth");
                throw new UnauthorizedException("Contraseña incorrecta. Verificá mayúsculas/minúsculas.");
            }

            // Verificar que la cuenta esté habilitada
            if (!user.Activo)
            {
                Console.WriteLine($"CUENTA DESHABILITADA: {cleanUsername}");
                await _auditService.RegistrarAccionAsync("LOGIN_BLOCKED", $"Acceso bloqueado: cuenta '{cleanUsername}' está deshabilitada.", cleanUsername, "Auth");
                throw new UnauthorizedException("Tu cuenta está temporalmente deshabilitada. Contactá al administrador.");
            }

            // SaaS Enforcement: Verificar si la federación madre está activa
            if (user.Rol != "SuperAdmin" && user.Club != null)
            {
                var federacionMadre = user.Club.ParentClub ?? user.Club;
                if (!federacionMadre.Activo)
                {
                    Console.WriteLine($"FEDERACIÓN SUSPENDIDA: {federacionMadre.Nombre} para usuario {cleanUsername}");
                    await _auditService.RegistrarAccionAsync("LOGIN_BLOCKED", $"Acceso bloqueado: la federación '{federacionMadre.Nombre}' está suspendida.", cleanUsername, "Auth");
                    throw new UnauthorizedException("El acceso de tu federación ha sido suspendido temporalmente por el administrador del sistema.");
                }
            }

            Console.WriteLine($"LOGIN EXITOSO: {cleanUsername}");

            var response = _mapper.Map<AuthResponseDto>(user);
            response.Token = _tokenService.CreateToken(user);
            
            await _auditService.RegistrarAccionAsync("LOGIN_SUCCESS", $"Usuario '{user.Username}' inició sesión correctamente.", user.Username, "Auth");

            return response;
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            if (await UserExistsAsync(registerDto.Username))
                throw new BadRequestException("El nombre de usuario ya existe");

            var user = _mapper.Map<Usuario>(registerDto);
            user.Username = registerDto.Username.ToLower();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            
            _context.Usuarios.Add(user);
            var res = await _context.SaveChangesAsync() > 0;

            if (res)
            {
                await _auditService.RegistrarAccionAsync("REGISTER_USER", 
                    $"Nuevo usuario registrado: '{user.Username}' (Rol: {user.Rol})", null, "Auth");
            }

            return res;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Usuarios.AnyAsync(u => u.Username == username.ToLower());
        }

        public async Task<System.Collections.Generic.IEnumerable<UsuarioDto>> GetUsuariosAsync()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Club)
                .ToListAsync();
            
            return _mapper.Map<System.Collections.Generic.IEnumerable<UsuarioDto>>(usuarios);
        }

        public async Task<bool> UpdatePasswordAsync(int id, string newPassword)
        {
            var user = await _context.Usuarios.FindAsync(id);
            if (user == null)
            {
                throw new NotFoundException($"Usuario con ID {id} no encontrado");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _context.Usuarios.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<UsuarioDto> GetMeAsync(string username)
        {
            var user = await _context.Usuarios
                .Include(u => u.Club)
                    .ThenInclude(c => c.ParentClub)
                .FirstOrDefaultAsync(u => u.Username == username.ToLower());

            if (user == null) throw new NotFoundException("Usuario no encontrado");

            // SaaS Enforcement en tiempo real
            if (user.Rol != "SuperAdmin" && user.Club != null)
            {
                var federacionMadre = user.Club.ParentClub ?? user.Club;
                if (!federacionMadre.Activo)
                {
                    throw new UnauthorizedException("El acceso de tu federación ha sido suspendido.");
                }
            }

            return _mapper.Map<UsuarioDto>(user);
        }

        public async Task<bool> ToggleActivoAsync(int id)
        {
            var user = await _context.Usuarios.FindAsync(id);
            if (user == null)
                throw new NotFoundException($"Usuario con ID {id} no encontrado");

            user.Activo = !user.Activo;
            _context.Usuarios.Update(user);
            var result = await _context.SaveChangesAsync() > 0;

            var accion = user.Activo ? "USUARIO_HABILITADO" : "USUARIO_DESHABILITADO";
            await _auditService.RegistrarAccionAsync(accion,
                $"Cuenta '{user.Username}' (Rol: {user.Rol}) {(user.Activo ? "habilitada" : "deshabilitada")} por administrador.",
                null, "Auth");

            return result;
        }
    }
}
