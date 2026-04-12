using SportTrack_v1.Controladores.Bote.Dtos;
using SportTrack_v1.Controladores.Categoria.Dtos;
using SportTrack_v1.Controladores.Distancia.Dtos;
using System;
using System.Collections.Generic;

namespace SportTrack_v1.Controladores.Evento.Dtos
{
    public class EventoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string? Ubicacion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaFinInscripciones { get; set; }
        public bool InscripcionesAbiertas => Estado == "Programada" && (!FechaFinInscripciones.HasValue || FechaFinInscripciones.Value > DateTime.UtcNow);
    }

    public class EventoCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string? Ubicacion { get; set; }
        public DateTime? FechaFinInscripciones { get; set; }
    }

    public class EventoUpdateDto
    {
        public string? Nombre { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Ubicacion { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaFinInscripciones { get; set; }
    }

    public class EventoPruebaDto
    {
        public int Id { get; set; }
        public int EventoId { get; set; }
        public int PruebaId { get; set; }
        public PruebaDto? Prueba { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Estado { get; set; }
        public int CantidadInscritos { get; set; }
    }

    public class EventoPruebaCreateDto
    {
        public int CategoriaId { get; set; }
        public int BoteId { get; set; }
        public int DistanciaId { get; set; }
        public int SexoId { get; set; } = 1; // Mixto por defecto
        public DateTime? FechaHora { get; set; }
    }

    public class PruebaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public CategoriaDto Categoria { get; set; } = null!;
        public BoteDto Bote { get; set; } = null!;
        public DistanciaDto Distancia { get; set; } = null!;
        public SexoDto? Sexo { get; set; }
        public string SexoNombre { get; set; } = string.Empty;
        public int SexoId { get; set; }
    }

    public class SexoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
