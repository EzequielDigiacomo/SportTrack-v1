using System;
using System.Collections.Generic;

namespace SportTrack_v1.Entidades.Entidades
{
    public class Resultado
    {
        public int Id { get; set; }
        public int FaseId { get; set; }
        public int InscripcionId { get; set; }
        
        public int? Carril { get; set; }
        public bool EsCabezaDeSerie { get; set; } = false;

        // Tiempos
        public TimeSpan? TiempoOficial { get; set; }

        // Posición y métricas
        public int? Posicion { get; set; }
        public decimal? Puntos { get; set; }
        public decimal? VelocidadMedia { get; set; }

        // Estado del resultado
        public Enums.EstadoResultadoEnum Estado { get; set; } = Enums.EstadoResultadoEnum.Pendiente;

        // Información adicional
        public string? Observaciones { get; set; }

        // Progression Traceability
        public int? FaseOrigenId { get; set; }
        public string? ReglaClasificacionAplicada { get; set; }

        // Auditoría
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public DateTime? FechaActualizacion { get; set; }
        public string? UsuarioRegistro { get; set; }
        public string? UsuarioActualizacion { get; set; }

        // Navigation properties
        public Fase Fase { get; set; } = null!;
        public Inscripcion Inscripcion { get; set; } = null!;
        public ICollection<Penalizacion> Penalizaciones { get; set; } = new List<Penalizacion>();
    }
}
