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
        public int? PlanSaaSId { get; set; }
        public string? PlanNombre { get; set; }
        public string? FrecuenciaPago { get; set; }
        public DateTime? FechaAltaPlan { get; set; }
        public DateTime? FechaVencimientoPlan { get; set; }
        public bool BloqueadoPorFaltaDePago { get; set; }
        public bool PagoAfiliacionAlDia { get; set; } = true;
        public bool SolicitudPagoPendiente { get; set; } = false;
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
        public string? FrecuenciaPago { get; set; }
        public DateTime? FechaAltaPlan { get; set; }
        public DateTime? FechaVencimientoPlan { get; set; }
        public bool BloqueadoPorFaltaDePago { get; set; }
        public bool PagoAfiliacionAlDia { get; set; } = true;
        public bool SolicitudPagoPendiente { get; set; } = false;
    }

    public class ClubUpdateDto : ClubCreateDto { }
}
