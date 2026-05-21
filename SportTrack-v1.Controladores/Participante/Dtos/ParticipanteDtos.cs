using System;

namespace SportTrack_v1.Controladores.Participante.Dtos
{
    public class ParticipanteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public int SexoId { get; set; }
        public string SexoNombre { get; set; } = string.Empty;
        public int? CategoriaId { get; set; }
        public string? CategoriaNombre { get; set; }
        public int? ClubId { get; set; }
        public string? ClubNombre { get; set; }
        public string? Pais { get; set; }
        public string? Dni { get; set; }
        public string? Email { get; set; }
        public int Edad { get; set; }
        public bool PagoAfiliacionAlDia { get; set; } = true;
    }

    public class ParticipanteCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public int SexoId { get; set; }
        public int? CategoriaId { get; set; }
        public int? ClubId { get; set; }
        public string? Pais { get; set; }
        public string? Dni { get; set; }
        public string? Email { get; set; }
        public bool PagoAfiliacionAlDia { get; set; } = true;
    }
}
