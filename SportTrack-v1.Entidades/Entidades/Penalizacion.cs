using SportTrack_v1.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Entidades
{
    public class Penalizacion
    {
        public int Id { get; set; }
        public int ResultadoId { get; set; }
        public TipoPenalizacionEnum TipoPenalizacion { get; set; } // Usando enum
        public string? Descripcion { get; set; }
        public TimeSpan? TiempoPenalizacion { get; set; }
        public SeveridadPenalizacionEnum Severidad { get; set; } // Usando enum
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public string? JuezAsignado { get; set; }

        // Navigation property
        public Resultado Resultado { get; set; } = null!;
    }
}
