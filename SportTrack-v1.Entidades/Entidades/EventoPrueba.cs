using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportTrack_v1.Entidades.Enums;

namespace SportTrack_v1.Entidades.Entidades
{
    public class EventoPrueba
    {
        public int Id { get; set; }
        public int EventoId { get; set; }
        public int PruebaId { get; set; }
        public DateTime FechaHora { get; set; }
        public int MaxParticipantes { get; set; } = 0;
        public string? Pista { get; set; }
        public EstadoEventoEnum Estado { get; set; } = EstadoEventoEnum.Programada; // Usando enum
        
        // Progression Traceability
        public string? PlanProgresionAsignado { get; set; }

        // Navigation properties
        public Evento Evento { get; set; } = null!;
        public Prueba Prueba { get; set; } = null!;
        public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
        public ICollection<Etapa> Etapas { get; set; } = new List<Etapa>();
        public ICollection<ReglaProgresion> ReglasProgresion { get; set; } = new List<ReglaProgresion>();
    }
}
