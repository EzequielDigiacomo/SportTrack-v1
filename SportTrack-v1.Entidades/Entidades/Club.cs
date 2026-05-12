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
        
        // SaaS Plan
        public int? PlanSaaSId { get; set; }
        public PlanSaaS? PlanSaaS { get; set; }

        // Navigation properties
        public ICollection<Participante> Participantes { get; set; } = new List<Participante>();
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
