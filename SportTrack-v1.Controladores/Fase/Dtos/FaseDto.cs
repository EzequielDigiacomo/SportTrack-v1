using System;
using System.Collections.Generic;

namespace SportTrack_v1.Controladores.Fase.Dtos
{
    public class FaseDto
    {
        public int Id { get; set; }
        public int EtapaId { get; set; }
        public string EtapaNombre { get; set; } = string.Empty;
        public int EventoPruebaId { get; set; }
        public string NombreFase { get; set; } = string.Empty;
        public int NumeroFase { get; set; }
        public int EtapaOrden { get; set; }
        public DateTime? FechaHoraProgramada { get; set; }
        public string Estado { get; set; } = string.Empty;

        public SportTrack_v1.Controladores.Evento.Dtos.EventoPruebaDto? Prueba { get; set; }
        public List<ResultadoFaseDto> Resultados { get; set; } = new List<ResultadoFaseDto>();
    }

    public class ResultadoFaseDto
    {
        public int Id { get; set; }
        public int FaseId { get; set; }
        public int InscripcionId { get; set; }
        public int? ParticipanteId { get; set; }
        public string? NumeroCompetidor { get; set; }
        public string? ParticipanteNombre { get; set; }
        public string? ClubNombre { get; set; }
        public string? ClubSigla { get; set; }
        
        public int? Carril { get; set; }
        public bool EsCabezaDeSerie { get; set; }

        public List<SportTrack_v1.Controladores.Inscripcion.Dtos.InscripcionTripulanteDto> Tripulantes { get; set; } = new();

        public TimeSpan? TiempoOficial { get; set; }
        public int? Posicion { get; set; }
        public string Estado { get; set; } = string.Empty;
    }

    public class FaseBatchUpdateDto
    {
        public int Id { get; set; }
        public DateTime FechaHoraProgramada { get; set; }
    }

    public class FaseDetailsUpdateDto
    {
    }
}
