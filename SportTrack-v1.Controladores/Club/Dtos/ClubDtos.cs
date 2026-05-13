using System.ComponentModel.DataAnnotations;

namespace SportTrack_v1.Controladores.Club.Dtos
{
    public class ClubDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Sigla { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Ubicacion { get; set; }
        public bool Activo { get; set; }
        public int CantidadAtletas { get; set; }
        public int? ParentClubId { get; set; } // ID de la federación madre (null si es federación raíz)
        public string? ParentClubNombre { get; set; }
    }

    public class ClubCreateDto
    {
        [Required(ErrorMessage = "El nombre del club es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
        
        [StringLength(10)]
        public string? Sigla { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Ubicacion { get; set; }
        public bool Activo { get; set; } = true;
        public int? ParentClubId { get; set; } // Federación madre al crear un sub-club
    }

    public class ClubUpdateDto : ClubCreateDto { }
}
