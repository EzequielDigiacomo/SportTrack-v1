using System;

namespace SportTrack_v1.Entidades.Entidades
{
    public class ReglaProgresion
    {
        public int Id { get; set; }
        public int EventoPruebaId { get; set; }
        
        public int EtapaOrigenId { get; set; }
        public int EtapaDestinoId { get; set; }

        public int PosicionDesde { get; set; }
        public int PosicionHasta { get; set; }
        
        public bool PorTiempo { get; set; } = false; // Si es true, se toman los mejores tiempos globales de la etapa origen
        public int? CantidadATomar { get; set; } // Para casos como "el mejor 1ero" o "los 2 mejores tiempos"

        // Navigation properties
        public EventoPrueba EventoPrueba { get; set; } = null!;
        public Etapa EtapaOrigen { get; set; } = null!;
        public Etapa EtapaDestino { get; set; } = null!;
    }
}
