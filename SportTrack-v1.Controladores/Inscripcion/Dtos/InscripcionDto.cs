using System;
using System.Collections.Generic;

namespace SportTrack_v1.Controladores.Inscripcion.Dtos
{
    public class InscripcionDto
    {
        public int Id { get; set; }
        public int EventoPruebaId { get; set; }
        public int? ParticipanteId { get; set; }
        public string? ParticipanteNombreCompleto { get; set; }
        public string? ClubNombre { get; set; }
        public string? ClubSigla { get; set; }
        public int? Carril { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public string NumeroCompetidor { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        // Nuevos campos fase y start list
        public bool EsCabezaDeSerie { get; set; }
        public string Fase { get; set; } = string.Empty;
        public int NumeroManga { get; set; }
        public string BoteIdentificador { get; set; } = "A";

        public ICollection<InscripcionTripulanteDto> Tripulantes { get; set; } = new List<InscripcionTripulanteDto>();
    }

    public class InscripcionTripulanteDto
    {
        public int Id { get; set; }
        public int ParticipanteId { get; set; }
        public string? ParticipanteNombreCompleto { get; set; }
        public int? PosicionEnBote { get; set; }
    }
}
