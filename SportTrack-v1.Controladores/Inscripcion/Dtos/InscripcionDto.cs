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
        public DateTime FechaInscripcion { get; set; }
        public string NumeroCompetidor { get; set; } = string.Empty;
        public bool EsCabezaDeSerie { get; set; }
        public string Estado { get; set; } = string.Empty;
        public bool Pagado { get; set; }
        public int? ClubId { get; set; }
        public int? ParticipanteClubId { get; set; }
        public string? EventoNombre { get; set; }
        public string? PruebaNombre { get; set; }

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
