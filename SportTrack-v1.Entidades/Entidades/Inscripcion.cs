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
        public int ParticipanteId { get; set; }
        public DateTime FechaInscripcion { get; set; } = DateTime.UtcNow;
        public string NumeroCompetidor { get; set; } = string.Empty;
        public Enums.EstadoInscripcionEnum Estado { get; set; } = EstadoInscripcionEnum.Inscrito; // Usando enum

        // Navigation properties
        public EventoPrueba EventoPrueba { get; set; } = null!;
        public Participante Participante { get; set; } = null!;
        public Resultado? Resultado { get; set; }
    }
}
