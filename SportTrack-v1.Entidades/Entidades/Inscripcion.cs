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
        public DateTime FechaInscripcion { get; set; } = DateTime.UtcNow;
        public string NumeroCompetidor { get; set; } = string.Empty;
        public bool EsCabezaDeSerie { get; set; } = false;
        public Enums.EstadoInscripcionEnum Estado { get; set; } = EstadoInscripcionEnum.Inscrito;
        public bool Pagado { get; set; } = false;

        // Navigation properties
        public EventoPrueba EventoPrueba { get; set; } = null!;
        public Participante? Participante { get; set; }
        public ICollection<InscripcionTripulante> Tripulantes { get; set; } = new List<InscripcionTripulante>();
        
        // Relación con el historial deportivo (todas sus bajadas en heats/finales)
        public ICollection<Resultado> Resultados { get; set; } = new List<Resultado>();
    }
}
