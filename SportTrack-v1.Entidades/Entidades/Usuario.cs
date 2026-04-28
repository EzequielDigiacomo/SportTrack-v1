using System;

namespace SportTrack_v1.Entidades.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = "Club"; // "Admin", "Club", "Largador", "Cronometrista"
        
        public int? ClubId { get; set; }
        
        // Navigation properties
        public Club? Club { get; set; }
        
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public bool Activo { get; set; } = true;
    }
}
