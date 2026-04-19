using System;
using System.Collections.Generic;

namespace SportTrack_v1.Entidades.Entidades
{
    public class Etapa
    {
        public int Id { get; set; }
        public int EventoPruebaId { get; set; }
        public string Nombre { get; set; } = string.Empty; // "Eliminatorias", "Semifinales", "Finales"
        public Enums.TipoEtapaEnum Tipo { get; set; }
        public int Orden { get; set; }

        // Navigation properties
        public EventoPrueba EventoPrueba { get; set; } = null!;
        public ICollection<Fase> Fases { get; set; } = new List<Fase>();
        public ICollection<ReglaProgresion> ReglasComoOrigen { get; set; } = new List<ReglaProgresion>();
        public ICollection<ReglaProgresion> ReglasComoDestino { get; set; } = new List<ReglaProgresion>();
    }
}
