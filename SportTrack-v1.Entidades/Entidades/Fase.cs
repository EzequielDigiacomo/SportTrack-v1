using System;
using System.Collections.Generic;

namespace SportTrack_v1.Entidades.Entidades
{
    public class Fase
    {
        public int Id { get; set; }
        public int EtapaId { get; set; }
        public string NombreFase { get; set; } = string.Empty; // "Serie 1", "Final A", etc
        public int NumeroFase { get; set; } = 1;
        public DateTime? FechaHoraProgramada { get; set; }
        public string Estado { get; set; } = "Programada"; 

        public DateTime? FechaHoraInicioReal { get; set; }
        public DateTime? FechaHoraFinReal { get; set; }


        // Navigation properties
        public Etapa Etapa { get; set; } = null!;
        public ICollection<Resultado> Resultados { get; set; } = new List<Resultado>();
    }
}
