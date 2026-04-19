using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Entidades
{
    public class InscripcionTripulante
    {
        public int Id { get; set; }
        public int InscripcionId { get; set; }
        public int ParticipanteId { get; set; }
        
        /// <summary>
        /// Representa la posición del atleta en el bote (ej: 1 para proa, 2, 3, 4 para popa en un K4)
        /// </summary>
        public int? PosicionEnBote { get; set; }

        // Navigation properties
        public Inscripcion Inscripcion { get; set; } = null!;
        public Participante Participante { get; set; } = null!;
    }
}
