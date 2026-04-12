using SportTrack_v1.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Entidades
{
    public class Evento
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string? Ubicacion { get; set; }
        public EstadoEventoEnum Estado { get; set; } = EstadoEventoEnum.Programada; // Usando enum
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaFinInscripciones { get; set; }

        // Navigation property
        public ICollection<EventoPrueba> EventoPruebas { get; set; } = new List<EventoPrueba>();
    }
}
