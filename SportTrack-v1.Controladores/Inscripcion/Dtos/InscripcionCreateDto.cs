using System.Collections.Generic;

namespace SportTrack_v1.Controladores.Inscripcion.Dtos
{
    public class InscripcionCreateDto
    {
        public int EventoPruebaId { get; set; }
        public int? ParticipanteId { get; set; } // Opcional si es K2/K4 y van todos en Tripulantes
        public int? Carril { get; set; }
        public string NumeroCompetidor { get; set; } = string.Empty;
        
        // Fases
        public bool EsCabezaDeSerie { get; set; } = false;
        public string Fase { get; set; } = "Serie";
        public int NumeroManga { get; set; } = 1;
        public string BoteIdentificador { get; set; } = "A";

        public ICollection<InscripcionTripulanteCreateDto> Tripulantes { get; set; } = new List<InscripcionTripulanteCreateDto>();
    }

    public class InscripcionTripulanteCreateDto
    {
        public int ParticipanteId { get; set; }
        public int? PosicionEnBote { get; set; }
    }

    public class InscripcionUpdateDto
    {
        public int? EventoPruebaId { get; set; }
        public int? Carril { get; set; }
        public string? Estado { get; set; }
        public string? NumeroCompetidor { get; set; }
        public bool? EsCabezaDeSerie { get; set; }
        public string? Fase { get; set; }
        public int? NumeroManga { get; set; }
        public string? BoteIdentificador { get; set; }
    }
}
