using SportTrack_v1.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Entidades
{
    public class Inscripcion
    {
        public int Id { get; set; }
        public int EventoPruebaId { get; set; }
        public int? ParticipanteId { get; set; }
        public int? Carril { get; set; }
        public DateTime FechaInscripcion { get; set; } = DateTime.UtcNow;
        public string NumeroCompetidor { get; set; } = string.Empty;
        public Enums.EstadoInscripcionEnum Estado { get; set; } = EstadoInscripcionEnum.Inscrito; // Usando enum

        // Nuevos campos para Start List y fases
        public bool EsCabezaDeSerie { get; set; } = false;
        public string Fase { get; set; } = "Serie"; // Serie, Semifinal, Final A, Final B
        public int NumeroManga { get; set; } = 1; // Ej: Serie 1, Serie 2...
        public string BoteIdentificador { get; set; } = "A"; // "A" o "B"

        // Navigation properties
        public EventoPrueba EventoPrueba { get; set; } = null!;
        public Participante? Participante { get; set; }
        public ICollection<InscripcionTripulante> Tripulantes { get; set; } = new List<InscripcionTripulante>();
        public Resultado? Resultado { get; set; }
    }
}
