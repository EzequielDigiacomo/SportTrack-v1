using System;
using System.Collections.Generic;

namespace SportTrack_v1.Entidades.Entidades
{
    public class Club
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Sigla { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Ubicacion { get; set; }
        public bool Activo { get; set; } = true;
        
        // Jerarquía SaaS: Un club puede ser una Federación (Parent == null) 
        // o un Club Afiliado (Parent != null)
        public int? ParentClubId { get; set; }
        public Club? ParentClub { get; set; }
        public ICollection<Club> Afiliados { get; set; } = new List<Club>();
        
        // SaaS Plan
        public int? PlanSaaSId { get; set; }
        public PlanSaaS? PlanSaaS { get; set; }

        // Subscription / Payment Status
        public string? FrecuenciaPago { get; set; } // "Mensual", "Anual"
        public DateTime? FechaAltaPlan { get; set; }
        public DateTime? FechaVencimientoPlan { get; set; }
        public bool BloqueadoPorFaltaDePago { get; set; } = false;
        public bool PagoAfiliacionAlDia { get; set; } = true;
        public bool SolicitudPagoPendiente { get; set; } = false;

        // Navigation properties
        public ICollection<Participante> Participantes { get; set; } = new List<Participante>();
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
