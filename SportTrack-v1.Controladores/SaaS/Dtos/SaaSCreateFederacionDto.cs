using System.ComponentModel.DataAnnotations;

namespace SportTrack_v1.Controladores.SaaS.Dtos
{
    public class SaaSCreateFederacionDto
    {
        // Datos de la Federación
        [Required]
        public string Nombre { get; set; } = string.Empty;
        public string? Sigla { get; set; }
        [Required]
        public string Email { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        
        // Datos del Administrador Inicial
        [Required]
        public string AdminUsername { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string AdminEmail { get; set; } = string.Empty;
        [Required]
        public string AdminPassword { get; set; } = string.Empty;
    }
}
