using System;

namespace SportTrack_v1.Controladores.Resultado.Dtos
{
    public class ResultadoDto
    {
        public int Id { get; set; }
        public int InscripcionId { get; set; }
        public string ParticipanteNombre { get; set; } = string.Empty;
        public string ClubNombre { get; set; } = string.Empty;
        public string ClubSigla { get; set; } = string.Empty;
        public int? Carril { get; set; }
        public TimeSpan? TiempoOficial { get; set; }
        public int? Posicion { get; set; }
        public decimal? Puntos { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }

    public class ResultadoCreateDto
    {
        public int InscripcionId { get; set; }
        public TimeSpan? TiempoOficial { get; set; }
        public int? Posicion { get; set; }
        public decimal? Puntos { get; set; }
        public string? Observaciones { get; set; }
    }
}
