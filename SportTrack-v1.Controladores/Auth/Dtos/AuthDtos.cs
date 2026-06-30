namespace SportTrack_v1.Controladores.Auth.Dtos
{
    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public int? ClubId { get; set; }
        public string? ClubNombre { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? FrecuenciaPago { get; set; }
        public System.DateTime? FechaVencimientoPlan { get; set; }
        public SportTrack_v1.Controladores.SaaS.Dtos.PlanSaaSDto? Plan { get; set; }
    }

    public class RegisterDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = "Club";
        public int? ClubId { get; set; }

        // Datos personales para roles de juez (Largador, Cronometrista, JuezControl)
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Dni { get; set; }
        public string? Telefono { get; set; }
    }

    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public int? ClubId { get; set; }
        public string? ClubNombre { get; set; }
        public bool Activo { get; set; } = true;

        // Datos personales
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Dni { get; set; }
        // Subscription details for frontend sidebar
        public string? FrecuenciaPago { get; set; }
        public System.DateTime? FechaVencimientoPlan { get; set; }
    }

    public class UpdatePerfilDto
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Dni { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
    }
}
